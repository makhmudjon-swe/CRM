# Build bosqichi
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# csproj larni avval copy qilamiz (tezroq restore uchun)
COPY *.sln .
COPY src/InventoryManagementService.Web/*.csproj ./src/InventoryManagementService.Web/
COPY src/InventoryManagementService.Domain/*.csproj ./src/InventoryManagementService.Domain/
COPY src/InventoryManagementService.Application/*.csproj ./src/InventoryManagementService.Application/
COPY src/InventoryManagementService.Infrastructure/*.csproj ./src/InventoryManagementService.Infrastructure/

RUN dotnet restore "InventoryManagementService.sln"

# Butun kodni copy va publish
COPY . .
WORKDIR /app/src/InventoryManagementService.Web
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime bosqichi (engil image)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "InventoryManagementService.Web.dll"]