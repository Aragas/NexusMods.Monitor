FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["NexusMods.Monitor.Subscriptions.API/NexusMods.Monitor.Subscriptions.API.csproj", "NexusMods.Monitor.Subscriptions.API/"]
COPY ["NexusMods.Monitor.Shared.Host/NexusMods.Monitor.Shared.Host.csproj", "NexusMods.Monitor.Shared.Host/"]
COPY ["NexusMods.Monitor.Subscriptions.Application/NexusMods.Monitor.Subscriptions.Application.csproj", "NexusMods.Monitor.Subscriptions.Application/"]
COPY ["NexusMods.Monitor.Subscriptions.Infrastructure/NexusMods.Monitor.Subscriptions.Infrastructure.csproj", "NexusMods.Monitor.Subscriptions.Infrastructure/"]
COPY ["NexusMods.Monitor.Shared.Infrastructure/NexusMods.Monitor.Shared.Infrastructure.csproj", "NexusMods.Monitor.Shared.Infrastructure/"]
COPY ["NexusMods.Monitor.Shared.Domain/NexusMods.Monitor.Shared.Domain.csproj", "NexusMods.Monitor.Shared.Domain/"]
COPY ["NexusMods.Monitor.Subscriptions.Domain/NexusMods.Monitor.Subscriptions.Domain.csproj", "NexusMods.Monitor.Subscriptions.Domain/"]
RUN dotnet restore "NexusMods.Monitor.Subscriptions.API/NexusMods.Monitor.Subscriptions.API.csproj"
COPY . .
WORKDIR "/src/NexusMods.Monitor.Subscriptions.API"
RUN dotnet build "NexusMods.Monitor.Subscriptions.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NexusMods.Monitor.Subscriptions.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NexusMods.Monitor.Subscriptions.API.dll"]