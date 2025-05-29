FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8082
EXPOSE 8083

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["MagazinEAPI/MagazinEAPI.csproj", "MagazinEAPI/"]
RUN dotnet restore "./MagazinEAPI/MagazinEAPI.csproj"
COPY . .
WORKDIR "/src/MagazinEAPI"
RUN dotnet build "./MagazinEAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./MagazinEAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MagazinEAPI.dll"]
