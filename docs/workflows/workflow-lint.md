# workflow-lint.yml

Lints the GitHub Actions workflows in the calling repo.

| Tool | Checks |
|---|---|
| [actionlint](https://github.com/rhysd/actionlint) | YAML/syntax errors, unknown inputs, expression types, shellcheck on `run:` scripts |
| [zizmor](https://docs.zizmor.sh) | Security: template injection, overly broad permissions, unpinned actions, persisted credentials, dangerous triggers |

Findings appear as annotations on the run and on the PR's *Files changed* tab.

## Usage

```yaml
on:
  pull_request:
    paths: [".github/**"]

jobs:
  lint:
    uses: speedysebi/cicd-template/.github/workflows/workflow-lint.yml@v1
```

## Inputs

| Input | Default | Description |
|---|---|---|
| `actionlint-version` | `1.7.12` | actionlint release |
| `extra-paths` | | Extra workflow files to lint besides `.github/workflows`, space-separated (actionlint also accepts globs) |
| `zizmor` | `true` | Run the zizmor audit |
| `zizmor-min-severity` | `low` | `unknown`, `informational`, `low`, `medium`, `high` |

## Required: zizmor config for `@v1` references

zizmor requires every `uses:` to be pinned to a full commit SHA, including
calls to this template. If your workflows use `...@v1`, copy
[`examples/zizmor.yml`](../../examples/zizmor.yml) to `.github/zizmor.yml`
in your project. It allows tag pins for `speedysebi/*` only, and keeps
SHA pins required for every third-party action.

## Notes

- Results are not uploaded to the Security tab, which needs GitHub Advanced
  Security (not available for private repos on free plans).
- Silence a specific zizmor finding with a `# zizmor: ignore[rule]` comment on
  the line, or a `zizmor.yml` config in the repo.
