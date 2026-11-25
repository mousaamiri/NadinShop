FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props global.json ./
COPY src/NadinShop.Domain/NadinShop.Domain.csproj src/NadinShop.Domain/
COPY src/NadinShop.Application/NadinShop.Application.csproj src/NadinShop.Application/
COPY src/NadinShop.Infrastructure/NadinShop.Infrastructure.csproj src/NadinShop.Infrastructure/
COPY src/NadinShop.Api/NadinShop.Api.csproj src/NadinShop.Api/

RUN dotnet restore src/NadinShop.Api/NadinShop.Api.csproj

COPY src/ src/

RUN dotnet publish src/NadinShop.Api/NadinShop.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "NadinShop.Api.dll"]
