# Branching and releases

The workflow these templates assume, written down so every project works the
same way.

## Branches

- `main` is always releasable.
- All work happens on short branches: `feat/...`, `fix/...`, `ci/...`,
  `docs/...`. Merge within a day or two.
- Open a PR even when working solo. It is where CI results, the diff, and
  the decision to release live together.
- Squash-merge or rebase-merge so `main` reads as one commit per change.

## Protecting `main`

*Settings → Rules → Rulesets → New branch ruleset*, target `main`:

| Rule | Setting |
|---|---|
| Restrict deletions | on |
| Block force pushes | on |
| Require a pull request before merging | on (0 required approvals when solo) |
| Require status checks to pass | the CI job names, e.g. `ci / ubuntu-latest / node 22` |
| Require branches to be up to date | optional; on if merges often conflict |

Add yourself to the bypass list if you occasionally need to push a hotfix
directly.

## Versioning your project

Use [Semantic Versioning](https://semver.org/): `MAJOR.MINOR.PATCH`.

- **PATCH** — bug fixes, no behavior change for users
- **MINOR** — new features, backwards compatible
- **MAJOR** — breaking changes

Before 1.0.0 anything may change; that's fine for early projects.

## Cutting a release

With `release.yml` in your pipeline, a release is just a version bump:

```bash
git checkout -b release/1.4.0
npm version minor --no-git-tag-version   # or edit pyproject.toml / gradle.properties / VERSION
git commit -am "Release 1.4.0"
gh pr create --fill
```

When the PR merges, the pipeline on `main`:

1. runs CI,
2. sees that tag `v1.4.0` doesn't exist and creates it plus a GitHub Release
   with auto-generated notes,
3. runs publish/deploy jobs that depend on `released == 'true'`.

Merges that don't change the version run CI and stop — nothing ships by
accident.

## Hotfixes

Same flow, faster: branch from `main`, fix, bump PATCH, PR, merge. No
separate hotfix branches are needed because `main` is always the latest
release plus whatever is about to be released.

## Better release notes

`--generate-notes` builds notes from merged PR titles. Make them readable by
writing PR titles as user-facing changes, and optionally add
`.github/release.yml` to group them by label:

```yaml
changelog:
  categories:
    - title: Features
      labels: [enhancement]
    - title: Fixes
      labels: [bug]
    - title: Other
      labels: ["*"]
```
