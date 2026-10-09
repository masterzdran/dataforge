# Extending DataForger

Three extension points cover almost everything:

1. **Custom entity generator** — produce data for a type DataForger does not ship.
2. **Custom provider / new country** — add a country or change how a country's primitives look.
3. **Provider capabilities** — expose optional country-specific features without breaking changes.

## 1. Custom generator

Implement `IDataGenerator<T>` and register it. Use the `IGenerationContext` given to your
constructor — it provides the country, the provider and a deterministic random source.

```csharp
public sealed class PassportGenerator : IDataGenerator<Passport>
{
    private readonly IGenerationContext _context;

    public PassportGenerator(IGenerationContext context)
    {
        _context = context;
    }

    public Passport Generate()
    {
        // _context.Next(min, max), _context.Digits(n), _context.Pick(list), ...
        return new Passport
        {
            Number = "P" + _context.Digits(8),
            Country = _context.Country,
        };
    }
}

var forge = Forge.ForCountry(Country.PT).Register<PassportGenerator>();
Passport p = forge.Create<Passport>();
```

### Constructor rules

`Register<TGenerator>()` accepts a generator that has **either**:

- a public parameterless constructor, **or**
- a public constructor taking exactly one `IGenerationContext`.

Both are invoked per generated entity. Types implementing `IDataGenerator<T>` for more than one
`T`, or none, are rejected at registration.

> Performance note: custom generators are invoked through reflection. For very large volumes
> prefer registering the built-in pattern (`IDataGenerator<T>` implementations), which route
> through direct delegates.

## 2. Custom provider / new country

Extend `CountryDataProvider` and supply name arrays + format templates. `#` in a template
expands to a random decimal digit.

```csharp
public sealed class ItalyDataProvider : CountryDataProvider
{
    public ItalyDataProvider()
        : base(
            Country.IT,                                  // requires adding Country.IT
            firstNames, lastNames, cities,               // string[]
            streetPrefixes,
            "Via {0}",
            new[] { "S.p.A.", "S.r.l." },                // company suffixes
            "#####",                                     // postal code format
            "+39 3## ### ####")                          // phone format
    {
    }

    public override string GetTaxIdentifier(IGenerationContext context)
    {
        var value = Expand(context, "#######");          // or context.Digits(7)
        return "IT" + value + CheckDigits.Luhn(value);   // IT + 11-digit VAT (Luhn)
    }
}
```

Then register it:

```csharp
var registry = new CountryDataProviderRegistry();
registry.Register(new ItalyDataProvider());

var forge = Forge.ForCountry(Country.IT, registry);
```

Check-digit helpers in `CheckDigits` (internal) are shared by the included providers; copy the
few lines you need into your own assembly if you want them.

## 3. Capability interfaces

Optional provider features use marker interfaces so the core provider contract never changes. The
shipped example is `IPersonTaxIdentifierProvider`:

```csharp
if (context.Provider is IPersonTaxIdentifierProvider provider)
{
    var nif = provider.GetPersonTaxIdentifier(context);
}
```

Your own capabilities follow the same pattern: define an interface in your library, implement it
on your provider, and `is`-test when you need it. Future core identifiers (passport, IBAN, VAT,
...) can be added the same way without breaking existing providers.

## Replacements and ordering

`Register<TGenerator>()` **replaces** any existing generator for the same output type. The
built-in generators register at construction; registering your own generator for a built-in type
(e.g. `Customer`) overrides it for that `Forge` instance only.

## Rules of thumb

- Keep `ICountryDataProvider` implementations **primitive-only** — they return strings, never
  entities.
- Keep generators **composable** — build entities from `IGenerationContext`, never from globals.
- Use only `IGenerationContext` randomness inside generators; context methods always exist.
  The context is not shared across threads — never cache it.
- Never call the wall clock; use the context-provided random values if you need "now"-relative
  data.