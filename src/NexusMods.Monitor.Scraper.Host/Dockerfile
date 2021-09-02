FROM mcr.microsoft.com/dotnet/runtime:5.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["NexusMods.Monitor.Scraper.Host/NexusMods.Monitor.Scraper.Host.csproj", "NexusMods.Monitor.Scraper.Host/"]
COPY ["NexusMods.Monitor.Shared.Host/NexusMods.Monitor.Shared.Host.csproj", "NexusMods.Monitor.Shared.Host/"]
COPY ["NexusMods.Monitor.Scraper.Application/NexusMods.Monitor.Scraper.Application.csproj", "NexusMods.Monitor.Scraper.Application/"]
COPY ["NexusMods.Monitor.Shared.Application/NexusMods.Monitor.Shared.Application.csproj", "NexusMods.Monitor.Shared.Application/"]
COPY ["NexusMods.Monitor.Scraper.Infrastructure/NexusMods.Monitor.Scraper.Infrastructure.csproj", "NexusMods.Monitor.Scraper.Infrastructure/"]
COPY ["NexusMods.Monitor.Scraper.Domain/NexusMods.Monitor.Scraper.Domain.csproj", "NexusMods.Monitor.Scraper.Domain/"]
COPY ["NexusMods.Monitor.Shared.Domain/NexusMods.Monitor.Shared.Domain.csproj", "NexusMods.Monitor.Shared.Domain/"]
COPY ["NexusMods.Monitor.Shared.Infrastructure/NexusMods.Monitor.Shared.Infrastructure.csproj", "NexusMods.Monitor.Shared.Infrastructure/"]
RUN dotnet restore "NexusMods.Monitor.Scraper.Host/NexusMods.Monitor.Scraper.Host.csproj"
COPY . .
WORKDIR "/src/NexusMods.Monitor.Scraper.Host"
RUN dotnet build "NexusMods.Monitor.Scraper.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NexusMods.Monitor.Scraper.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NexusMods.Monitor.Scraper.Host.dll"]