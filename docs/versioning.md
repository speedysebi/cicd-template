# Versioning this template

How callers choose which version of these workflows they run, and how changes
here reach them.

## Refs you can use

```yaml
uses: speedysebi/cicd-template/.github/workflows/node-ci.yml@<ref>
```

| `<ref>` | Example | Behavior | Use for |
|---|---|---|---|
| Major tag | `@v1` | Moves to the latest `v1.x.y`; gets fixes and features, never breaking changes | **Default for your projects** |
| Exact tag | `@v1.2.0` | Never changes | Something that must not change under you |
| Commit SHA | `@3f2a...` | Never changes, can't be moved | Maximum supply-chain safety |
| Branch | `@main` | Whatever is on main right now | Trying unreleased changes |

Since you own this repo, `@v1` is a reasonable trade-off. Use a SHA for
anything you deploy to production if you ever share write access.

## Rules for changing a workflow

A **breaking change** is anything that can make a currently-green caller fail
or behave differently without the caller changing:

- removing or renaming an input, secret or output
- changing an input's default in a way that runs different commands
- requiring new permissions or secrets
- dropping support for a runtime version or OS

Breaking changes go into a new major version (`v2`). Everything else is a
minor (new input, new workflow) or patch (bug fix, action bump).

## Making a release of the template

1. Merge changes to `main`; wait for **Self-test** to pass.
2. Add an entry to [`CHANGELOG.md`](../CHANGELOG.md).
3. Tag and move the major tag:

   ```bash
   git checkout main && git pull
   git tag -a v1.3.0 -m "v1.3.0"
   git tag -fa v1 -m "v1 -> v1.3.0"
   git push origin v1.3.0
   git push origin v1 --force
   gh release create v1.3.0 --generate-notes
   ```

Callers on `@v1` pick it up on their next run.

## Starting v2

Create the tag `v2.0.0` and a new moving `v2` tag. Leave `v1` where it is so
existing projects keep working, and migrate them one at a time.
