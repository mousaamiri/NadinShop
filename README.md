# NadinShop

زیرساخت اولیه فروشگاه نادین‌شاپ بر پایه ASP.NET Core 8، با طراحی دامنه بر اساس **DDD (Domain-Driven Design)** و چیدمان لایه‌ها بر اساس **Clean Architecture**.

NadinShop از الگوی **CQRS** (جداسازی Command و Query با MediatR)، نگاشت با **Mapster** و احراز هویت بر پایه **ASP.NET Core Identity و JWT** استفاده می‌کند.

دامنه به‌صورت غنی (Rich Domain Model) طراحی شده است: Aggregate Rootها، Entityها، Value Objectها و رخدادهای دامنه، با قواعد کسب‌وکار و ناوَریانت‌هایی که داخل خود دامنه اعمال می‌شوند و مستقل از دیتابیس و فریم‌ورک هستند. جزئیات بیشتر در `docs/ARCHITECTURE.md`.

## ساختار

```
NadinShop.sln
src/
  NadinShop.Domain/          Aggregateها، Entityها، Value Objectها، رخدادهای دامنه (بدون وابستگی)
  NadinShop.Application/     CQRS (Command/Query)، Result، Mapster Mapping، قراردادها
  NadinShop.Infrastructure/  EF Core، DbContext، Repositoryها، ASP.NET Core Identity و JWT
  NadinShop.Api/             Web API و Composition Root
tests/
  NadinShop.Domain.Tests/
  NadinShop.Application.Tests/
  NadinShop.Infrastructure.Tests/
  NadinShop.IntegrationTests/
docs/
  ARCHITECTURE.md
```

## پیش‌نیازها

- .NET SDK 8.0.428 (نسخه در `global.json` قفل شده است)
- SQL Server یا LocalDB برای اجرای مهاجرت‌ها

## اجرا

```bash
dotnet build
dotnet test
dotnet run --project src/NadinShop.Api
```

پس از اجرا، وضعیت سرویس از مسیر `/health` قابل بررسی است.
