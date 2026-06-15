# SAST Report — Semgrep

## Инструмент

Для статического анализа безопасности был выбран Semgrep.

Semgrep анализирует исходный код без запуска приложения и ищет потенциально небезопасные конструкции.

## Настроенные правила

Были добавлены кастомные правила в файле:

`.semgrep/security-rules.yml`

Правила проверяют:

1. Hardcoded secrets — пароли, токены и ключи, записанные прямо в коде.
2. `HttpOnly = false` — небезопасные cookie, доступные из JavaScript.
3. `SecurePolicy = CookieSecurePolicy.None` — cookie без требования HTTPS.
4. `RequireHttpsMetadata = false` — отключение проверки HTTPS metadata.

## Результат первого запуска

Semgrep нашёл 4 проблемы:

1. `RequireHttpsMetadata = false` в `ProgramService/ProgramService.API/Program.cs`.
2. Hardcoded password в `security-demo/SastVulnerableDemo.cs`.
3. Hardcoded token в `security-demo/SastVulnerableDemo.cs`.
4. Cookie с `HttpOnly = false` в `security-demo/SastVulnerableDemo.cs`.

Файл отчёта:

`reports/semgrep-report.txt`

## Реальная найденная проблема

В проекте была найдена настройка:

```csharp
options.RequireHttpsMetadata = false;
