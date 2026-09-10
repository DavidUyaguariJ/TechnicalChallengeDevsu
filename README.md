# Devsu — Prueba Técnica (Microservicios .NET)

Dos microservicios independientes sobre .NET 10 que cubren el reto técnico de Devsu: gestión de clientes (`PersonCustomer`) y gestión de cuentas/movimientos con reportes (`AccountMovement`), comunicados entre sí de forma síncrona por HTTP.

## Cómo levantarlo

### Local (sin Docker)

Cada Api lee su cadena de conexión (y, en el caso de `AccountMovement`, la URL del otro servicio) desde un archivo `.env` propio, no versionado, con la cadena de conexión completa como un solo valor:

```
src/PersonCustomer/Devsu.PersonCustomer.Api/.env
src/AccountMovement/Devsu.AccountMovement.Api/.env
```

Junto a cada uno hay un `.env.example` con las claves a completar. Con SQL Server accesible y los `.env` creados:

```powershell
dotnet run --project src/PersonCustomer/Devsu.PersonCustomer.Api
dotnet run --project src/AccountMovement/Devsu.AccountMovement.Api
```

`PersonCustomer` queda en `http://localhost:5046`, `AccountMovement` en `http://localhost:5004`.

### Con Docker Compose

Las imágenes **ya están construidas y publicadas** en Docker Hub, una por cada `Dockerfile` (contexto de build = raíz del repositorio, porque cada Api referencia proyectos fuera de su propia carpeta):

| Imagen | Construida a partir de |
| :--- | :--- |
| `daviduyaguarij/devsu-customer:latest` | `src/PersonCustomer/Devsu.PersonCustomer.Api/Dockerfile` |
| `daviduyaguarij/devsu-account:latest` | `src/AccountMovement/Devsu.AccountMovement.Api/Dockerfile` |

El `docker-compose.yml` de la raíz **no construye ya que previamente se construyeron las imágenes**, y solo levanta los dos contenedores de Api. Cada uno lee su configuración con `env_file:` directamente del `.env` real de ese microservicio (el mismo que usa `dotnet run`) — no hay ningún `.env` global ni variables inventadas.

Levantar y verificar:

```powershell
docker compose up -d
curl http://localhost:5046/clientes
curl http://localhost:5004/cuentas
```

Bajar (conservando el volumen de datos de SQL Server):

```powershell
docker compose down
```

En ambos casos (local o Docker), antes de usar las Apis hay que aplicar `SCRIPTS.sql` (raíz del repositorio) contra el SQL Server correspondiente: crea `CustomersDB` y `AccountsDB` con su esquema y carga los datos de ejemplo del enunciado. Ninguna de las dos Api ejecuta migraciones ni `EnsureCreated()` al arrancar, así que es un paso manual.

## Documentación y pruebas de la API (Scalar)

No se adjunta una colección de Postman: cada Api expone su propia documentación interactiva con **Scalar** sobre el OpenAPI que genera ASP.NET Core. Con la Api corriendo (local o en Docker, en entorno `Development`/con OpenAPI habilitado), entrando a `/scalar` se puede ver cada endpoint, su esquema de request/response, y **ejecutar las llamadas directamente desde el navegador**.

| Microservicio | Scalar | Endpoints |
| :--- | :--- | :--- |
| PersonCustomer | `http://localhost:5046/scalar` | `/clientes` (GET, GET/{id}, POST, PUT/{id}, DELETE/{id}) |
| AccountMovement | `http://localhost:5004/scalar` | `/cuentas` (GET, GET/{id}, POST, PUT/{id}) · `/movimientos` (GET, GET/{id}, POST, PUT/{id}) · `/reportes` (GET, `?cliente={id}&fechaInicio={fecha}&fechaFin={fecha}`) |

`/clientes`, `/cuentas`, `/movimientos` y `/reportes` se mantienen en español porque `prueba_tecnica.md` los exige literalmente como contrato de la API; el resto del código (clases, propiedades, JSON en camelCase) está en inglés.

## Arquitectura

Cada microservicio está organizado en **arquitectura hexagonal (Ports & Adapters)**, en 4 capas:

```
Api            -> adaptador de entrada (controllers, manejo global de excepciones, Scalar/OpenAPI)
Application    -> casos de uso + puertos (interfaces): servicios, DTOs, excepciones de aplicación
Domain         -> entidades y reglas de negocio puras, sin dependencias externas
Infrastructure -> adaptadores de salida: EF Core (SQL Server) y el cliente HTTP hacia el otro servicio
```

