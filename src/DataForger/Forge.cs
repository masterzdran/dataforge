using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DataForger
{
    /// <summary>
    /// Entry point and fluent builder for synthetic data generation.
    /// </summary>
    /// <example>
    /// <code>
    /// var customer = Forge.ForCountry(Country.PT).WithSeed(123).Create&lt;Customer&gt;();
    /// var customers = Forge.ForCountry(Country.PT).Create&lt;Customer&gt;(100);
    /// </code>
    /// </example>
    /// <remarks>
    /// Instances are safe to share across threads: built-in generation uses a private random source
    /// per call and the generator registry is a <see cref="ConcurrentDictionary{TKey, TValue}"/>.
    /// Configure the instance (seed, registrations) before generating from multiple threads.
    /// </remarks>
    public sealed class Forge
    {
        private readonly ConcurrentDictionary<Type, Func<IGenerationContext, object>> _factories;

        // ponytail: boxed int keeps the seed read/write atomic without a lock.
        private volatile object _seed;

        /// <summary>Initializes a new instance of the <see cref="Forge"/> class for the specified country using the built-in providers.</summary>
        /// <param name="country">Target country.</param>
        public Forge(Country country)
            : this(country, new CountryDataProviderRegistry())
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Forge"/> class for the specified country using a custom provider registry.</summary>
        /// <param name="country">Target country.</param>
        /// <param name="registry">Provider registry used to resolve the country provider.</param>
        public Forge(Country country, ICountryDataProviderRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            Provider = registry.Get(country);
            Country = country;
            _factories = new ConcurrentDictionary<Type, Func<IGenerationContext, object>>();
            RegisterBuiltInGenerators(_factories);
        }

        /// <summary>Gets the target country.</summary>
        public Country Country { get; }

        /// <summary>Gets the resolved country data provider.</summary>
        public ICountryDataProvider Provider { get; }

        /// <summary>Creates a builder for the specified country using the built-in providers.</summary>
        /// <param name="country">Target country.</param>
        /// <returns>A new <see cref="Forge"/> instance.</returns>
        public static Forge ForCountry(Country country)
        {
            return new Forge(country);
        }

        /// <summary>Creates a builder for the specified country using a custom provider registry.</summary>
        /// <param name="country">Target country.</param>
        /// <param name="registry">Provider registry used to resolve the country provider.</param>
        /// <returns>A new <see cref="Forge"/> instance.</returns>
        public static Forge ForCountry(Country country, ICountryDataProviderRegistry registry)
        {
            return new Forge(country, registry);
        }

        /// <summary>Sets the seed used for deterministic generation.</summary>
        /// <param name="seed">Any 32-bit integer; all values are valid.</param>
        /// <returns>This instance, for chaining.</returns>
        public Forge WithSeed(int seed)
        {
            _seed = seed;
            return this;
        }

        /// <summary>Registers a custom generator, replacing any existing generator for its output type.</summary>
        /// <typeparam name="TGenerator">Generator implementation with a parameterless constructor or a constructor taking an <see cref="IGenerationContext"/>.</typeparam>
        /// <returns>This instance, for chaining.</returns>
        /// <remarks>
        /// The output type is discovered from the <see cref="IDataGenerator{T}"/> interface the generator implements.
        /// Custom generators are invoked by reflection; prefer built-in generators for very large volumes.
        /// </remarks>
        public Forge Register<TGenerator>()
            where TGenerator : class
        {
            var generatorType = typeof(TGenerator);
            var outputType = ResolveOutputType(generatorType);
            var generateMethod = typeof(IDataGenerator<>).MakeGenericType(outputType).GetMethod("Generate");
            var contextConstructor = generatorType.GetConstructor(new[] { typeof(IGenerationContext) });
            var defaultConstructor = generatorType.GetConstructor(Type.EmptyTypes);

            if (contextConstructor == null && defaultConstructor == null)
            {
                throw new InvalidOperationException(
                    "Type '" + generatorType.FullName + "' must declare either a public parameterless constructor or a public constructor taking IGenerationContext.");
            }

            Func<IGenerationContext, object> factory;
            if (contextConstructor != null)
            {
                factory = context => generateMethod.Invoke(contextConstructor.Invoke(new object[] { context }), null);
            }
            else
            {
                factory = context => generateMethod.Invoke(defaultConstructor.Invoke(null), null);
            }

            _factories[outputType] = factory;
            return this;
        }

        /// <summary>Generates a single entity of type <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">Entity type, previously registered.</typeparam>
        /// <returns>A newly generated entity.</returns>
        public T Create<T>()
            where T : class
        {
            return Resolve<T>()(CreateContext());
        }

        /// <summary>Generates a collection of entities of type <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">Entity type, previously registered.</typeparam>
        /// <param name="count">Number of entities to generate. Must be non-negative.</param>
        /// <returns>A read-only collection containing exactly <paramref name="count"/> entities.</returns>
        public IReadOnlyCollection<T> Create<T>(int count)
            where T : class
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "The count must not be negative.");
            }

            var factory = Resolve<T>();
            var context = CreateContext();
            var items = new List<T>(count);
            for (var i = 0; i < count; i++)
            {
                items.Add(factory(context));
            }

            return new ReadOnlyCollection<T>(items);
        }

        private static void RegisterBuiltInGenerators(ConcurrentDictionary<Type, Func<IGenerationContext, object>> factories)
        {
            factories[typeof(Person)] = context => new PersonGenerator(context).Generate();
            factories[typeof(Address)] = context => new AddressGenerator(context).Generate();
            factories[typeof(Company)] = context => new CompanyGenerator(context).Generate();
            factories[typeof(Customer)] = context => new CustomerGenerator(context).Generate();
            factories[typeof(Contract)] = context => new ContractGenerator(context).Generate();
            factories[typeof(Product)] = context => new ProductGenerator(context).Generate();
        }

        private static Type ResolveOutputType(Type generatorType)
        {
            Type outputType = null;
            var interfaces = generatorType.GetInterfaces();
            for (var i = 0; i < interfaces.Length; i++)
            {
                var candidate = interfaces[i];
                if (!candidate.IsGenericType || candidate.GetGenericTypeDefinition() != typeof(IDataGenerator<>))
                {
                    continue;
                }

                var argument = candidate.GetGenericArguments()[0];
                if (outputType != null && outputType != argument)
                {
                    throw new InvalidOperationException(
                        "Generator '" + generatorType.FullName + "' implements IDataGenerator<T> for more than one type.");
                }

                outputType = argument;
            }

            if (outputType == null)
            {
                throw new InvalidOperationException(
                    "Type '" + generatorType.FullName + "' does not implement IDataGenerator<T>.");
            }

            return outputType;
        }

        private static int NextNonDeterministicSeed()
        {
            return BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);
        }

        private GenerationContext CreateContext()
        {
            var boxed = _seed;
            var seed = boxed is int value ? value : NextNonDeterministicSeed();
            return new GenerationContext(Country, Provider, seed);
        }

        private Func<IGenerationContext, T> Resolve<T>()
            where T : class
        {
            if (_factories.TryGetValue(typeof(T), out var factory))
            {
                // ponytail: delegate types are invariant; wrap instead of casting the Func<> itself.
                return context => (T)factory(context);
            }

            throw new InvalidOperationException(
                "No generator is registered for type '" + typeof(T).FullName + "'. Use Register<TGenerator>() to add one.");
        }
    }
}
