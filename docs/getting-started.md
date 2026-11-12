# Getting started

Add CI to a project in about five minutes.

## 1. Make sure the project can call this repo

| Your project is | This template repo must be | Extra step |
|---|---|---|
| Public | Public | none |
| Private (owned by you) | Public **or** private | If private: *Settings → Actions → General → Access* → "Accessible from repositories owned by the user" |

A public repo cannot call reusable workflows that live in a private repo.

## 2. Pick the example for your language

| Language | Example | Reusable workflow |
|---|---|---|
| JavaScript / TypeScript (npm, pnpm, yarn) | [`examples/node.yml`](../examples/node.yml) | `node-ci.yml` |
| Bun | [`examples/bun.yml`](../examples/bun.yml) | `node-ci.yml` |
| Python (pip, uv, poetry) | [`examples/python.yml`](../examples/python.yml) | `python-ci.yml` |
| Java / Kotlin (Gradle, Maven) | [`examples/java.yml`](../examples/java.yml) | `java-ci.yml` |
| C# / .NET | [`examples/dotnet.yml`](../examples/dotnet.yml) | `dotnet-ci.yml` |
| Go | [`examples/go.yml`](../examples/go.yml) | `go-ci.yml` |

Copy it into your project as `.github/workflows/ci.yml`.

## 3. Make the project's own commands work

The workflows run your project's normal commands, so CI passes exactly when
these pass locally:

- **Node/Bun**: the `lint`, `typecheck`, `test`, `build` scripts in
  `package.json` (any you don't have are skipped). Commit your lockfile.
- **Python**: `pytest`. Put dev tools (pytest, ruff, mypy) in
  `requirements-dev.txt`, a `[dev]` extra, or your uv/poetry dev group.
- **Java**: `./gradlew build` or `mvn verify`. Commit the wrapper
  (`gradlew`, `gradle/wrapper/*`) if you use one.
- **.NET**: `dotnet build` and `dotnet test`.
- **Go**: `go vet`, `go build`, `go test ./...`.

## 4. Push and check

```bash
git checkout -b ci
git add .github/workflows/ci.yml
git commit -m "Add CI"
git push -u origin ci
gh pr create --fill
gh pr checks --watch
```

## 5. Require CI before merging

*Settings → Rules → Rulesets → New branch ruleset* on `main`:

- Require a pull request before merging
- Require status checks to pass → add the `ci / ...` checks

## 6. Add releases and deploys (optional)

Start from [`examples/full-pipeline.yml`](../examples/full-pipeline.yml). It
adds:

1. **release** — creates a tag + GitHub Release when you bump the version on
   `main` ([release.md](workflows/release.md))
2. **image** — builds and pushes a Docker image to GHCR ([docker.md](workflows/docker.md))
3. **deploy** — a job gated by a `production` environment with manual approval

## Next

- [Versioning](versioning.md) — which `@ref` to use and how updates reach you
- [Secrets and environments](secrets-and-environments.md)
- [Troubleshooting](troubleshooting.md)
