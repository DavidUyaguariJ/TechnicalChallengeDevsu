# syntax=docker/dockerfile:1
#
# Devsu.PersonCustomer.Api
# Build context MUST be the repository root, because this API references
# projects that live outside its own folder (Domain/Application/Infrastructure).
#
#   docker build -f src/PersonCustomer/Devsu.PersonCustomer.Api/Dockerfile -t devsu/person-customer-api .

# ---------------------------------------------------------------------------
# Runtime base
# ---------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
# Kestrel listens on 8080 (HTTP only inside the container; TLS is terminated upstream).
ENV ASPNETCORE_HTTP_PORTS=8080
# Non-root user shipped with the image (uid 1654).
USER $APP_UID

# ---------------------------------------------------------------------------
# Build
# ---------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Project files first so the restore layer stays cached while only sources change.
COPY ["src/PersonCustomer/Devsu.PersonCustomer.Domain/Devsu.PersonCustomer.Domain.csproj", "src/PersonCustomer/Devsu.PersonCustomer.Domain/"]
COPY ["src/PersonCustomer/Devsu.PersonCustomer.Application/Devsu.PersonCustomer.Application.csproj", "src/PersonCustomer/Devsu.PersonCustomer.Application/"]
COPY ["src/PersonCustomer/Devsu.PersonCustomer.Infrastructure/Devsu.PersonCustomer.Infrastructure.csproj", "src/PersonCustomer/Devsu.PersonCustomer.Infrastructure/"]
COPY ["src/PersonCustomer/Devsu.PersonCustomer.Api/Devsu.PersonCustomer.Api.csproj", "src/PersonCustomer/Devsu.PersonCustomer.Api/"]

RUN dotnet restore "src/PersonCustomer/Devsu.PersonCustomer.Api/Devsu.PersonCustomer.Api.csproj"

# Only the sources this microservice needs, so a change in AccountMovement
# does not invalidate this image's cache.
COPY src/PersonCustomer/ src/PersonCustomer/

RUN dotnet build "src/PersonCustomer/Devsu.PersonCustomer.Api/Devsu.PersonCustomer.Api.csproj" \
    -c $BUILD_CONFIGURATION --no-restore -o /app/build

# ---------------------------------------------------------------------------
# Test
# ---------------------------------------------------------------------------
FROM build AS test
COPY ["tests/PersonCustomer/Devsu.PersonCustomer.UnitTests/Devsu.PersonCustomer.UnitTests.csproj", "tests/PersonCustomer/Devsu.PersonCustomer.UnitTests/"]
RUN dotnet restore "tests/PersonCustomer/Devsu.PersonCustomer.UnitTests/Devsu.PersonCustomer.UnitTests.csproj"
COPY tests/PersonCustomer/Devsu.PersonCustomer.UnitTests/ tests/PersonCustomer/Devsu.PersonCustomer.UnitTests/
RUN dotnet test "tests/PersonCustomer/Devsu.PersonCustomer.UnitTests/Devsu.PersonCustomer.UnitTests.csproj" \
    -c $BUILD_CONFIGURATION --no-restore

# ---------------------------------------------------------------------------
# Publish
# ---------------------------------------------------------------------------
FROM test AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "src/PersonCustomer/Devsu.PersonCustomer.Api/Devsu.PersonCustomer.Api.csproj" \
    -c $BUILD_CONFIGURATION --no-restore -o /app/publish /p:UseAppHost=false

# ---------------------------------------------------------------------------
# Final image
# ---------------------------------------------------------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Devsu.PersonCustomer.Api.dll"]
