FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
WORKDIR /src
COPY ["apartment-meters.API/apartment-meters.API.csproj", "apartment-meters.API/"]
COPY ["apartment-meters.Application/apartment-meters.Application.csproj", "apartment-meters.Application/"]
COPY ["apartment-meters.Domain/apartment-meters.Domain.csproj", "apartment-meters.Domain/"]
COPY ["apartment-meters.Persistence/apartment-meters.Persistence.csproj", "apartment-meters.Persistence/"]
COPY ["apartment-meters.Infrastructure/apartment-meters.Infrastructure.csproj", "apartment-meters.Infrastructure/"]
RUN dotnet restore "apartment-meters.API/apartment-meters.API.csproj" -a $TARGETARCH
COPY . .
WORKDIR "/src/apartment-meters.API"
RUN dotnet build "apartment-meters.API.csproj" -c $BUILD_CONFIGURATION -o /app/build -a $TARGETARCH

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "apartment-meters.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false -a $TARGETARCH

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "apartment-meters.API.dll"]
