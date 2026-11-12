# Fixtures

Minimal projects that [`self-test.yml`](../../.github/workflows/self-test.yml)
runs the reusable workflows against. Each one has a single passing test, so a
red self-test means a workflow broke, not the fixture.

| Fixture | Exercised by | Notes |
|---|---|---|
| [`node/`](node) | `node-ci.yml` (npm on Linux + Windows, Node 22 + 24; and bun) | `test` uses `node --test`; `build` writes `dist/` for the artifact upload |
| [`python/`](python) | `python-ci.yml` (3.12, 3.13) | No requirements: checks pytest auto-install |
| [`java-maven/`](java-maven) | `java-ci.yml` (JDK 17, 21) | JUnit 5 via surefire |
| [`dotnet/`](dotnet) | `dotnet-ci.yml` | xUnit, `net8.0` |
| [`go/`](go) | `go-ci.yml` | No dependencies, so no `go.sum`: checks the cache toggle |
| [`docker/`](docker) | `docker.yml` (build only) | Alpine image |
| `node/package.json` version | `release.yml` (dry run) | Expects version `1.0.0` / tag `v1.0.0` |

Keep fixtures tiny: they run on every push.
