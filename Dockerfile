FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy project files first so restore can be cached
COPY Shop.Api/Shop.Api.csproj Shop.Api/
COPY Shop.Application/Shop.Application.csproj Shop.Application/
COPY Shop.Domain/Shop.Domain.csproj Shop.Domain/
COPY Shop.Infrastructure/Shop.Infrastructure.csproj Shop.Infrastructure/

RUN dotnet restore Shop.Api/Shop.Api.csproj

# Copy source code
COPY Shop.Api/ Shop.Api/
COPY Shop.Application/ Shop.Application/
COPY Shop.Domain/ Shop.Domain/
COPY Shop.Infrastructure/ Shop.Infrastructure/

WORKDIR /src/Shop.Api

RUN dotnet publish Shop.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Shop.Api.dll"]