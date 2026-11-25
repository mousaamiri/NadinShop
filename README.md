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

## تنظیمات (appsettings)

فایل `appsettings.json` حاوی اطلاعات محیطی و کلیدهای امنیتی است و همراه پروژه ارسال نمی‌شود. برای اجرای برنامه، ابتدا فایل خودتان را بسازید:

1. فایل نمونه `src/NadinShop.Api/appsettings.Example.json` را کپی کنید و نام آن را به `appsettings.json` تغییر دهید (کنار `NadinShop.Api.csproj`).
2. مقادیر را با اطلاعات درست محیط خودتان جایگزین کنید:
   - `ConnectionStrings:Default`: رشته اتصال SQL Server یا LocalDB شما.
   - `Jwt:SigningKey`: یک کلید محرمانه و تصادفی با طول حداقل ۳۲ بایت (برنامه در صورت کوتاه بودن کلید اجرا نمی‌شود).
   - `Jwt:Issuer` و `Jwt:Audience`: در صورت نیاز، مطابق تنظیمات سرویس خودتان.
3. سپس برنامه را اجرا کنید.

در حالت Docker نیازی به این فایل نیست؛ مقادیر از طریق متغیرهای محیطی در `docker-compose.yml` تنظیم می‌شوند.

## اجرا

```bash
dotnet build
dotnet test
dotnet run --project src/NadinShop.Api
```

پس از اجرا، وضعیت سرویس از مسیر `/health` قابل بررسی است.
