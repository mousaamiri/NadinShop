# معماری NadinShop

## نگاه کلی

NadinShop با ترکیب دو رویکرد طراحی شده است: **DDD (Domain-Driven Design)** برای مدل‌سازی قلب کسب‌وکار و **Clean Architecture** برای جداسازی لایه‌ها و جهت وابستگی‌ها. هدف این است که قواعد کسب‌وکار مستقل از دیتابیس، فریم‌ورک وب و کتابخانه‌های بیرونی باقی بمانند و تغییر در جزئیات فنی، دامنه را تحت تأثیر قرار ندهد.

## فلسفه DDD در دامنه

دامنه بر اساس مفاهیم DDD طراحی شده است، نه به‌صورت مدل کم‌جان (Anemic Model):

- **Aggregate Root**: هر Aggregate با یک ریشه و مرز مشخص تعریف می‌شود و تنها راه تغییر وضعیت آن، عبور از متدهای خود Aggregate است (نمونه: `Product`).
- **Entity و Value Object**: موجودیت‌ها با هویت (`Id`) شناخته می‌شوند و Value Objectها بدون هویت و مبتنی بر مقدار هستند (`ProductName`، `ManufacturePhone`، `ManufactureEmail`، `ProduceDate`).
- **قواعد و Invariant  درون دامنه**: اعتبارسنجی و قواعد کسب‌وکار در سازنده‌های Value Object و متدهای Aggregate اعمال می‌شوند تا شیء نامعتبر اصلاً ساخته نشود.
- **Scream Architecture**: ساختار پوشه‌ها و نام‌گذاری‌ها بر اساس مفهوم کسب‌وکار (مثل `Products`) است، نه بر اساس نوع فنی کلاس‌ها.
- **رخدادهای دامنه**: ریشه‌های Aggregate امکان ثبت رخداد دامنه (`IDomainEvent`) را دارند تا تغییرات مهم دامنه قابل انتشار باشند.
- **ساخت از طریق Factory**: ساخت Aggregate از طریق متد کارخانه‌ای انجام می‌شود تا Invariant  همیشه برقرار بمانند.

## لایه‌ها

| لایه | مسئولیت | وابستگی مجاز |
| --- | --- | --- |
| `NadinShop.Domain` | موجودیت‌ها، Aggregate Root، Value Objectها، رخدادهای دامنه، قواعد کسب‌وکار | هیچ |
| `NadinShop.Application` | Use Caseها (Command/Query)، DTOها، `Result`، واسط‌های موردنیاز زیرساخت | Domain |
| `NadinShop.Infrastructure` | EF Core، `NadinShopDbContext`، Repositoryها، Identity/JWT، سرویس‌های بیرونی | Application |
| `NadinShop.Api` | نقاط پایانی Web API و Composition Root | Application، Infrastructure |

## قواعد وابستگی

- `Domain` به هیچ پروژه‌ای وابسته نیست و هیچ ارجاعی به EF Core یا ASP.NET Core ندارد.
- `Application` فقط به `Domain` وابسته است و جزئیات فنی را از طریق واسط‌ها (مانند `IRepository`، `IUnitOfWork`، `ICurrentUserService`) می‌شناسد.
- `Infrastructure` واسط‌های لایه Application را پیاده‌سازی می‌کند و رو به داخل است.
- `Api` فقط لایه‌ها را به هم متصل می‌کند (Composition Root) و منطق کسب‌وکار ندارد.
- تست‌ها به لایه مربوط به خود وابسته‌اند؛ `IntegrationTests` میزبان `Api` را پوشش می‌دهد.

## لایه Application

- **CQRS**: خواندن و نوشتن از هم جدا شده‌اند؛ Commandها وضعیت را تغییر می‌دهند و Queryها فقط داده برمی‌گردانند. هر Use Case یک Command/Query با Handler اختصاصی دارد و از طریق MediatR ارسال می‌شود.
- **Result Pattern** به‌جای استثنا برای خطاهای قابل پیش‌بینی (اعتبارسنجی، عدم یافت شدن، تعارض).
- **Pipeline Behaviorها**: اعتبارسنجی با FluentValidation و ذخیره‌سازی خودکار با Unit of Work در خط لوله MediatR اجرا می‌شوند.
- **Mapster Mapping**: نگاشت بین Aggregateها و DTOها/Responseها با Mapster انجام می‌شود و پیکربندی نگاشت‌ها به‌صورت متمرکز در `MappingConfig` تعریف شده است.

## لایه Infrastructure

- EF Core با `NadinShopDbContext` و پیکربندی‌های جداگانه (`IEntityTypeConfiguration`) برای هر Aggregate.
- نگاشت Value Objectها با Conversion و تعریف ایندکس‌های یکتا در سطح دیتابیس.
- پیاده‌سازی Repositoryها، Unit of Work و سرویس‌های بیرونی.

## احراز هویت

- احراز هویت بر پایه **ASP.NET Core Identity** و **JWT** پیاده‌سازی شده است.
- مدیریت کاربران (ثبت‌نام، یکتایی ایمیل و شماره تلفن) و هش رمز عبور با `IPasswordHasher` از ASP.NET Core Identity انجام می‌شود.
- پس از اعتبارسنجی کاربر، **Access Token** و **Refresh Token** با JWT صادر می‌شود.
- لایه Application فقط واسط‌های `IIdentityService`، `IJwTokenService` و `ICurrentUserService` را می‌شناسد و پیاده‌سازی آن‌ها در Infrastructure قرار دارد.

## تست

- `Domain.Tests` روی رفتار Aggregate و Value Objectها تمرکز دارد.
- `Application.Tests` رفتار Handlerها، Validatorها و Behaviorها را پوشش می‌دهد.
- `Infrastructure.Tests` پیاده‌سازی‌های EF Core و سرویس‌ها را بررسی می‌کند.
- `IntegrationTests` مسیر کامل از میزبان `Api` را آزمون می‌کند.
