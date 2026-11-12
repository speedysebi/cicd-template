# release.yml

**Release on version bump.** Reads your project's version; if there's no tag
for it yet, creates the tag and a GitHub Release with generated notes. If the
tag exists, it does nothing, so it's safe to run on every push to `main`.

## Usage

```yaml
jobs:
  ci:
    uses: speedysebi/cicd-template/.github/workflows/node-ci.yml@v1

  release:
    needs: ci
    if: github.event_name == 'push' && github.ref == 'refs/heads/main'
    permissions:
      contents: write
    uses: speedysebi/cicd-template/.github/workflows/release.yml@v1

  publish:
    needs: release
    if: needs.release.outputs.released == 'true'
    runs-on: ubuntu-latest
    steps:
      - run: echo "Publishing ${{ needs.release.outputs.tag }}"
```

## Where the version comes from

With no `version` or `version-file`, the first of these found in
`working-directory` is used:

| File | Read from |
|---|---|
| `package.json` | `version` |
| `pyproject.toml` | `[project].version` or `[tool.poetry].version` |
| `Cargo.toml` | `[package].version` |
| `gradle.properties` | `version=` |
| `pom.xml` | `project.version` (via Maven) |
| `VERSION` | whole file |

## Inputs

| Input | Default | Description |
|---|---|---|
| `version` | | Use this version instead of reading a file |
| `version-file` | *(auto)* | One of the files above |
| `working-directory` | `.` | Where to look for the version file |
| `tag-prefix` | `v` | Tag = prefix + version |
| `prerelease` | `false` | Mark the release as a prerelease |
| `draft` | `false` | Create a draft release |
| `artifact-name` | | Artifact from this run to attach as release assets |
| `dry-run` | `false` | Resolve version and tag, create nothing |

## Outputs

| Output | Example |
|---|---|
| `released` | `'true'` only when a new release was created |
| `version` | `1.4.0` |
| `tag` | `v1.4.0` |

## Notes

- The calling job must grant `contents: write`.
- Tags created this way don't trigger `on: push: tags` workflows (GitHub
  stops `GITHUB_TOKEN` from triggering runs). Use the outputs, as above.
- Release notes come from merged PR titles; see
  [branching-and-releases.md](../branching-and-releases.md#better-release-notes)
  to group them.
