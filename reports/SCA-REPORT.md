# SCA Report — NuGet Vulnerability Scan

## Инструмент

Для анализа зависимостей был использован встроенный механизм .NET CLI:

`dotnet list package --vulnerable --include-transitive`

Также была предпринята попытка использовать OWASP Dependency-Check, но он не смог завершить обновление базы NVD из-за ограничения API:

`NVD Returned Status Code: 429`

Поэтому для выполнения лабораторной был использован NuGet vulnerability scan, который также относится к SCA / Dependency Checker.

## Цель проверки

Проверить зависимости ASP.NET Core проекта Applicant Account System на известные уязвимости.

## Проверка основного проекта

Были выполнены команды:

```bash
dotnet list WebApp/WebApp.csproj package --vulnerable --include-transitive
dotnet list ApplicantSystem.slnx package --vulnerable --include-transitive
