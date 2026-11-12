# cicd-template

Reusable GitHub Actions workflows for every project: CI for **Node/Bun,
Python, Java, .NET and Go**, plus **Docker** images, **version-bump releases**
and **workflow linting**. Each project's pipeline is a ~10-line file that
calls these, so a fix here reaches every project.

```yaml
# .github/workflows/ci.yml in any project
name: CI
on:
  push:
    branches: [main]
  pull_request:
permissions:
  contents: read
jobs:
  ci:
    uses: speedysebi/cicd-template/.github/workflows/node-ci.yml@v1
```

## Workflows

| Workflow | For | Does |
|---|---|---|
| [`node-ci.yml`](docs/workflows/node-ci.md) | JS/TS with npm, pnpm, yarn, bun | install → lint → typecheck → test → build (whichever scripts exist) |
| [`python-ci.yml`](docs/workflows/python-ci.md) | pip, uv, poetry | install → lint → typecheck → pytest |
| [`java-ci.yml`](docs/workflows/java-ci.md) | Gradle, Maven | build + test, uploads reports on failure |
| [`dotnet-ci.yml`](docs/workflows/dotnet-ci.md) | .NET | restore → format check → build → test |
| [`go-ci.yml`](docs/workflows/go-ci.md) | Go modules | vet → build → test → golangci-lint |
| [`docker.yml`](docs/workflows/docker.md) | Any Dockerfile | build, tag, push to GHCR or another registry |
| [`release.yml`](docs/workflows/release.md) | Any project with a version | tag + GitHub Release when the version changes |
| [`workflow-lint.yml`](docs/workflows/workflow-lint.md) | Any repo | actionlint + zizmor security audit |

Every workflow is tested on each change by [`self-test.yml`](.github/workflows/self-test.yml)
against small fixture projects in [`tests/fixtures/`](tests/fixtures).

## Quick start

1. Copy the example for your language from [`examples/`](examples) into
   your project's `.github/workflows/`.
2. Push a branch and open a PR.
3. Require the checks on `main`.

Full walkthrough: **[docs/getting-started.md](docs/getting-started.md)**.

## A full pipeline

[`examples/full-pipeline.yml`](examples/full-pipeline.yml):

```
PR / push ──► CI ──► (main only) release ──► docker image ──► deploy
                     only if version bumped                   manual approval
```

## Docs

| | |
|---|---|
| [Strategies](docs/strategies.md) | Research: which CI/CD strategies work for small projects, and why this template is built the way it is |
| [Getting started](docs/getting-started.md) | Add CI to a project in five minutes |
| [Branching and releases](docs/branching-and-releases.md) | Trunk-based flow, protecting `main`, cutting releases |
| [Versioning](docs/versioning.md) | `@v1` vs `@v1.2.0` vs SHA; releasing this template |
| [Secrets and environments](docs/secrets-and-environments.md) | Where keys go and how to pass them |
| [Security](docs/security.md) | Hardening built in, and rules for callers |
| [Cost and speed](docs/cost-and-speed.md) | Staying fast and inside free minutes |
| [Troubleshooting](docs/troubleshooting.md) | Common errors and fixes |

## Design principles

- **Zero-config defaults.** Calling a workflow with no inputs does the right
  thing for a conventional project.
- **Run the project's own commands.** CI passes exactly when `npm test`,
  `pytest`, `./gradlew build`... pass locally.
- **Secure by default.** SHA-pinned actions, read-only tokens, no credential
  persistence, no input interpolation in scripts.
- **Versioned.** Callers pin `@v1`; breaking changes go to `@v2`.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md). Changes are listed in
[CHANGELOG.md](CHANGELOG.md).

## License

[MIT](LICENSE)
