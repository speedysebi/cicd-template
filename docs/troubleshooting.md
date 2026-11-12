# Troubleshooting

Common failures and what they mean.

## Calling the workflows

**`error parsing called workflow ... workflow was not found`**
- Check the path: `speedysebi/cicd-template/.github/workflows/<file>.yml@<ref>`.
- Check the ref exists: `git ls-remote --tags https://github.com/speedysebi/cicd-template`.
- If this repo is private: the caller must be private too, and *Settings →
  Actions → General → Access* here must allow "repositories owned by the user".
  A public caller cannot use a private reusable workflow at all.

**`The nested job 'x' is requesting 'contents: write', but is only allowed 'contents: read'`**
The caller job didn't grant the permission. Add it on the calling job:
```yaml
release:
  permissions:
    contents: write
  uses: .../release.yml@v1
```

**`Invalid input, x is not defined in the referenced workflow`**
Typo in an input name, or the input was added in a newer version than the
ref you pinned. Compare with the workflow's doc page.

**Matrix inputs fail with `fromJSON` errors**
List inputs are JSON strings: `os: '["ubuntu-latest"]'`, not
`os: [ubuntu-latest]`. Use single quotes outside, double quotes inside.

## Node

**`npm ci` fails: "requires an existing package-lock.json"**
Commit your lockfile. For bun/pnpm/yarn, commit `bun.lock` / `pnpm-lock.yaml`
/ `yarn.lock`.

**Lockfile is out of date / frozen-lockfile fails**
Run the install locally and commit the updated lockfile.

**A script didn't run**
Only scripts listed in `scripts` *and* defined in `package.json` run; the log
says `Skipping 'x'` for missing ones.

## Python

**`ModuleNotFoundError` in tests**
The package isn't installed. Either make `pyproject.toml` installable (it
needs a `[project]` or `[build-system]` table) or add a `conftest.py` at the
project root so pytest adds it to `sys.path`.

**`pytest: command not found` in uv/poetry projects**
Add pytest to your dev dependencies; the workflow only auto-installs it for
plain pip projects.

## Java

**`./gradlew: Permission denied`**
The workflow `chmod`s it, but commit it executable anyway:
`git update-index --chmod=+x gradlew`.

**Tests fail only in CI**
Download the `test-reports-*` artifact from the run page — the HTML report
shows the full failure.

## .NET

**`NETSDK1045: The current .NET SDK does not support targeting .NET X`**
Add that SDK to `dotnet-version`.

## Docker

**`denied: installation not allowed to Create organization package`** or
**`permission_denied: write_package`**
The calling job needs `packages: write`.

## Release

**Nothing was released**
Expected when the tag for the current version already exists — bump the
version. The run shows a notice saying so.

**A `on: push: tags` workflow didn't run after a release**
Tags created with `GITHUB_TOKEN` don't trigger workflows. Chain the job on
`needs.release.outputs.released == 'true'` in the same pipeline instead.

## Debugging any workflow

- Re-run with *Re-run jobs → Enable debug logging*.
- Run workflows locally with [`act`](https://github.com/nektos/act) (needs Docker).
- Validate syntax locally with [`actionlint`](https://github.com/rhysd/actionlint).
