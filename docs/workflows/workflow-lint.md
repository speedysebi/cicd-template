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
| `zizmor` | `true` | Run the zizmor audit |
| `zizmor-min-severity` | `low` | `unknown`, `informational`, `low`, `medium`, `high` |

## Notes

- Results are not uploaded to the Security tab, which needs GitHub Advanced
  Security (not available for private repos on free plans).
- Silence a specific zizmor finding with a `# zizmor: ignore[rule]` comment on
  the line, or a `zizmor.yml` config in the repo.
