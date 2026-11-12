# Security

The hardening built into these workflows, and the rules to follow in caller
workflows so you don't undo it.

## What the template does

| Measure | Where | Protects against |
|---|---|---|
| Third-party actions pinned to full commit SHAs | every `uses:` | A compromised action re-pointing its tags (as in the 2025 `tj-actions/changed-files` attack) |
| Dependabot updates the pins weekly, 7-day cooldown | `.github/dependabot.yml` | Pins going stale; adopting a malicious release the day it ships |
| `permissions: contents: read` by default | top of each workflow | A compromised step using the token to push code or releases |
| Write scopes only on the job that needs them | `release.yml` (`contents: write`) | Same |
| `persist-credentials: false` on checkout | every checkout | Later steps or uploaded artifacts leaking the token from `.git/config` |
| Inputs passed through `env:`, never `${{ }}` inside `run:` | all scripts | Script injection through crafted input values |
| actionlint + zizmor on every change | `workflow-lint.yml` via `self-test.yml` | Regressions of all of the above |

## Rules for caller workflows

1. **Set `permissions:` at the top of every workflow.** Start from
   `contents: read`, and grant more only on the job that needs it:

   ```yaml
   permissions:
     contents: read

   jobs:
     release:
       permissions:
         contents: write
       uses: speedysebi/cicd-template/.github/workflows/release.yml@v1
   ```

   A reusable workflow can never get more permissions than its caller job.

2. **Don't use `pull_request_target`** for anything that checks out PR code.
   It runs with your secrets and a write token on code from forks.

3. **Pass secrets explicitly.** Prefer

   ```yaml
   secrets:
     REGISTRY_PASSWORD: ${{ secrets.DOCKERHUB_TOKEN }}
   ```

   over `secrets: inherit`, so each workflow only sees what it needs.

4. **Keep deploy credentials in Environments**, not repository secrets.
   Environment secrets are only available to jobs that declare
   `environment: production`, which can require your approval. See
   [secrets-and-environments.md](secrets-and-environments.md).

5. **Prefer OIDC to long-lived keys.** AWS, Azure, GCP and npm (Trusted
   Publishing) can all trust a short-lived token GitHub mints per run
   (`permissions: id-token: write`). Nothing to leak or rotate.

6. **Never print secrets**, and don't upload `.env` files, `~/.npmrc` or
   coverage reports that might include environment dumps as artifacts.

## Enforcing pinning on your account

*Settings → Actions → General* on a repo (or account-wide on paid plans)
offers "Require actions to be pinned to a full-length commit SHA". Turn it on
for repos where you want the guarantee rather than the convention.

## Reporting problems

See [`SECURITY.md`](../SECURITY.md).
