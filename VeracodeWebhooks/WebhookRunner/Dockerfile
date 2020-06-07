#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
ARG NAME

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["WebhookRunner/WebhookRunner.csproj", "WebhookRunner/"]
COPY ["WebhookLogic/WebhookLogic.csproj", "WebhookLogic/"]
COPY ["VeracodeService/VeracodeService.csproj", "VeracodeService/"]
COPY ["DataAccess/DataAccess.csproj", "DataAccess/"]
RUN dotnet restore "WebhookRunner/WebhookRunner.csproj"
COPY . .
WORKDIR "/src/WebhookRunner"
RUN dotnet build "WebhookRunner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebhookRunner.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebhookRunner.dll", $NAME]