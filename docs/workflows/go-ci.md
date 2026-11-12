# go-ci.yml

CI for **Go** modules: `go vet`, `go build`, `go test`, optional golangci-lint.

## Usage

```yaml
jobs:
  ci:
    uses: speedysebi/cicd-template/.github/workflows/go-ci.yml@v1
```

## Inputs

| Input | Default | Description |
|---|---|---|
| `go-version` | *(from go.mod)* | Explicit Go version |
| `os` | `'["ubuntu-latest"]'` | JSON array of runners |
| `working-directory` | `.` | Module folder |
| `test-flags` | `-race` | Flags for `go test`, e.g. `-race -cover` |
| `golangci-lint` | `false` | Run golangci-lint (uses your `.golangci.yml` if present) |
| `timeout-minutes` | `15` | Per-job timeout |

## Notes

- Module caching is enabled when `go.sum` exists.
