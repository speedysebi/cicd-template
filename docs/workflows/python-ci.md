# python-ci.yml

CI for Python projects using **pip, uv or poetry**.

## What it does

1. Sets up the requested Python versions.
2. Installs dependencies based on what the project has:

   | Found | Install | Commands run through |
   |---|---|---|
   | `uv.lock` | `uv sync --all-extras` | `uv run` |
   | `poetry.lock` | `poetry install` | `poetry run` |
   | otherwise | `pip install -r` each of `requirements.txt`, `requirements-dev.txt`, `dev-requirements.txt`, `requirements/dev.txt`; then `pip install -e ".[dev]"` if the project is a package | plain shell |

3. Runs `lint-command`, `typecheck-command`, `test-command`, `extra-command`
   (each skipped when empty) inside the project environment.

The default test command `pytest` installs pytest for pip projects if it's
missing and **passes when no tests are collected**, so a brand-new project is
green from the first commit.

## Usage

```yaml
jobs:
  ci:
    uses: speedysebi/cicd-template/.github/workflows/python-ci.yml@v1
    with:
      lint-command: ruff check .
```

## Inputs

| Input | Default | Description |
|---|---|---|
| `python-versions` | `'["3.12"]'` | JSON array |
| `os` | `'["ubuntu-latest"]'` | JSON array of runners |
| `working-directory` | `.` | Project folder |
| `install-command` | *(auto)* | Replace dependency installation |
| `lint-command` | | e.g. `ruff check .` or `ruff format --check .` |
| `typecheck-command` | | e.g. `mypy .` or `pyright` |
| `test-command` | `pytest` | Empty to skip tests |
| `extra-command` | | Anything else |
| `timeout-minutes` | `15` | Per-job timeout |

## Notes

- Commands are prefixed with `uv run` / `poetry run` automatically, so write
  `pytest -x`, not `uv run pytest -x`.
- Ruff, mypy etc. must be in your dev dependencies.
