# CI/CD strategies for small projects

Research notes behind the choices in this template: what the common pipeline
strategies are, what they cost, and which ones actually pay off when the team
is one to five people.

## TL;DR — what works for small projects

| Decision | Recommendation | Why |
|---|---|---|
| Branching | **Trunk-based**: `main` + short-lived branches/PRs | Least ceremony; long-lived branches defeat the point of CI |
| CI platform | **GitHub Actions** (if code is on GitHub) | No extra account or service; generous free minutes for public repos |
| Sharing pipelines | **Reusable workflows** in one repo, versioned with tags | One fix updates every project; callers stay ~10 lines |
| What runs on every push | lint → typecheck → test → build, under ~10 min | Fast feedback is the whole value of CI |
| Releases | **Release on version bump** to `main` | No manual tagging; nothing ships by accident |
| Deploys | Automatic to staging (if any), **manual approval** to production via Environments | Safety without a release manager |
| Security | Least-privilege `GITHUB_TOKEN`, SHA-pinned actions + Dependabot, secrets only in Environments | Cheap, prevents the common real-world compromises |

Everything below explains those choices and the alternatives.

---

## 1. Branching strategy

### Options

**Git Flow** — `main`, `develop`, `feature/*`, `release/*`, `hotfix/*`.
Designed for versioned products with scheduled releases and several
supported versions at once. For a small project it is mostly overhead: two
permanent branches to keep in sync, release branches nobody needs, and merges
that pile up.

**GitHub Flow** — `main` is always deployable; work happens on a short branch,
opened as a PR, merged after CI passes. This is trunk-based development with
PRs and is what this template assumes.

**Pure trunk-based** — commit straight to `main`, rely on CI to catch
breakage. Fine for a true solo project; you lose PR checks before merge, so a
red build lands on `main` and is fixed forward.

### Recommendation

Use GitHub Flow. Keep branches to a day or two — trunk-based development
loses its benefit once branches live longer than that, because integration
problems are deferred exactly as with long-lived feature branches. Protect
`main` so CI must pass before merge (see
[branching-and-releases.md](branching-and-releases.md)).

## 2. Where to run CI

For code hosted on GitHub, GitHub Actions is the default choice: no
separate service, account or webhook, and the checks show up natively on PRs.
CircleCI, Buildkite, GitLab CI and Jenkins are all capable, but for a solo or
small project they add an extra system to maintain without solving a problem
you have.

