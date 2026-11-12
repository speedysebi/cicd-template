# docker.yml

Builds a Docker image with Buildx and optionally pushes it: to GitHub
Container Registry (GHCR) by default, or to any registry with credentials.

## What it does

- Tags the image automatically: branch name, PR number, short SHA, semver
  tags on tag pushes, `latest` on the default branch, plus `version` if given.
- Uses the GitHub Actions cache for layers, so rebuilds are fast.
- Outputs the image name and digest for deploy jobs.

## Usage

Build only (e.g. on PRs, to prove the Dockerfile works):

```yaml
jobs:
  image:
    uses: speedysebi/cicd-template/.github/workflows/docker.yml@v1
```

Build and push to GHCR:

```yaml
jobs:
  image:
    permissions:
      contents: read
      packages: write
    uses: speedysebi/cicd-template/.github/workflows/docker.yml@v1
    with:
      push: true
```

Push to Docker Hub:

```yaml
    with:
      registry: docker.io
      image: myuser/myapp
      push: true
    secrets:
      REGISTRY_USERNAME: ${{ secrets.DOCKERHUB_USERNAME }}
      REGISTRY_PASSWORD: ${{ secrets.DOCKERHUB_TOKEN }}
```

## Inputs

| Input | Default | Description |
|---|---|---|
| `context` | `.` | Build context |
| `file` | `Dockerfile` | Dockerfile path from the repo root |
| `registry` | `ghcr.io` | Registry host |
| `image` | *owner/repo* | Image name without the registry |
| `push` | `false` | Push after building |
| `platforms` | `linux/amd64` | e.g. `linux/amd64,linux/arm64` (arm64 builds are slower under emulation) |
| `version` | | Extra tag, typically `needs.release.outputs.version` |
| `build-args` | | Newline-separated `KEY=value` |

## Secrets

| Secret | Default |
|---|---|
| `REGISTRY_USERNAME` | `github.actor` |
| `REGISTRY_PASSWORD` | `GITHUB_TOKEN` (works for GHCR) |

## Outputs

| Output | Example |
|---|---|
| `image` | `ghcr.io/speedysebi/myapp` |
| `digest` | `sha256:...` |

Deploy by digest (`image@digest`), not by tag: it's exactly what was built.

## Notes

- The workflow declares no permissions of its own; it inherits the calling
  job's. Pushing to GHCR needs `packages: write` on the calling job.
- A new GHCR package is private by default; change it under the package's
  settings on GitHub if it should be public.
