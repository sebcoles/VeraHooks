FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS base
WORKDIR /app
EXPOSE 80 
EXPOSE 443
RUN curl -sL https://deb.nodesource.com/setup_10.x |  bash -
RUN apt-get install -y nodejs

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
RUN curl -sL https://deb.nodesource.com/setup_10.x |  bash -
RUN apt-get install -y nodejs
WORKDIR /src
COPY ["React/React.csproj", "React/"]
COPY ["WebhookLogic/WebhookLogic.csproj", "WebhookLogic/"]
COPY ["VeracodeService/VeracodeService.csproj", "VeracodeService/"]
COPY ["DataAccess/DataAccess.csproj", "DataAccess/"]
RUN dotnet restore "React/React.csproj"
COPY . .
WORKDIR "/src/React"
RUN dotnet build "React.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "React.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "React.dll"]