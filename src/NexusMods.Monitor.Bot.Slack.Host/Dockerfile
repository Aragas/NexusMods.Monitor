FROM mcr.microsoft.com/dotnet/core/runtime:3.1-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["NexusMods.Monitor.Bot.Slack.Host/NexusMods.Monitor.Bot.Slack.Host.csproj", "NexusMods.Monitor.Bot.Slack.Host/"]
COPY ["NexusMods.Monitor.Shared.Host/NexusMods.Monitor.Shared.Host.csproj", "NexusMods.Monitor.Shared.Host/"]
COPY ["NexusMods.Monitor.Bot.Slack.Application/NexusMods.Monitor.Bot.Slack.Application.csproj", "NexusMods.Monitor.Bot.Slack.Application/"]
COPY ["NexusMods.Monitor.Shared.Domain/NexusMods.Monitor.Shared.Domain.csproj", "NexusMods.Monitor.Shared.Domain/"]
COPY ["NexusMods.Monitor.Shared.Application/NexusMods.Monitor.Shared.Application.csproj", "NexusMods.Monitor.Shared.Application/"]
RUN dotnet restore "NexusMods.Monitor.Bot.Slack.Host/NexusMods.Monitor.Bot.Slack.Host.csproj"
COPY . .
WORKDIR "/src/NexusMods.Monitor.Bot.Slack.Host"
RUN dotnet build "NexusMods.Monitor.Bot.Slack.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NexusMods.Monitor.Bot.Slack.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NexusMods.Monitor.Bot.Slack.Host.dll"]