# node-ci.yml

CI for JavaScript and TypeScript projects using **npm, pnpm, yarn or bun**.

## What it does

1. Checks out the code and sets up Node (and Bun, pnpm or yarn if selected).
2. Installs dependencies from the lockfile (`npm ci`, `pnpm install --frozen-lockfile`,
   `yarn install --immutable` / `--frozen-lockfile` for yarn 1, `bun install --frozen-lockfile`).
3. Runs each script in `scripts` **that exists in package.json**, in order.
   Missing scripts are skipped, so the default works for any project.
4. Runs `extra-command`, if any.
5. Uploads `artifact-path` as an artifact, if set (from the first matrix job only).

## Usage

```yaml
jobs:
  ci:
    uses: speedysebi/cicd-template/.github/workflows/node-ci.yml@v1
```

## Inputs

| Input | Default | Description |
|---|---|---|
| `package-manager` | `npm` | `npm`, `pnpm`, `yarn` or `bun` |
| `node-versions` | `'["22"]'` | JSON array; one job per version |
| `bun-version` | `latest` | Used when `package-manager: bun` |
| `os` | `'["ubuntu-latest"]'` | JSON array of runners |
| `working-directory` | `.` | Folder containing `package.json` (monorepos) |
| `install-command` | *(auto)* | Replace the install step entirely |
| `scripts` | `lint typecheck test build` | Scripts to run if present |
| `extra-command` | | Any extra shell command, e.g. `npm pack --dry-run` |
| `artifact-path` | | Path to upload, relative to `working-directory`, e.g. `dist` |
| `artifact-name` | `build` | Name of the uploaded artifact |
| `timeout-minutes` | `15` | Per-job timeout |

## Examples

Library tested on several Node versions and Windows:

```yaml
with:
  node-versions: '["20", "22", "24"]'
  os: '["ubuntu-latest", "windows-latest"]'
```

pnpm monorepo package:

```yaml
with:
  package-manager: pnpm
  working-directory: packages/api
```

Build once, then attach the output to a release:

```yaml
jobs:
  ci:
    uses: speedysebi/cicd-template/.github/workflows/node-ci.yml@v1
    with:
      artifact-path: dist
  release:
    needs: ci
    permissions:
      contents: write
    uses: speedysebi/cicd-template/.github/workflows/release.yml@v1
    with:
      artifact-name: build     # attaches dist/* to the GitHub Release
```

## Notes

- pnpm and yarn are enabled through corepack (respecting `packageManager` in
  package.json); on Node versions without corepack they're installed with npm.
- npm dependency caching is on when `package-lock.json` exists.
