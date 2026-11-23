# NadinShop

زیرساخت اولیه فروشگاه نادین‌شاپ بر پایه ASP.NET Core 8 و معماری تمیز (Clean Architecture).

## ساختار

```
NadinShop.sln
src/
  NadinShop.Domain/          موجودیت‌ها، Aggregate‌ها، قواعد کسب‌وکار (بدون وابستگی)
  NadinShop.Application/     سرویس‌های کاربردی، Result، قراردادها
  NadinShop.Infrastructure/  EF Core، DbContext، پیاده‌سازی‌های بیرونی
  NadinShop.Api/             Web API
tests/
  NadinShop.Domain.Tests/
  NadinShop.Application.Tests/
  NadinShop.IntegrationTests/
docs/
  ARCHITECTURE.md
```

## پیش‌نیازها

- .NET SDK 8.0.428 (نسخه در `global.json` قفل شده است)
- SQL Server یا LocalDB برای اجرای مهاجرت‌ها
