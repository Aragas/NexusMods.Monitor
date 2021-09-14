# Stage 1: Build application
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-env

WORKDIR /build

COPY [".git/HEAD", ".git/HEAD"]
COPY [".git/config", ".git/config"]
COPY [".git/refs/heads/master", ".git/refs/heads/master"]
COPY ["build/", "build/"]
COPY ["src/Directory.Build.props", "src/"]
COPY ["src/Directory.Build.targets", "src/"]

COPY ["src/NexusMods.Monitor.Shared.API/", "src/NexusMods.Monitor.Shared.API/"]
COPY ["src/NexusMods.Monitor.Shared.Host/", "src/NexusMods.Monitor.Shared.Host/"]
COPY ["src/NexusMods.Monitor.Shared.Application/", "src/NexusMods.Monitor.Shared.Application/"]
COPY ["src/NexusMods.Monitor.Shared.Infrastructure/", "src/NexusMods.Monitor.Shared.Infrastructure/"]
COPY ["src/NexusMods.Monitor.Shared.Domain/", "src/NexusMods.Monitor.Shared.Domain/"]

RUN dotnet build "src/NexusMods.Monitor.Shared.Domain/NexusMods.Monitor.Shared.Domain.csproj" && \
  dotnet build "src/NexusMods.Monitor.Shared.Infrastructure/NexusMods.Monitor.Shared.Infrastructure.csproj" && \
  dotnet build "src/NexusMods.Monitor.Shared.Application/NexusMods.Monitor.Shared.Application.csproj" && \
  dotnet build "src/NexusMods.Monitor.Shared.Host/NexusMods.Monitor.Shared.Host.csproj" && \
  dotnet build "src/NexusMods.Monitor.Shared.API/NexusMods.Monitor.Shared.API.csproj"
# Shared Layer End

COPY ["src/NexusMods.Monitor.Subscriptions.API/NexusMods.Monitor.Subscriptions.API.csproj", "src/NexusMods.Monitor.Subscriptions.API/"]
COPY ["src/NexusMods.Monitor.Subscriptions.Application/NexusMods.Monitor.Subscriptions.Application.csproj", "src/NexusMods.Monitor.Subscriptions.Application/"]
COPY ["src/NexusMods.Monitor.Subscriptions.Infrastructure/NexusMods.Monitor.Subscriptions.Infrastructure.csproj", "src/NexusMods.Monitor.Subscriptions.Infrastructure/"]
COPY ["src/NexusMods.Monitor.Subscriptions.Domain/NexusMods.Monitor.Subscriptions.Domain.csproj", "src/NexusMods.Monitor.Subscriptions.Domain/"]

RUN dotnet restore "src/NexusMods.Monitor.Subscriptions.API/NexusMods.Monitor.Subscriptions.API.csproj"

COPY ["src/NexusMods.Monitor.Subscriptions.API/", "src/NexusMods.Monitor.Subscriptions.API/"]
COPY ["src/NexusMods.Monitor.Subscriptions.Application/", "src/NexusMods.Monitor.Subscriptions.Application/"]
COPY ["src/NexusMods.Monitor.Subscriptions.Infrastructure/", "src/NexusMods.Monitor.Subscriptions.Infrastructure/"]
COPY ["src/NexusMods.Monitor.Subscriptions.Domain/", "src/NexusMods.Monitor.Subscriptions.Domain/"]

RUN /bin/ash -c 'set -ex && \
    ARCH=`uname -m` && \
    if [[ "$ARCH" == "amd64" || "$ARCH" == "x86_64" ]]; then \
       echo "x86_64" && \
       #dotnet publish "src/NexusMods.Monitor.Subscriptions.API/NexusMods.Monitor.Subscriptions.API.csproj" -c Release -o ./output --os alpine --arch x64 --self-contained true /p:PublishReadyToRun=true /p:PublishReadyToRunShowWarnings=true; \
       dotnet publish "src/NexusMods.Monitor.Subscriptions.API/NexusMods.Monitor.Subscriptions.API.csproj" -c Release -o ./output --os alpine --arch x64 --self-contained true; \
    elif [[ "$ARCH" == "aarch64" || "$ARCH" == "arm64" ]]; then \
       echo "aarch64" && \
       dotnet publish "src/NexusMods.Monitor.Subscriptions.API/NexusMods.Monitor.Subscriptions.API.csproj" -c Release -o ./output --os alpine --arch arm64 --self-contained true; \
    else \
       echo "unknown arch" && \
       dotnet publish "src/NexusMods.Monitor.Subscriptions.API/NexusMods.Monitor.Subscriptions.API.csproj" -c Release -o ./output --os alpine --arch $ARCH --self-contained true; \
    fi'

# Stage 2: Copy application artifacts into a smaller runtime environment, which is then used as our final image
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine

WORKDIR /app

COPY --from=build-env /build/output .

EXPOSE 80
ENTRYPOINT ["./NexusMods.Monitor.Subscriptions.API"]