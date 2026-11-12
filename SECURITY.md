# Security policy

These workflows run in other repositories' CI with their tokens and secrets,
so a flaw here affects every project that uses them.

## Reporting a vulnerability

Please **don't** open a public issue. Use GitHub's private reporting:
*Security → Report a vulnerability* on this repository.

Include which workflow and version (`@v1`, tag or SHA) is affected and how it
can be triggered.

## Supported versions

| Version | Supported |
|---|---|
| `v1` (latest `v1.x.y`) | Yes |

## Hardening

What the workflows do to limit damage, and what callers should do, is
documented in [docs/security.md](docs/security.md).
