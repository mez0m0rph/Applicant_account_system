# CI/CD Report — GitHub Actions

## Цель

Интегрировать SAST, DAST и SCA проверки в CI/CD pipeline, чтобы security-анализ запускался автоматически при push, pull request и вручную через workflow_dispatch.

## Файл workflow

CI/CD настроен в файле:

`.github/workflows/security.yml`

## Настроенные jobs

### 1. SAST - Semgrep

Semgrep запускается в двух режимах:

1. Проверка реального проекта без папки `security-demo`.
2. Проверка демонстрационных уязвимостей в `security-demo`.

Команда для реального проекта:

```bash
semgrep scan --config .semgrep/security-rules.yml . --exclude security-demo --json --output reports/semgrep-ci-report.json
