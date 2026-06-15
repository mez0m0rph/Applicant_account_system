# DAST Report — OWASP ZAP

## Инструмент

Для динамического анализа безопасности был выбран OWASP ZAP Baseline Scan.

DAST проверяет уже запущенное приложение через HTTP. В отличие от SAST, он не анализирует исходный код, а смотрит на поведение приложения снаружи.

## Цель проверки

Было запущено приложение WebApp по адресу:

`http://localhost:5000`

После этого OWASP ZAP просканировал приложение и сформировал отчёт:

- `reports/zap-report.html`
- `reports/zap-report.md`

## Результаты первого запуска

ZAP обнаружил:

- High: 0
- Medium: 2
- Low: 6
- Informational: 4

Основные найденные проблемы:

1. `Content Security Policy (CSP) Header Not Set`
2. `Missing Anti-clickjacking Header`
3. `X-Content-Type-Options Header Missing`
4. `Permissions Policy Header Not Set`
5. `Application Error Disclosure`

## Объяснение найденных проблем

### Content Security Policy Header Not Set

CSP ограничивает источники, из которых браузер может загружать скрипты, стили, изображения и другие ресурсы. Отсутствие CSP повышает риск XSS-атак.

### Missing Anti-clickjacking Header

Отсутствие `X-Frame-Options` или CSP `frame-ancestors` позволяет встраивать страницу в iframe. Это может привести к clickjacking-атакам.

### X-Content-Type-Options Header Missing

Без `X-Content-Type-Options: nosniff` браузер может попытаться угадать тип содержимого, что может привести к небезопасной интерпретации файлов.

### Application Error Disclosure

ZAP обнаружил ответы `500 Internal Server Error` на `/Account/Login` и `/Account/Register`. Такие ошибки могут раскрывать техническую информацию и должны обрабатываться через безопасные error pages.

## Исправление

В `WebApp/Program.cs` были добавлены security headers:

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self'; frame-ancestors 'none'; object-src 'none'; base-uri 'self';";

    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["Permissions-Policy"] =
        "camera=(), microphone=(), geolocation=()";

    context.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin";
    context.Response.Headers["Cross-Origin-Resource-Policy"] = "same-origin";

    await next();
});
