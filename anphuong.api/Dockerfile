# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["anphuong.api/anphuong.api.csproj", "anphuong.api/"]
COPY ["anphuong.Core/anphuong.Core.csproj", "anphuong.Core/"]
COPY ["anphuong.Service/anphuong.Service.csproj", "anphuong.Service/"]
COPY ["anphuong.Repository/anphuong.Repository.csproj", "anphuong.Repository/"]
RUN dotnet restore "./anphuong.api/anphuong.api.csproj"
COPY . .
WORKDIR "/src/anphuong.api"
RUN dotnet build "./anphuong.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./anphuong.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "anphuong.api.dll"]