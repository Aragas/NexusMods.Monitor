FROM mcr.microsoft.com/dotnet/core/runtime:3.1-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["NexusMods.Monitor.Bot.Discord.Host/NexusMods.Monitor.Bot.Discord.Host.csproj", "NexusMods.Monitor.Bot.Discord.Host/"]
COPY ["NexusMods.Monitor.Shared.Host/NexusMods.Monitor.Shared.Host.csproj", "NexusMods.Monitor.Shared.Host/"]
COPY ["NexusMods.Monitor.Bot.Discord.Application/NexusMods.Monitor.Bot.Discord.Application.csproj", "NexusMods.Monitor.Bot.Discord.Application/"]
COPY ["NexusMods.Monitor.Shared.Application/NexusMods.Monitor.Shared.Application.csproj", "NexusMods.Monitor.Shared.Application/"]
COPY ["NexusMods.Monitor.Shared.Domain/NexusMods.Monitor.Shared.Domain.csproj", "NexusMods.Monitor.Shared.Domain/"]
RUN dotnet restore "NexusMods.Monitor.Bot.Discord.Host/NexusMods.Monitor.Bot.Discord.Host.csproj"
COPY . .
WORKDIR "/src/NexusMods.Monitor.Bot.Discord.Host"
RUN dotnet build "NexusMods.Monitor.Bot.Discord.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NexusMods.Monitor.Bot.Discord.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NexusMods.Monitor.Bot.Discord.Host.dll"]