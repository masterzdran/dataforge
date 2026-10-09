# DataForge

A lightweight, extensible, enterprise-ready synthetic test data generation library for .NET.

DataForge creates **deterministic, country-specific, clearly synthetic** data — no real personal
information is ever used, no network or file access happens at runtime, and the same seed always
produces the same dataset. It targets **.NET Standard 2.0** and is thread-safe and DI-friendly.

```csharp
using DataForge;

var customer = Forge.ForCountry(Country.PT).WithSeed(1234).Create<Customer>();

var customers = Forge.ForCountry(Country.BR).WithSeed(42).Create<Customer>(1000);
```

## Why DataForge?

- **Deterministic** — the same seed + country + entity type always yields the same data. Dates are
  anchored to a fixed epoch (2025-01-01 UTC), so results never depend on the wall clock and are
  stable across .NET runtime versions.
- **Country-aware** — built-in providers for Portugal, Spain, United Kingdom, France, Germany,
  United States and Brazil: names, phone formats, postal codes and country-validated tax
  identifiers (NIF/NIPC, DNI/CIF, CPF/CNPJ, SSN-like, EIN, SIREN, ...).
- **No real PII** — names are curated common names, e-mails use RFC 2606 reserved domains
  (`example.com`, `example.net`, `example.org`), and US SSN-like numbers use the never-issued
  `9xx` area.
- **Extensible** — implement `IDataGenerator<T>` or `ICountryDataProvider` and plug into the
  fluent API; optional provider capabilities (e.g. `IPersonTaxIdentifierProvider`) can grow
  without breaking changes.
- **Enterprise-ready** — strong-named, XML-documented, StyleCop-clean,
  SourceLink-enabled, zero compiler warnings, and a CI pipeline in this repository.

## Getting started

1. Reference the packages from your project (or add project references to `src/`):
   - `DataForge` — the fluent entry point.
   - `DataForge.Abstractions` — entities, interfaces, enums (reference this in extension libraries).
   - `DataForge.CountryProviders` — the built-in country data providers.
   - `DataForge.Generators` — the built-in entity generators.
   - `DataForge.Extensions` — optional extension methods (e.g. `WithSeed(string)`).

2. Build a `Forge` for a country and generate:

```csharp
// Single entity
var person = Forge.ForCountry(Country.PT).Create<Person>();

// Deterministic batch
var contracts = Forge.ForCountry(Country.UK).WithSeed(2024).Create<Contract>(500);

// Text seed via the extensions package
var products = Forge.ForCountry(Country.DE).WithSeed("demo-fixture").Create<Product>(100);
```

3. Use the entities in your tests, demos or development databases.

> **Note on the entry point name.** The original specification used `DataForge.ForCountry(...)`,
> but C# forbids a type and a namespace from sharing a name, so the facade is the static class
> `Forge` in the `DataForge` namespace: `Forge.ForCountry(Country.PT)`. Everything else in the
> public API matches the specification.

## Built-in entities

| Entity | Key generated fields |
| --- | --- |
| `Person` | First/last/full name, e-mail, phone, date of birth, gender |
| `Address` | Street, number, postal code, city, country |
| `Company` | Name, country tax identifier, e-mail, phone, address |
| `Customer` | GUID id, customer number, name, e-mail, phone, billing address |
| `Contract` | Contract number, start/end dates, customer, status |
| `Product` | SKU, name, description, price |

See [docs/api.md](docs/api.md) for the full public API surface.

## Quick API reference

```csharp
Forge.ForCountry(Country country)                                          // entry point
Forge.ForCountry(Country country, ICountryDataProviderRegistry registry)   // custom providers
forge.WithSeed(int seed)                                                   // deterministic
forge.WithSeed(string seed)                                                // extension: FNV-1a hash
forge.Register<TGenerator>()                                               // custom generator
forge.Create<T>()                                                          // single entity
forge.Create<T>(int count)                                                 // IReadOnlyCollection<T>
```

## Thread safety

`Forge` instances are safe to share across threads: the generator registry is a
`ConcurrentDictionary`, the seed is read/written atomically, and every generation call receives a
fresh, private context with its own random source. Configure the instance (seed, registrations)
before generating from multiple threads.

## Documentation

- [Architecture overview](docs/architecture.md)
- [Public API reference](docs/api.md)
- [Extending DataForge](docs/extending.md) — custom generators, providers and capabilities
- [Security considerations](docs/security.md) — threat model, misuse, validation, determinism
- [Contributing](CONTRIBUTING.md)

## Building and testing

```bash
dotnet build DataForge.slnx
dotnet test DataForge.slnx
dotnet pack DataForge.slnx -c Release
```

Packages land in `artifacts/packages/`.

## License

MIT — see [LICENSE](LICENSE).