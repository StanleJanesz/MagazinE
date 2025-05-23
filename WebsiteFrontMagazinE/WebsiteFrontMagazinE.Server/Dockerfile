#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 5173

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS with-node
RUN apt-get update
RUN apt-get install curl
RUN curl -sL https://deb.nodesource.com/setup_20.x | bash
RUN apt-get -y install nodejs


FROM with-node AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebsiteFrontMagazinE/websitefrontmagazine.client/websitefrontmagazine.client.esproj", "WebsiteFrontMagazinE/websitefrontmagazine.client/"]
COPY . .
COPY WebsiteFrontMagazinE/websitefrontmagazine.client/src/assets ./src/assets


WORKDIR /src/WebsiteFrontMagazinE/websitefrontmagazine.client
RUN npm install
RUN npm run build
RUN npm install -g serve
EXPOSE 5173
CMD ["npm", "run", "dev", "--", "--host"]




