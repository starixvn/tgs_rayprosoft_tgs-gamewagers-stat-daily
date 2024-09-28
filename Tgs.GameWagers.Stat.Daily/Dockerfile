FROM hub.lumigame.com/base/dotnet/core/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameWagers.Stat.Daily/Tgs.GameWagers.Stat.Daily.csproj", "Tgs.GameWagers.Stat.Daily/"]
COPY ["Tgs.GameWagers.Stat.Daily.Common/Tgs.GameWagers.Stat.Daily.Common.csproj", "Tgs.GameWagers.Stat.Daily.Common/"]
COPY ["Tgs.GameWagers.Stat.Daily.Core/Tgs.GameWagers.Stat.Daily.Core.csproj", "Tgs.GameWagers.Stat.Daily.Core/"]
COPY ["Tgs.GameWagers.Stat.Daily.Repositories/Tgs.GameWagers.Stat.Daily.Repositories.csproj", "Tgs.GameWagers.Stat.Daily.Repositories/"]
RUN dotnet restore "./Tgs.GameWagers.Stat.Daily/./Tgs.GameWagers.Stat.Daily.csproj"
COPY . .
WORKDIR "/src/Tgs.GameWagers.Stat.Daily"
RUN dotnet build "./Tgs.GameWagers.Stat.Daily.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameWagers.Stat.Daily.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM hub.lumigame.com/base/dotnet/core/aspnet:8.0-alpine-24h1.1 AS base
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameWagers.Stat.Daily.dll"]