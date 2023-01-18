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

COPY ["src/NexusMods.Monitor.Bot.Discord.Host/NexusMods.Monitor.Bot.Discord.Host.csproj", "src/NexusMods.Monitor.Bot.Discord.Host/"]
COPY ["src/NexusMods.Monitor.Bot.Discord.Application/NexusMods.Monitor.Bot.Discord.Application.csproj", "src/NexusMods.Monitor.Bot.Discord.Application/"]

RUN dotnet restore "src/NexusMods.Monitor.Bot.Discord.Host/NexusMods.Monitor.Bot.Discord.Host.csproj"

COPY ["src/NexusMods.Monitor.Bot.Discord.Host/", "src/NexusMods.Monitor.Bot.Discord.Host/"]
COPY ["src/NexusMods.Monitor.Bot.Discord.Application/", "src/NexusMods.Monitor.Bot.Discord.Application/"]

RUN dotnet publish "src/NexusMods.Monitor.Bot.Discord.Host/NexusMods.Monitor.Bot.Discord.Host.csproj" -c Release -o /publish


# Stage 2: Copy application artifacts into a smaller runtime environment, which is then used as our final image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime
WORKDIR /publish
COPY --from=build-env /publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "NexusMods.Monitor.Bot.Discord.Host.dll"]