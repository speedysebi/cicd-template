# Examples

Caller workflows to copy into a project's `.github/workflows/` folder.

| File | Copy as | For |
|---|---|---|
| [`node.yml`](node.yml) | `ci.yml` | npm / pnpm / yarn projects |
| [`bun.yml`](bun.yml) | `ci.yml` | Bun projects |
| [`python.yml`](python.yml) | `ci.yml` | pip / uv / poetry projects |
| [`java.yml`](java.yml) | `ci.yml` | Gradle / Maven projects |
| [`dotnet.yml`](dotnet.yml) | `ci.yml` | .NET projects |
| [`go.yml`](go.yml) | `ci.yml` | Go modules |
| [`workflow-lint.yml`](workflow-lint.yml) | `workflow-lint.yml` | Any repo, lints its workflows |
| [`zizmor.yml`](zizmor.yml) | `../zizmor.yml` (i.e. `.github/zizmor.yml`) | Needed with workflow-lint so `@v1` refs pass the audit |
| [`full-pipeline.yml`](full-pipeline.yml) | `pipeline.yml` | CI → release → Docker → deploy |

Copy one with the GitHub CLI:

```bash
mkdir -p .github/workflows
gh api repos/speedysebi/cicd-template/contents/examples/node.yml \
  -H "Accept: application/vnd.github.raw" > .github/workflows/ci.yml
```

All examples pin `@v1`. See [../docs/versioning.md](../docs/versioning.md)
for other options.
