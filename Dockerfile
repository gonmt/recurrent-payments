ARG DOTNET_VERSION=9.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS restore
WORKDIR /src

COPY Directory.Build.props ./
COPY Archetype.sln ./
COPY src/Archetype.Api/Archetype.Api.csproj src/Archetype.Api/
COPY src/Archetype.Core/Archetype.Core.csproj src/Archetype.Core/
COPY tests/Archetype.Core.Tests/Archetype.Core.Tests.csproj tests/Archetype.Core.Tests/
COPY tests/Archetype.Api.IntegrationTests/Archetype.Api.IntegrationTests.csproj tests/Archetype.Api.IntegrationTests/

RUN dotnet restore Archetype.sln

COPY . .

FROM restore AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish src/Archetype.Api/Archetype.Api.csproj \
    -c ${BUILD_CONFIGURATION} \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS production
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
COPY --from=publish /app/publish ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "Archetype.Api.dll"]

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS development
WORKDIR /src
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_USE_POLLING_FILE_WATCHER=1

COPY Directory.Build.props ./
COPY Archetype.sln ./
COPY src/Archetype.Api/Archetype.Api.csproj src/Archetype.Api/
COPY src/Archetype.Core/Archetype.Core.csproj src/Archetype.Core/
COPY tests/Archetype.Core.Tests/Archetype.Core.Tests.csproj tests/Archetype.Core.Tests/
COPY tests/Archetype.Api.IntegrationTests/Archetype.Api.IntegrationTests.csproj tests/Archetype.Api.IntegrationTests/
RUN dotnet restore Archetype.sln

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl unzip \
    && rm -rf /var/lib/apt/lists/*
RUN curl -sSL https://aka.ms/getvsdbgsh -o getvsdbg.sh \
    && bash ./getvsdbg.sh -v latest -l /vsdbg \
    && rm getvsdbg.sh

EXPOSE 8080
ENTRYPOINT ["dotnet", "watch", "--project", "src/Archetype.Api/Archetype.Api.csproj", "run", "--no-launch-profile", "--urls", "http://0.0.0.0:8080"]
