# Public API reference

All public types are in the `DataForge` namespace. XML documentation is embedded in every
assembly, so IntelliSense shows the full contract; this page is a compact map.

## Entry point: `Forge`

```csharp
// Static entry points
public static Forge ForCountry(Country country)
public static Forge ForCountry(Country country, ICountryDataProviderRegistry registry)

// Instance members
public Country Country { get; }
public ICountryDataProvider Provider { get; }
public Forge WithSeed(int seed)
public Forge Register<TGenerator>() where TGenerator : class
public T Create<T>() where T : class
public IReadOnlyCollection<T> Create<T>(int count) where T : class
```

- `Create<T>(count)` throws `ArgumentOutOfRangeException` for a negative `count`.
- `Create<T>()` throws `InvalidOperationException` when no generator is registered for `T`.
- `Register<TGenerator>()` discovers the output type from the `IDataGenerator<T>` interface and
  throws `InvalidOperationException` for types that implement it for more than one output type or
  not at all, or that lack a public parameterless constructor or a public
  `IGenerationContext`-taking constructor.

## Extensions (`DataForge.Extensions`)

```csharp
public static Forge WithSeed(this Forge forge, string seed)   // FNV-1a hash → stable int seed
```

Throws `ArgumentNullException` for a null `forge`, `ArgumentException` for null/empty seed text.

## Enums

| Type | Values |
| --- | --- |
| `Country` | `PT, ES, UK, FR, DE, US, BR` |
| `Gender` | `Female, Male` |
| `ContractStatus` | `Draft, Active, Suspended, Completed, Cancelled` |

## Entities (abstractions)

All models have public settable properties: `Person`, `Address`, `Company`, `Customer`,
`Contract`, `Product`. See the entity table in the [README](../README.md) for the generated
fields.

## Interfaces

| Interface | Purpose |
| --- | --- |
| `IGenerationContext` | Ambient state: `Country`, `Provider`, plus `Next`, `NextDouble`, `NextBool`, `Pick<T>`, `Digits`, `NextBytes` |
| `ICountryDataProvider` | Primitive country data: names, city/street/postal code/phone/company name, `GetTaxIdentifier` |
| `ICountryDataProviderRegistry` | Resolve a provider for a `Country`; register providers |
| `IDataGenerator<T>` | `T Generate()`; the unit custom generators implement |
| `IPersonTaxIdentifierProvider` | Optional capability: `GetPersonTaxIdentifier(context)` |

`ICountryDataProvider` and `IGenerationContext` methods throw on null arguments; `Next` and
`Digts`-style helpers throw `ArgumentOutOfRangeException` on inverted ranges and negative counts.
`Pick<T>` requires a non-null, non-empty list.

## Behavior contracts

- **Same seed → same data.** Independent of runtime, culture and wall clock.
- **No real data.** No scraping, datasets, network or file access at runtime.
- **Thread-safe for reading.** Configure a `Forge`, then share it across threads.
- **Null-safe inputs.** Public methods validate arguments up front (no partial generation).