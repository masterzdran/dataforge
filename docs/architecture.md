# Architecture

## Overview

DataForge is a small, layered library built on SOLID principles. The core idea: a **country data
provider** supplies primitive, country-specific values and formats; **entity generators** compose
those primitives into business entities; and a **fluent facade** (`Forge`) wires both together
with deterministic randomness and a plug-in registry.

```
┌──────────────────────────────┐
│ Forge (facade)               │   entry point, seed, registry, Create<T>
├──────────────────────────────┤
│ Generators (composition)     │   PersonGenerator, CustomerGenerator, ...
├──────────────────────────────┤
│ Country data providers       │   PortugalDataProvider, SpainDataProvider, ...
├──────────────────────────────┤
│ Abstractions + Deterministic │   models, interfaces, GenerationContext,
│ Random                       │   DeterministicRandom, CheckDigits
└──────────────────────────────┘
```

## Project layout

```
src/
  DataForge.Abstractions/    models (Person, Address, Company, Customer, Contract, Product),
                             Country/Gender/ContractStatus enums, IGenerationContext,
                             ICountryDataProvider, ICountryDataProviderRegistry,
                             IDataGenerator<T>, IPersonTaxIdentifierProvider
  DataForge.CountryProviders/ CountryDataProvider base class, CheckDigits helpers,
                             registry, one provider per country (PT, ES, UK, FR, DE, US, BR)
  DataForge.Generators/      one generator per entity + SyntheticText (emails, SKUs, ...)
                             and GenerationDates (fixed date anchors)
  DataForge/                 Forge facade, GenerationContext, DeterministicRandom
  DataForge.Extensions/      optional extension methods (WithSeed(string))
tests/
  DataForge.UnitTests/        unit tests (xUnit + FluentAssertions)
  DataForge.IntegrationTests/ end-to-end tests: 100k-volume, cross-country determinism,
                             custom generator + provider capability flows
```

All public types live in the `DataForge` namespace regardless of assembly, keeping the public
surface flat and easy to consume.

## Layers and responsibilities

### Abstractions (`DataForge.Abstractions`)

- Domain models are plain classes with settable properties so custom generators stay simple.
- `ICountryDataProvider` returns **primitives only** (names, postal codes, phone numbers, tax
  identifiers). Providers never know about business entities — that separation is what keeps
  adding a new country a purely additive change.
- Optional capabilities (e.g. `IPersonTaxIdentifierProvider`) extend providers without touching
  the base interface, following the Open/Closed principle.
- `IGenerationContext` carries the selected country, provider and a deterministic random source
  to every generator.

### Country providers (`DataForge.CountryProviders`)

- `CountryDataProvider` is a base class driven by constructor-injected name arrays and format
  templates (`#` expands to a random digit), so a new country is mostly data + a few rules.
- Tax identifiers with real-world check digits are computed in each provider using the pure
  helpers in `CheckDigits` (Portuguese mod-11, Spanish DNI letter and CIF control, Brazilian
  CPF/CNPJ mod-11, Luhn for French SIREN).

| Country | Person tax id (`IPersonTaxIdentifierProvider`) | Company tax id (`GetTaxIdentifier`) |
| --- | --- | --- |
| PT | NIF (9 digits, mod-11) | NIPC (9 digits, mod-11) |
| ES | DNI (8 digits + letter) | CIF (letter + 7 digits + control) |
| BR | CPF (11 digits, mod-11 ×2) | CNPJ (14 digits, mod-11 ×2) |
| US | SSN-like (`9xx` area only) | EIN-like (`NN-NNNNNNN`) |
| FR | — | SIREN (9 digits, Luhn) |
| DE | — | USt-IdNr-like (`DE` + 9 digits) |
| UK | — | Companies House-like (`SC`, `NI` or `OC` + 6 digits) |

### Generators (`DataForge.Generators`)

Each generator takes an `IGenerationContext` in its constructor and implements
`IDataGenerator<T>` with a single `Generate()`. They compose provider primitives into entities
and own the cross-cutting rules:

- e-mails always use RFC 2606 reserved domains (`example.com`, `example.net`, `example.org`);
- customer GUIDs are derived from 16 random bytes per call, so the sequence is fully
  deterministic given the seed;
- dates come from `GenerationDates`, anchored to a fixed epoch (`2025-01-01 UTC`) — never the
  wall clock.

### Core (`DataForge`)

- `Forge` is the facade: `ForCountry(...)` entry points, `WithSeed(...)`, `Register<TGenerator>()`
  and `Create<T>(...)`. Registrations live in a `ConcurrentDictionary<Type, Func<...>>`; the seed
  is a `volatile object` holding a boxed int, so reads/writes are atomic without a lock.
- `GenerationContext` implements `IGenerationContext` with argument guards and forwards random
  calls to `DeterministicRandom`.
- `DeterministicRandom` is a **SplitMix64-based** pseudo-random source. It is deliberately *not*
  `System.Random`, whose algorithm changed between .NET Framework and .NET Core (and again in
  .NET 6+), which would break cross-runtime determinism.

## Determinism model

A full `Create<T>` call starts from the configured seed: the un-seeded case derives a one-time
seed from `Guid.NewGuid()`. Every context gets a fresh `DeterministicRandom(seed)`, so:

- `Forge.ForCountry(Country.PT).WithSeed(123).Create<Customer>()` returns the same customer
  forever, on every supported runtime;
- `Create<T>(n)` produces the same ordered collection for the same seed;
- wall-clock time, thread scheduling and prior calls never influence results.

## Thread safety

- One `GenerationContext` per generation call; contexts are never shared.
- The registry is a `ConcurrentDictionary`; concurrent registration of the same type is
  last-writer-wins with a single atomic replace.
- The seed write is a single volatile store; generation snapshots the seed once per call.
- Net effect: a `Forge` configured once can be used from many threads immediately.

## Performance notes

- No reflection in the hot path: built-in generators are registered as direct delegates.
- `Register<TGenerator>()` resolves the output type and constructors **once** at registration
  time; per-entity invocation goes through `MethodInfo.Invoke`, which is fine for most workloads
  but slower than built-ins — prefer built-ins for very large volumes, or register a delegate
  through the internal surface.
- `Create<T>(count)` pre-sizes the backing `List<T>` and returns a read-only wrapper.

## Design decisions worth knowing

- **`Forge`, not `DataForge`:** the specification's `DataForge.ForCountry(...)` cannot compile
  because C# forbids a type and its containing namespace from sharing a name. The facade is the
  static class `Forge` in the `DataForge` namespace.
- **No external dependencies at runtime.** The only dependencies are analyzer/SourceLink
  packages, all `PrivateAssets="all"`.
- **Primitive-only providers** keep the providers isolated from entities (separation of
  concerns) and make custom countries trivial.
- **Settable model properties** (public `get; set;`) trade encapsulation for extensibility;
  models are data carriers, not behavior holders.