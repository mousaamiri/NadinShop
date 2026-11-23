# معماری NadinShop

## لایه‌ها

| لایه | مسئولیت | وابستگی مجاز |
| --- | --- | --- |
| `NadinShop.Domain` | موجودیت‌ها، Aggregate Root، رخدادهای دامنه، قواعد کسب‌وکار | هیچ |
| `NadinShop.Application` | Use Caseها، DTOها، `Result`، واسط‌های موردنیاز زیرساخت | Domain |
| `NadinShop.Infrastructure` | EF Core، `NadinShopDbContext`، پیاده‌سازی سرویس‌های بیرونی | Application |
| `NadinShop.Api` | نقاط پایانی Web API | Application، Infrastructure |

## قواعد وابستگی

- `Domain` به هیچ پروژه‌ای وابسته نیست.
- `Application` فقط به `Domain` وابسته است و نباید EF Core یا ASP.NET Core را بشناسد.
- `Infrastructure` جزئیات فنی را پیاده‌سازی می‌کند و رو به `Application` است.
- پروژه اجرایی (`Api`) فقط ترکیب‌کننده (Composition Root) است.
- تست‌ها به لایه مربوط به خود وابسته‌اند؛ `IntegrationTests` میزبان `Api` را پوشش می‌دهد.
