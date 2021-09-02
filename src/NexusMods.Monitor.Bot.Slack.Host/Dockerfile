# Stage 1: Build application
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-env

WORKDIR /build

COPY ["src/NexusMods.Monitor.Bot.Slack.Host/NexusMods.Monitor.Bot.Slack.Host.csproj", "NexusMods.Monitor.Bot.Slack.Host/"]
COPY ["src/NexusMods.Monitor.Bot.Slack.Application/NexusMods.Monitor.Bot.Slack.Application.csproj", "NexusMods.Monitor.Bot.Slack.Application/"]
COPY ["src/NexusMods.Monitor.Shared.Host/NexusMods.Monitor.Shared.Host.csproj", "NexusMods.Monitor.Shared.Host/"]
COPY ["src/NexusMods.Monitor.Shared.Application/NexusMods.Monitor.Shared.Application.csproj", "NexusMods.Monitor.Shared.Application/"]
COPY ["src/NexusMods.Monitor.Shared.Infrastructure/NexusMods.Monitor.Shared.Infrastructure.csproj", "NexusMods.Monitor.Shared.Infrastructure/"]
COPY ["src/NexusMods.Monitor.Shared.Domain/NexusMods.Monitor.Shared.Domain.csproj", "NexusMods.Monitor.Shared.Domain/"]

RUN dotnet restore "NexusMods.Monitor.Bot.Slack.Host/NexusMods.Monitor.Bot.Slack.Host.csproj"

COPY ["src/NexusMods.Monitor.Bot.Slack.Host/", "NexusMods.Monitor.Bot.Slack.Host/"]
COPY ["src/NexusMods.Monitor.Bot.Slack.Application/", "NexusMods.Monitor.Bot.Slack.Application/"]
COPY ["src/NexusMods.Monitor.Shared.Host/", "NexusMods.Monitor.Shared.Host/"]
COPY ["src/NexusMods.Monitor.Shared.Application/", "NexusMods.Monitor.Shared.Application/"]
COPY ["src/NexusMods.Monitor.Shared.Infrastructure/", "NexusMods.Monitor.Shared.Infrastructure/"]
COPY ["src/NexusMods.Monitor.Shared.Domain/", "NexusMods.Monitor.Shared.Domain/"]

RUN /bin/ash -c 'set -ex && \
    ARCH=`uname -m` && \
    if [ "$ARCH" == "amd64" ] || [ "$ARCH" == "x86_64" ]; then \
       echo "x86_64" && \
       dotnet publish "NexusMods.Monitor.Bot.Slack.Host/NexusMods.Monitor.Bot.Slack.Host.csproj" -c Release -o ./output -r alpine-x64 --self-contained true; \
    elif [ "$ARCH" == "aarch64" ] || [ "$ARCH" == "arm64" ]; then \
       echo "aarch64" && \
       dotnet publish "NexusMods.Monitor.Bot.Slack.Host/NexusMods.Monitor.Bot.Slack.Host.csproj" -c Release -o ./output -r alpine-arm --self-contained true; \
    else \
       echo "unknown arch" && \
       dotnet publish "NexusMods.Monitor.Bot.Slack.Host/NexusMods.Monitor.Bot.Slack.Host.csproj" -c Release -o ./output -r alpine --self-contained true; \
    fi'

# Stage 2: Copy application artifacts into a smaller runtime environment, which is then used as our final image
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine

WORKDIR /app

COPY --from=build-env /build/output .

ENTRYPOINT ["./NexusMods.Monitor.Bot.Slack.Host"]