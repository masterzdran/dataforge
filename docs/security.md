# Security considerations

DataForger generates synthetic test data. It aims to be safe **by construction**: no real personal
data, no network, no file access, no reflection in the hot path, deterministic and threadsafe. This
document records the threat model, misuse scenarios, the validation guarantees and the design
choices behind them.

## Goals and non-goals

**Goals**

- A consumer can generate large volumes of data with **zero risk of leaking real PII**.
- Deterministic outputs enable reproducible tests and bug reports.
- Library code has no ambient authority: it cannot touch the network, the file system or the
  process environment at runtime.

**Non-goals**

- NOT a cryptography library. `DeterministicRandom` is a fast PRNG (SplitMix64), not a CSPRNG;
  numbers are guessable when the seed is known — that is exactly the determinism contract.
- NOT a privacy-anonymizer. Data is synthetic, not anonymized real data.
- NOT a data validator for production inputs.

## Threat model

| Threat | Mitigation |
| --- | --- |
| Generated data collides with real identities | Curated name pools; e-mails restricted to RFC 2606 reserved domains (`example.com/net/org`); US SSN-like numbers confined to the never-issued `9xx` area code |
| Accidental network/file/process access at runtime | No `System.Net`, `System.IO`, `System.Diagnostics` or environment use anywhere in `src/`; no runtime dependencies |
| Non-determinism across machines/runtimes | Seed-driven SplitMix64 PRNG (not `System.Random`, whose algorithm changed across runtimes); dates anchored to a fixed epoch; invariant-culture formatting where applicable |
| Data races under parallel generation | Fresh `IGenerationContext` per call; `ConcurrentDictionary` registry; atomic volatile seed store |
| Reflection abuse via custom generators | `Register<TGenerator>()` is opt-in and resolves the output type/ctors once; the registered type must implement `IDataGenerator<T>` — already in the caller's trust domain |
| Resource exhaustion from `Create<T>(n)` | `count` is validated non-negative; a caller choosing a huge count is allocating its own memory, not the library's |

## Misuse and abuse scenarios (documented behavior)

- **Using output as real production data.** Not supported. SSN-like, NIF, DNI, CPF etc. are
  structurally valid but **not issued** numbers; treat them as test fixtures only. Do not route
  them through identity-checking services.
- **Using DataForger to probe real check-digit schemes.** The formats are public knowledge and the
  generated numbers are synthetic; any resemblance to an existing number is a coincidence.
- **Guessing/collision guarantees.** Collisions within a seed are possible for short codes
  (postal codes, phone numbers). They are bounded by the format's alphabet size. For keys that
  require uniqueness, derive them from the context (e.g. `Customer.Id` uses 16 random bytes per
  call) or post-process with your own constraint.
- **Relying on `Create<T>()` on an unconfigured type.** Throws `InvalidOperationException` — fail
  fast instead of silently returning nothing.
- **Sharing a `Forge` mid-configuration across threads.** Registration writes are safe
  (`ConcurrentDictionary`), but configure-then-generate is the supported model.
- **Storing synthetic PII in production stores.** Even synthetic data can be mistaken for real
  data downstream; keep it clearly labeled (a `Synthetic` flag or dedicated database).

## Input validation

All public entry points validate arguments up front and throw before any generation:

- `Create<T>(count)` — `ArgumentOutOfRangeException` if `count < 0`.
- `Register<TGenerator>()` — `InvalidOperationException` if the type does not implement
  `IDataGenerator<T>`, implements it for multiple output types, or lacks a usable constructor.
- `WithSeed(string)` (extensions) — `ArgumentNullException` / `ArgumentException`.
- `Forge(country, registry)` — `ArgumentNullException` for a null registry; the registry throws
  `ArgumentOutOfRangeException` for an unknown country.
- `IGenerationContext.Next(min, max)` — `ArgumentOutOfRangeException` on inverted/empty ranges
  (`max <= min`).
- `IGenerationContext.Digits(count)` — `ArgumentOutOfRangeException` for negative counts.
- `IGenerationContext.Pick<T>(items)` — `ArgumentNullException`/empty-list `ArgumentException`.
- `IGenerationContext.NextBytes(buffer)` — `ArgumentNullException`.

No value produced by the library is ever validated as real-world identity data; check digits
prove internal consistency, not real existence.

## Thread safety

- A fresh context per generation call — contexts are never shared or cached by the library.
- `Forge._factories` is a `ConcurrentDictionary<Type, Func<IGenerationContext, object>>`.
- The seed is a `volatile object` (boxed `int`); read-modify-write of seeds is
  last-store-wins, which is fine because seeds are set once before concurrent generation.
- Built-in generators and providers hold no mutable state between calls.

## Determinism risks

Determinism is a feature, but consider the implications:

- **Replay across security boundaries.** If the seed is known, all "random" values are
  predictable. Never seed with a secret or use DataForger for anything that requires
  unpredictability.
- **Culture sensitivity.** String formatting uses the provider implementation; built-ins avoid
  culture-dependent output so the same seed yields the same strings everywhere.
- **Versioning.** The exact sequence for a given seed is tied to this implementation. If you need
  byte-for-byte stability across DataForger versions, pin the version and record it in your
  fixtures. A future release that fixes a name pool or check-digit rule will change output — by
  design.

## Dependency review

- Runtime: **zero third-party dependencies**. The only packages are `PrivateAssets="all"`
  build-time analyzers and SourceLink (`StyleCop.Analyzers`, `Microsoft.SourceLink.GitHub`).
- A dependency scan (GitHub Dependabot, NuGet audit) applies to the tooling packages only and
  cannot affect runtime attack surface.
- The strong-name key (`build/DataForger.snk`) is committed deliberately so anyone can verify
  assemblies signed by CI; it is the public trust anchor for the package identity, not a secret.

## Supply-chain notes for consumers

- Packages are strong-named and SourceLink-enabled: you can step into sources and verify builds.
- `ContinuousIntegrationBuild` is enabled on GitHub Actions so PDBs embed deterministic paths.
- Verify the package's `RepositoryUrl` and the signed assembly before trusting a distribution
  other than the official feed.

## Vulnerability reporting

Report security issues privately (see [CONTRIBUTING.md](../CONTRIBUTING.md)) rather than in a
public issue. Because the runtime surface is intentionally empty and deterministic, most reports
will concern misuse guidance, which this document tracks.