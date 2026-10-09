# Contributing to DataForger

Thanks for helping. Keep the bar: **CI green, zero warnings, StyleCop clean.**

## Development environment

- .NET SDK 8+ (the repo targets `netstandard2.0` and tests run on `net8.0` + `net10.0`).
- Any OS — everything is cross-platform.

## Getting started

```bash
dotnet build DataForger.slnx          # must succeed with 0 errors and 0 warnings
dotnet test DataForger.slnx           # unit + integration tests, all TFMs
```

## Style and conventions

- Microsoft C# conventions: PascalCase public members, `I`-prefix interfaces, `_camelCase`
  private fields, `Async` suffix where applicable.
- StyleCop Analyzers run with warnings-as-errors. Notable disabled rules are documented in
  `.editorconfig` — do not weaken rules to silence a warning; fix the code.
- All public/protected members carry XML docs (SA1600). CI treats warnings as errors.
- Follow the existing layering: providers return primitives, generators compose entities,
  `Forge` is the only facade. No static mutable state, no hidden side effects.
- Mark deliberate simplifications with a `// ponytail` comment so reviewers know they are
  intentional (e.g. nullable-off, volatility tricks).

## Change flow

1. Fork, branch, commit with clear messages (Conventional Commits preferred:
   `feat:`, `fix:`, `docs:`, `test:`, `chore:`).
2. Add tests that fail without your change and pass with it. New behavior needs both unit and,
   where relevant, integration tests.
3. Run the full build + tests locally.
4. Open a pull request; the GitHub Actions workflow runs the same gates.

## Adding a country

1. Add the value to `Country` in `src/DataForger.Abstractions/Country.cs`.
2. Create a provider in `src/DataForger.CountryProviders/` deriving from `CountryDataProvider`
   (constructor-injected name arrays + formats). Implement `GetTaxIdentifier`; implement
   `IPersonTaxIdentifierProvider` when the country has a person-level id.
3. Register it in `CountryDataProviderRegistry`.
4. Add provider tests, tax-identifier validation tests (with independent reference
   implementations in the test project), and an end-to-end determinism case in the integration
   project.

## Documentation

User-facing changes should be reflected in the README and `docs/`. Keep examples runnable and
check any API name you document against the actual surface — the code is the contract.

## Security

See [docs/security.md](docs/security.md). Report vulnerabilities privately to the maintainers
before opening a public issue.