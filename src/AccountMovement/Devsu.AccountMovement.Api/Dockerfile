# syntax=docker/dockerfile:1
#
# Devsu.AccountMovement.Api
# Build context MUST be the repository root, because this API references
# projects that live outside its own folder (Domain/Application/Infrastructure/Shared).
#
#   docker build -f src/AccountMovement/Devsu.AccountMovement.Api/Dockerfile -t devsu/account-movement-api .

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
COPY ["src/Shared/Devsu.Shared.Contracts/Devsu.Shared.Contracts.csproj", "src/Shared/Devsu.Shared.Contracts/"]
COPY ["src/AccountMovement/Devsu.AccountMovement.Domain/Devsu.AccountMovement.Domain.csproj", "src/AccountMovement/Devsu.AccountMovement.Domain/"]
COPY ["src/AccountMovement/Devsu.AccountMovement.Application/Devsu.AccountMovement.Application.csproj", "src/AccountMovement/Devsu.AccountMovement.Application/"]
COPY ["src/AccountMovement/Devsu.AccountMovement.Infrastructure/Devsu.AccountMovement.Infrastructure.csproj", "src/AccountMovement/Devsu.AccountMovement.Infrastructure/"]
COPY ["src/AccountMovement/Devsu.AccountMovement.Api/Devsu.AccountMovement.Api.csproj", "src/AccountMovement/Devsu.AccountMovement.Api/"]

RUN dotnet restore "src/AccountMovement/Devsu.AccountMovement.Api/Devsu.AccountMovement.Api.csproj"

# Only the sources this microservice needs, so a change in PersonCustomer
# does not invalidate this image's cache.
COPY src/Shared/ src/Shared/
COPY src/AccountMovement/ src/AccountMovement/

RUN dotnet build "src/AccountMovement/Devsu.AccountMovement.Api/Devsu.AccountMovement.Api.csproj" \
    -c $BUILD_CONFIGURATION --no-restore -o /app/build

# ---------------------------------------------------------------------------
# Publish
# ---------------------------------------------------------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "src/AccountMovement/Devsu.AccountMovement.Api/Devsu.AccountMovement.Api.csproj" \
    -c $BUILD_CONFIGURATION --no-restore -o /app/publish /p:UseAppHost=false

# ---------------------------------------------------------------------------
# Final image
# ---------------------------------------------------------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Devsu.AccountMovement.Api.dll"]
