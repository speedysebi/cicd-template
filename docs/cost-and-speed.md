# Cost and speed

Keeping pipelines fast and inside the free minutes.

## What costs minutes

- **Public repos**: standard GitHub-hosted runners are free.
- **Private repos**: usage counts against your plan's monthly quota. Linux is
  the cheapest; Windows and macOS runners use the quota faster. Check
  [GitHub's billing page](https://docs.github.com/en/billing/managing-billing-for-your-products/managing-billing-for-github-actions/about-billing-for-github-actions)
  for current numbers, and *Settings → Billing* for your usage.

Each job is billed separately, rounded up to the minute. A 3×3 matrix is nine
jobs.

## Built in

- Dependency caching (npm, Maven, Gradle, Go, uv) through the setup actions.
- `timeout-minutes` on every job, so a hung test can't eat hours.
- `fail-fast: false` matrices — more useful results per run, at the cost of
  not stopping early. Set your matrix small instead.

## In your caller workflows

**Cancel superseded runs** on the same branch:

```yaml
concurrency:
  group: ${{ github.workflow }}-${{ github.ref }}
  cancel-in-progress: true
```

(All examples include this. The full pipeline keeps runs on `main` alive so a
release isn't cut off halfway.)

**Skip CI for changes that can't break it:**

```yaml
on:
  pull_request:
    paths-ignore: ["**.md", "docs/**"]
```

Careful: if a skipped workflow is a *required* status check, the PR will wait
forever. Either don't use `paths-ignore` on required checks or make the check
non-required.

**Keep matrices honest.** `os: '["ubuntu-latest"]'` and one runtime version
unless you ship to several. Libraries: test oldest and newest supported
versions, not every one in between.

**Move slow suites off the PR path.** Run integration/e2e tests on `push` to
`main` or on a schedule:

```yaml
on:
  schedule:
    - cron: "0 6 * * 1"   # Mondays 06:00 UTC
```

## Speed targets

| Stage | Target |
|---|---|
| Lint + typecheck | < 2 min |
| Unit tests | < 5 min |
| Whole PR pipeline | < 10 min |

When a pipeline creeps past that, look at the run's timing breakdown first —
it's usually dependency installation without a cache hit, or one slow test
file.
