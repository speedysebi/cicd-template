# java-ci.yml

CI for Java (and Kotlin/Scala on the JVM) projects built with **Gradle or Maven**.

## What it does

1. Detects the build tool: Gradle if there's `gradlew`, `build.gradle(.kts)` or
   `settings.gradle(.kts)`; otherwise Maven if there's `pom.xml`.
2. Sets up the JDK(s) and caching (Maven via setup-java, Gradle via
   `gradle/actions/setup-gradle`).
3. Runs `./gradlew build` / `gradle build` or `./mvnw -B verify` / `mvn -B verify`.
   Wrappers are preferred when committed.
4. On failure, uploads Gradle test reports and Maven surefire reports as
   `test-reports-<os>-java<version>`.
5. Uploads `artifact-path` if set.

## Usage

```yaml
jobs:
  ci:
    uses: speedysebi/cicd-template/.github/workflows/java-ci.yml@v1
```

## Inputs

| Input | Default | Description |
|---|---|---|
| `java-versions` | `'["21"]'` | JSON array of JDK versions |
| `distribution` | `temurin` | Any distribution actions/setup-java supports |
| `build-tool` | *(auto)* | `gradle` or `maven` |
| `os` | `'["ubuntu-latest"]'` | JSON array of runners |
| `working-directory` | `.` | Project folder |
| `command` | *(auto)* | Replace the build command, e.g. `./gradlew check jacocoTestReport` |
| `artifact-path` | | Glob to upload, e.g. `build/libs/*.jar` or `target/*.jar` |
| `artifact-name` | `build` | Name of the uploaded artifact |
| `timeout-minutes` | `20` | Per-job timeout |

## Notes

- Projects without any build tool (plain IntelliJ modules) aren't supported;
  add a minimal `build.gradle` or `pom.xml`.
- Commit `gradlew` as executable: `git update-index --chmod=+x gradlew`.
