

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=DockerDevelopment
WORKDIR /src
COPY ["mushroomAPI.csproj", "."]
RUN dotnet restore "./mushroomAPI.csproj"
COPY . .
RUN dotnet build "./mushroomAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=DockerDevelopment
RUN dotnet publish "./mushroomAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=DockerDevelopment
ENTRYPOINT ["dotnet", "mushroomAPI.dll"]