Cost: public repositories run on GitHub-hosted runners for free. Private
repositories get a monthly quota of free minutes on the free plan (check
[GitHub's billing docs](https://docs.github.com/en/billing/managing-billing-for-your-products/managing-billing-for-github-actions/about-billing-for-github-actions)
for the current number); Windows and macOS runners consume that quota faster
than Linux. See [cost-and-speed.md](cost-and-speed.md) for staying inside it.

## 3. How to share pipelines between projects

GitHub offers three mechanisms:

| | Reusable workflow | Composite action | Workflow template / copy-paste |
|---|---|---|---|
| Unit | Whole job(s) | Steps inside a job | A file you copy |
| Lives in | `.github/workflows/*.yml` with `on: workflow_call` | `action.yml` in a repo/dir | Anywhere |
| Updates reach callers | Yes, on the next run (per pinned ref) | Yes (per pinned ref) | No — every copy drifts |
| Can define runners, matrix, permissions, services | Yes | No (inherits caller's job) | Yes |
| Secrets | Explicit `secrets:` or `secrets: inherit` | Must be passed as inputs | n/a |
| Nesting | Up to 10 levels | Up to 10 composite actions per workflow | n/a |

**This template uses reusable workflows** because a small project wants the
*whole* job handled — runner, matrix, caching, setup, test — with the caller
supplying only a few inputs. Composite actions are better when you want to
share a handful of steps inside jobs you still write yourself; you can add
some later under `actions/` if that need comes up.

Copying a template file is the simplest option but the worst over time: each
project ends up with a slightly different, slightly outdated pipeline.

### Visibility rules that matter

From GitHub's docs on reusing workflows:

- A **public** caller can only use reusable workflows from **public** repos.
- A **private** caller can use workflows from public repos and from private
  repos whose *Actions → General → Access* setting allows it.
- Workflow-level `env` from the caller is **not** passed into the called
  workflow; use inputs or `vars`.
- Permissions can only be kept or reduced down the chain, never elevated.
- One workflow file can call at most 50 unique reusable workflows.

So: if any project that will use this template is public, the template repo
must be public too. There is nothing secret in a pipeline definition.

## 4. What the pipeline should do

The standard stages, and which ones a small project should actually run:

1. **Install** with a frozen lockfile (`npm ci`, `uv sync`, `--frozen-lockfile`) —
   builds must not silently pick up new dependency versions.
2. **Lint and format check** — cheap, catches a large share of review noise.
3. **Type-check** — where the language has one.
4. **Unit tests** — the core of CI.
5. **Build / package** — proves the artifact can actually be produced.
6. **Integration / end-to-end tests** — valuable but slow and often need
   secrets; run them on `main` or nightly rather than on every PR.
7. **Release** — tag + changelog + publish.
8. **Deploy** — staging automatically, production behind an approval.

Aim for the per-PR part (1–5) to finish in under ten minutes. Past that,
people stop waiting for it.

### Matrix builds

Test against more than one OS or runtime version only where it buys
something: a library that users run on several Node/Python versions, or a CLI
used on Windows. For an app you deploy to one Linux box, a single
`ubuntu-latest` job is enough. Matrices multiply minutes.

## 5. Release strategies

| Strategy | How it works | Good for |
|---|---|---|
| **Manual tags** | You run `git tag v1.2.0 && git push --tags` | Rare releases; easy to forget steps |
| **Release on version bump** | CI on `main` reads the version from `package.json` etc.; if no tag exists, it tags and releases | Small projects — the version bump in a PR *is* the release decision |
| **Conventional commits + automation** (semantic-release, release-please, changesets) | Commit messages determine the next version and changelog | Libraries with many contributors and strict semver |
| **Continuous deployment** | Every green `main` is deployed; no versions | Web apps and services with good tests |

This template implements **release on version bump** (`release.yml`). It
needs no commit-message discipline, it is idempotent (re-running on the same
version does nothing), and it composes with CD: deploy jobs run only when
`released == 'true'`.

Note: tags created with the default `GITHUB_TOKEN` do not trigger other
workflows (to prevent loops). Chain publish/deploy jobs on the release
workflow's *outputs* in the same pipeline, as in
[`examples/full-pipeline.yml`](../examples/full-pipeline.yml), rather than on
`on: push: tags`.

## 6. Deployment strategies

- **Push-to-deploy**: every release is deployed automatically. Fine when you
  have good tests and easy rollback.
- **Approval gate**: use a GitHub **Environment** (`production`) with yourself
  as a required reviewer. The job pauses until you click *Approve*. This is
  the best default for a small project: automation does the work, you keep
  the final say.
- **Blue/green, canary, feature flags**: real techniques, but they need
  infrastructure that small projects rarely have. Revisit when downtime or
  bad deploys actually hurt.

Prefer **OIDC** (`id-token: write` + the cloud provider's login action, or
npm Trusted Publishing) over long-lived cloud keys stored as secrets.

## 7. Security baseline

Small projects get hit by the same supply-chain attacks as big ones. In March
2025, the popular `tj-actions/changed-files` action had its existing tags
repointed to malicious code that dumped CI secrets into logs of thousands of
repositories (CVE-2025-30066). Every repo referencing it by tag ran the
attacker's code. Cheap defenses, all applied in this template:

- **Pin third-party actions to a full commit SHA**, with the version in a
  comment (`uses: x/y@<sha> # v1.2.3`). Tags can be moved; SHAs cannot.
- **Let Dependabot update the pins** (`.github/dependabot.yml`), with a
  cooldown so brand-new releases age a few days first.
- **Least-privilege `permissions:`** — default `contents: read`, and grant
  write scopes only on the jobs that need them.
- **`persist-credentials: false`** on checkout so the token isn't left in
  `.git/config` for later steps.
- **No `${{ inputs.* }}` directly inside `run:` scripts** — pass through `env:`
  to avoid script injection.
- **Avoid `pull_request_target`** unless you fully understand it; it runs with
  secrets on code from forks.
- **Lint workflows** with actionlint and audit them with zizmor
  (`workflow-lint.yml`).

Details in [security.md](security.md).

## 8. Anti-patterns to avoid

- A pipeline that only runs on `main` — you find out after merging.
- Tests that hit real external APIs on every PR (flaky, slow, needs secrets).
- `latest` versions of tools everywhere — non-reproducible builds. Pin, and
  let Dependabot move them.
- Copy-pasted pipelines per project that slowly diverge.
- Deploying from a laptop "just this once".
- Secrets in repository variables, logs, or committed `.env` files.

## Sources

- GitHub Docs — [Reusing workflow configurations](https://docs.github.com/en/actions/reference/workflows-and-actions/reusing-workflow-configurations) (access rules, limits, `env` and outputs)
- GitHub Docs — [Reuse workflows](https://docs.github.com/en/actions/how-tos/reuse-automations/reuse-workflows) (nesting, permissions, secrets)
- GitHub Blog — [Using reusable workflows with GitHub Actions](https://github.blog/developer-skills/github/using-reusable-workflows-github-actions)
- Tenki — [Reusable workflows vs composite actions](https://tenki.cloud/blog/reusable-workflows-vs-composite-actions)
- Aviator — [Why trunk-based development is key to continuous delivery](https://www.aviator.co/blog/?p=3739)
- Simon Inglis — [Solo dev release process: safety without ceremony](https://simoninglis-com.pages.dev/posts/solo-dev-release-process)
- DeployHQ — [How to build a CI/CD pipeline (with GitHub Actions)](https://deployhq.com/blog/ci-cd-pipelines-complete-guide)
- Brights — [CI/CD best practices](https://brights.io/blog/cicd-best-practices)
- Semgrep — [SHA pinning for GitHub Actions org-wide](https://semgrep.dev/blog/2026/sha-pinning-for-github-actions-org-wide)
- Lullabot ADR — [Always pin GitHub Actions to a commit hash](https://architecture.lullabot.com/adr/20260505-pin-github-actions-to-hash/)
- Classmethod — [Securely pinning action versions](https://dev.classmethod.jp/en/articles/github-actions-secure-action-pinning/)
