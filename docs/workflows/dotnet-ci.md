# dotnet-ci.yml

CI for **.NET** projects: restore, optional format check, build, test.

## Usage

```yaml
jobs:
  ci:
    uses: speedysebi/cicd-template/.github/workflows/dotnet-ci.yml@v1
    with:
      dotnet-version: 8.0.x
```

## Inputs

| Input | Default | Description |
|---|---|---|
| `dotnet-version` | `8.0.x` and `10.0.x` | SDK version(s); newline-separate several |
| `os` | `'["ubuntu-latest"]'` | JSON array of runners |
| `working-directory` | `.` | Folder to run in |
| `project` | | `.sln`/`.csproj` to build when the folder has more than one |
| `configuration` | `Release` | Build configuration |
| `format-check` | `false` | Run `dotnet format --verify-no-changes` |
| `timeout-minutes` | `20` | Per-job timeout |

## Notes

- Test results are written as `.trx` and uploaded as `test-results-<os>` when
  the job fails.
- Install every SDK whose target framework your tests run on: a `net8.0`
  test project needs the 8.0 runtime, which the 8.0 SDK provides.
