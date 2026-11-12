# Secrets and environments

Where to put API keys, tokens and deploy credentials, and how to get them to
a reusable workflow.

## Kinds of configuration

| Kind | Set with | Visible in logs | Use for |
|---|---|---|---|
| Repository **variable** | `gh variable set NAME` | Yes | Non-secret config: region, app name, feature flags |
| Repository **secret** | `gh secret set NAME` | Masked | Tokens CI needs on every run (e.g. a test API key) |
| **Environment** secret | `gh secret set NAME --env production` | Masked | Deploy credentials; only jobs with `environment: production` can read them |
| `GITHUB_TOKEN` | automatic | Masked | Talking to this repo's GitHub API; scoped by `permissions:` |

## Setting them

```bash
# Repository secret (prompts for the value; nothing lands in shell history)
gh secret set KALSHI_API_KEY

# From a file
gh secret set DEPLOY_SSH_KEY < ~/.ssh/deploy_key

# Environment secret
gh api -X PUT repos/{owner}/{repo}/environments/production
gh secret set FLY_API_TOKEN --env production

# Variable
gh variable set APP_REGION --body us-east-1
```

## Passing secrets to reusable workflows

Reusable workflows receive **no** secrets unless the caller passes them:

```yaml
jobs:
  image:
    uses: speedysebi/cicd-template/.github/workflows/docker.yml@v1
    with:
      registry: docker.io
      image: myuser/myapp
      push: true
    secrets:
      REGISTRY_USERNAME: ${{ secrets.DOCKERHUB_USERNAME }}
      REGISTRY_PASSWORD: ${{ secrets.DOCKERHUB_TOKEN }}
```

`secrets: inherit` passes all of them, which is convenient but gives every
workflow access to everything. Prefer listing them.

Workflow-level `env:` in the caller is also **not** passed down; use inputs.

## Environments

*Settings → Environments → New environment* (e.g. `production`):

- **Required reviewers**: add yourself — deploy jobs pause until you approve.
- **Wait timer**: optional delay before deploys.
- **Deployment branches**: limit to `main` so a branch can't deploy.
- **Environment secrets**: credentials that only this environment's jobs can
  read.

A job opts in with:

```yaml
deploy:
  runs-on: ubuntu-latest
  environment: production
```

## Secrets in pull requests

Secrets are not available to workflows triggered by PRs from forks. Design CI
so the per-PR jobs (lint, test, build) need no secrets, and keep jobs that do
(integration tests against real APIs, deploys) on `push` to `main`.

## Local development

Keep local secrets in an untracked `.env` (add it to `.gitignore`) and commit
an `env.example` with the variable names and fake values.
