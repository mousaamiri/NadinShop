# Running NadinShop with Docker Compose

```bash
docker compose up --build
```

The API is available at `http://localhost:8080` and Swagger UI at
`http://localhost:8080/swagger`. The database is created automatically on startup
through EF Core migrations.

Services:

- `sqlserver`: SQL Server 2022 with a persistent `sqlserver-data` volume.
- `api`: the ASP.NET Core API built from `Dockerfile`.

Configuration is supplied through environment variables in `docker-compose.yml`
(`ConnectionStrings__Default`, `Jwt__*`). Replace the sample SA password and JWT
signing key before using this setup outside local development.