`Domain` no depende de nada; `Application` depende solo de `Domain`; `Infrastructure` implementa los puertos de `Application`; `Api` es la raíz de composición (inyección de dependencias) y arma todo.
- `AccountMovement` es el único lado "consumidor". Antes de crear una cuenta valida que el cliente exista (`GET /clientes/{id}` contra `PersonCustomer`); en el reporte de estado de cuenta resuelve el nombre del cliente de la misma forma.
- `PersonCustomer` es el lado "proveedor": expone su propio `CustomerDto` por REST y no conoce nada del otro servicio.
- El contrato de esa llamada (`CustomerSummary`) vive en `src/Shared/Devsu.Shared.Contracts`, la única librería que comparten ambos microservicios — y solo `AccountMovement` la referencia (`Application` e `Infrastructure`).

```
devsu/
├── docker-compose.yml
├── SCRIPTS.sql
├── src/
│   ├── Shared/Devsu.Shared.Contracts/          (contrato HTTP compartido: CustomerSummary)
│   ├── PersonCustomer/     (Domain / Application / Infrastructure / Api)
│   └── AccountMovement/    (Domain / Application / Infrastructure / Api)
└── tests/
    ├── PersonCustomer/     (UnitTests / IntegrationTests)
    └── AccountMovement/    (UnitTests / IntegrationTests)
```

## Tecnologías y dependencias

- **.NET 10** / C# (ASP.NET Core Web API, controllers clásicos, sin Minimal APIs)
- **Entity Framework Core 10** sobre **SQL Server 2022**
- **Scalar** como UI de documentación sobre el OpenAPI que genera ASP.NET Core
- **xUnit + Moq + FluentAssertions** para pruebas unitarias, **Microsoft.AspNetCore.Mvc.Testing** para integración
- **Docker** multi-stage (`sdk:10.0` para build/test, `aspnet:10.0` como runtime final) — el build de cada imagen falla si sus pruebas unitarias no pasan

| Paquete | Dónde | Motivo |
| :--- | :--- | :--- |
| `Microsoft.EntityFrameworkCore` / `.SqlServer` / `.Design` | `Infrastructure` de ambos servicios | ORM contra SQL Server y herramientas de diseño |
| `Microsoft.Extensions.Http` | `AccountMovement.Infrastructure` | `IHttpClientFactory` en una *class library* — lo usa `CustomerServiceClient` para llamar a `PersonCustomer` |
| `Microsoft.AspNetCore.OpenApi` / `Microsoft.OpenApi` | `Api` de ambos servicios | Genera el documento OpenAPI que consume Scalar |
| `Scalar.AspNetCore` | `Api` de ambos servicios | UI de documentación/prueba de la API |
| `Microsoft.AspNetCore.JsonPatch.SystemTextJson` | `Api` de ambos servicios | Instalado para `PATCH` con JSON Patch (RFC 6902); **ningún endpoint lo usa todavía** — las actualizaciones son `PUT` con DTO completo |
| `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` | `Api` de ambos servicios | Perfil "Container (Dockerfile)" de Visual Studio; no afecta el build/imagen desde CLI |
| `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `coverlet.collector` | todos los proyectos de test | Framework de pruebas y cobertura |
| `Moq` | `*.UnitTests` | Mocks de puertos para aislar la capa `Application` |
| `FluentAssertions` | `*.UnitTests` | Aserciones legibles |
| `Microsoft.AspNetCore.Mvc.Testing` | `*.IntegrationTests` | `WebApplicationFactory<Program>`: levanta la Api completa en memoria |

Sin dependencias de mensajería ni de `Newtonsoft.Json`: todo el pipeline serializa con `System.Text.Json`.

## Pruebas automatizadas

```powershell
dotnet test tests/PersonCustomer/Devsu.PersonCustomer.UnitTests
dotnet test tests/AccountMovement/Devsu.AccountMovement.UnitTests
dotnet test tests/PersonCustomer/Devsu.PersonCustomer.IntegrationTests
```

## Uso de IA en este proyecto

Parte de este proyecto se desarrolló con asistencia de IA (Claude Code), usada como guía de trabajo — no como "vibe coding". Ninguna implementación, decisión de arquitectura, configuración o documento se aprobó ni se integró sin revisión propia primero.

- **Human in the loop en cada paso**: la IA proponía código, tests, configuración o documentación; la decisión de aceptarlo, pedir cambios o descartarlo fue siempre mía antes de que quedara en el repositorio. Nada se aprueba sin mi intervención.
- **Qué no se comparte con la IA**: el mismo criterio que para un correo o un adjunto — si no lo enviaría por ahí, no lo puse en la IA. Contraseñas, cadenas de conexión reales y cualquier credencial del entorno se mantuvieron siempre en los `.env` locales, no versionados y fuera de lo que se le dio como contexto a la herramienta.
