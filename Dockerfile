# ================================
# BOSQICH 1: Build
# ================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# csproj fayllarni avval copy qilamiz (Docker layer cache uchun)
COPY InventoryManagementService.sln .
COPY src/InventoryManagementService.Web/*.csproj                     ./src/InventoryManagementService.Web/
COPY src/InventoryManagementService.Application/*.csproj             ./src/InventoryManagementService.Application/
COPY src/InventoryManagementService.Infrastructure/*.csproj          ./src/InventoryManagementService.Infrastructure/
COPY src/WholesaleCRM.Domain/*.csproj                                ./src/WholesaleCRM.Domain/

# NuGet package restore
RUN dotnet restore "InventoryManagementService.sln"

# Butun source kodni copy qilamiz
COPY . .

# Release build va publish
WORKDIR /app/src/InventoryManagementService.Web
RUN dotnet publish -c Release -o /app/publish --no-restore

# ================================
# BOSQICH 2: Runtime (engil image)
# ================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Publish qilingan fayllarni copy qilamiz
COPY --from=build /app/publish .

# Port ochish
EXPOSE 8080

# Environment o'zgaruvchilari (AWS da override qilinadi)
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "InventoryManagementService.Web.dll"]