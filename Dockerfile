ARG DOTNET_VERSION=9.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS restore
WORKDIR /src

COPY Directory.Build.props ./
COPY Payments.Recurring.sln ./
COPY src/Payments.Api/Payments.Api.csproj src/Payments.Api/
COPY src/Payments.Core/Payments.Core.csproj src/Payments.Core/
COPY tests/Payments.Core.Tests/Payments.Core.Tests.csproj tests/Payments.Core.Tests/
COPY tests/Payments.Api.IntegrationTests/Payments.Api.IntegrationTests.csproj tests/Payments.Api.IntegrationTests/

RUN dotnet restore Payments.Recurring.sln

COPY . .

FROM restore AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish src/Payments.Api/Payments.Api.csproj \
    -c ${BUILD_CONFIGURATION} \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS production
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
COPY --from=publish /app/publish ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "Payments.Api.dll"]

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS development
WORKDIR /src
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_USE_POLLING_FILE_WATCHER=1

COPY Directory.Build.props ./
COPY Payments.Recurring.sln ./
COPY src/Payments.Api/Payments.Api.csproj src/Payments.Api/
COPY src/Payments.Core/Payments.Core.csproj src/Payments.Core/
COPY tests/Payments.Core.Tests/Payments.Core.Tests.csproj tests/Payments.Core.Tests/
COPY tests/Payments.Api.IntegrationTests/Payments.Api.IntegrationTests.csproj tests/Payments.Api.IntegrationTests/
RUN dotnet restore Payments.Recurring.sln

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl unzip \
    && rm -rf /var/lib/apt/lists/*
RUN curl -sSL https://aka.ms/getvsdbgsh -o getvsdbg.sh \
    && bash ./getvsdbg.sh -v latest -l /vsdbg \
    && rm getvsdbg.sh

EXPOSE 8080
ENTRYPOINT ["dotnet", "watch", "--project", "src/Payments.Api/Payments.Api.csproj", "run", "--no-launch-profile", "--urls", "http://0.0.0.0:8080"]
