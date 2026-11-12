# Contributing

## Layout

```
.github/workflows/   reusable workflows (on: workflow_call) + self-test.yml
.github/dependabot.yml
docs/                guides; docs/workflows/<name>.md per workflow
examples/            caller workflows to copy into projects
tests/fixtures/      tiny projects the self-test runs each workflow against
```

GitHub only finds reusable workflows directly in `.github/workflows/`, so no
subfolders there.

## Changing a workflow

1. Branch: `git checkout -b feat/python-coverage`.
2. Edit the workflow. Keep to the conventions below.
3. Update its page in `docs/workflows/` (inputs table, examples).
4. If behavior is new, make a fixture exercise it and add a job to
   `self-test.yml`.
5. Add a line under **Unreleased** in `CHANGELOG.md`.
6. Open a PR; **Self-test** must be green.

## Adding a workflow

- Name it `<language>-ci.yml` or by what it does (`docker.yml`).
- Add `docs/workflows/<name>.md`, an `examples/<name>.yml`, a fixture, a
  `self-test.yml` job, and a row in the README table.

## Conventions

- `on: workflow_call` only, with a description on every input.
- Top-level `permissions: contents: read`; any extra scope only on the job
  that needs it, and documented. (`docker.yml` deliberately inherits instead.)
- Pin every action to a full commit SHA with a `# vX.Y.Z` comment.
- `persist-credentials: false` on every checkout.
- Never put `${{ inputs.* }}` inside `run:`; map it to `env:` first.
- `shell: bash` so scripts behave the same on Windows runners.
- A `timeout-minutes` input on every job.
- Lists are JSON-string inputs (`'["ubuntu-latest"]'`) and go through `fromJSON`.
- Empty string means "skip" for optional commands.

## Releasing

See [docs/versioning.md](docs/versioning.md#making-a-release-of-the-template).
