# Changelog

All notable changes to these workflows. Format based on
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/); versions follow
[Semantic Versioning](https://semver.org/) as described in
[docs/versioning.md](docs/versioning.md).

## [Unreleased]

### Added

- `workflow-lint.yml`: `extra-paths` input to lint workflow files outside
  `.github/workflows`.
- `examples/zizmor.yml` for projects that call this template by tag.
- Self-test coverage for pnpm, yarn, uv, Gradle, golangci-lint, release
  version parsing from pyproject.toml/gradle.properties/pom.xml, and lint of
  every example.

### Fixed

- Projects copying the examples failed `workflow-lint.yml`, because zizmor
  requires SHA pins for `@v1` references.

## [1.0.0] - 2026-11-11

### Added

- `node-ci.yml`: npm, pnpm, yarn and bun; runs whichever of
  lint/typecheck/test/build exist; OS and Node version matrix; artifact upload.
- `python-ci.yml`: pip, uv and poetry auto-detection; lint, typecheck and
  pytest steps; passes when no tests are collected.
- `java-ci.yml`: Gradle and Maven auto-detection, wrapper support, test
  reports uploaded on failure.
- `dotnet-ci.yml`: restore, optional format check, build, test with TRX results.
- `go-ci.yml`: vet, build, test with `-race`, optional golangci-lint.
- `docker.yml`: Buildx build with automatic tags, GHA layer cache, optional
  push to GHCR or any registry; image and digest outputs.
- `release.yml`: tag and GitHub Release on version bump, read from
  package.json, pyproject.toml, Cargo.toml, gradle.properties, pom.xml or
  VERSION; `released`/`version`/`tag` outputs; dry-run mode.
- `workflow-lint.yml`: actionlint and zizmor.
- `self-test.yml` with fixture projects for every workflow.
- Dependabot for SHA-pinned actions.
- Examples and documentation.

[Unreleased]: https://github.com/speedysebi/cicd-template/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/speedysebi/cicd-template/releases/tag/v1.0.0
