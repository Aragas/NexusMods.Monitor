# Stage 1: Build application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

WORKDIR /build

COPY [".git/HEAD", ".git/HEAD"]
COPY [".git/config", ".git/config"]
COPY [".git/refs/heads/", ".git/refs/heads/"]
COPY ["build/", "build/"]
COPY ["src/Directory.Build.props", "src/"]
COPY ["src/Directory.Build.targets", "src/"]

COPY ["src/NexusMods.Monitor.Shared.Host/", "src/NexusMods.Monitor.Shared.Host/"]
COPY ["src/NexusMods.Monitor.Shared.Application/", "src/NexusMods.Monitor.Shared.Application/"]
COPY ["src/NexusMods.Monitor.Shared.Infrastructure/", "src/NexusMods.Monitor.Shared.Infrastructure/"]
COPY ["src/NexusMods.Monitor.Shared.Domain/", "src/NexusMods.Monitor.Shared.Domain/"]
COPY ["src/NexusMods.Monitor.Shared.Common/", "src/NexusMods.Monitor.Shared.Common/"]

RUN dotnet build "src/NexusMods.Monitor.Shared.Common/NexusMods.Monitor.Shared.Common.csproj" && \
  dotnet build "src/NexusMods.Monitor.Shared.Domain/NexusMods.Monitor.Shared.Domain.csproj" && \
  dotnet build "src/NexusMods.Monitor.Shared.Infrastructure/NexusMods.Monitor.Shared.Infrastructure.csproj" && \
  dotnet build "src/NexusMods.Monitor.Shared.Application/NexusMods.Monitor.Shared.Application.csproj" && \
  dotnet build "src/NexusMods.Monitor.Shared.Host/NexusMods.Monitor.Shared.Host.csproj"
# Shared Layer End

COPY ["src/NexusMods.Monitor.Shared.Infrastructure.Npgsql/NexusMods.Monitor.Shared.Infrastructure.Npgsql.csproj", "src/NexusMods.Monitor.Shared.Infrastructure.Npgsql/"]
COPY ["src/NexusMods.Monitor.Scraper.Host/NexusMods.Monitor.Scraper.Host.csproj", "src/NexusMods.Monitor.Scraper.Host/"]
COPY ["src/NexusMods.Monitor.Scraper.Application/NexusMods.Monitor.Scraper.Application.csproj", "src/NexusMods.Monitor.Scraper.Application/"]
COPY ["src/NexusMods.Monitor.Scraper.Infrastructure/NexusMods.Monitor.Scraper.Infrastructure.csproj", "src/NexusMods.Monitor.Scraper.Infrastructure/"]
COPY ["src/NexusMods.Monitor.Scraper.Domain/NexusMods.Monitor.Scraper.Domain.csproj", "src/NexusMods.Monitor.Scraper.Domain/"]

RUN dotnet restore "src/NexusMods.Monitor.Scraper.Host/NexusMods.Monitor.Scraper.Host.csproj"

COPY ["src/NexusMods.Monitor.Shared.Infrastructure.Npgsql/", "src/NexusMods.Monitor.Shared.Infrastructure.Npgsql/"]
COPY ["src/NexusMods.Monitor.Scraper.Host/", "src/NexusMods.Monitor.Scraper.Host/"]
COPY ["src/NexusMods.Monitor.Scraper.Application/", "src/NexusMods.Monitor.Scraper.Application/"]
COPY ["src/NexusMods.Monitor.Scraper.Infrastructure/", "src/NexusMods.Monitor.Scraper.Infrastructure/"]
COPY ["src/NexusMods.Monitor.Scraper.Domain/", "src/NexusMods.Monitor.Scraper.Domain/"]

RUN dotnet publish "src/NexusMods.Monitor.Scraper.Host/NexusMods.Monitor.Scraper.Host.csproj" -c Release -o /publish --no-restore


# Stage 2: Copy application artifacts into a smaller runtime environment, which is then used as our final image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime
WORKDIR /publish
COPY --from=build-env /publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "NexusMods.Monitor.Scraper.Host.dll"]