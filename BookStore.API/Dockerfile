# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
EXPOSE 8080
EXPOSE 8081

# Install OpenSSL for SSL certificate generation
USER root
RUN apt-get update && apt-get install -y openssl && rm -rf /var/lib/apt/lists/*
USER app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BookStore.API/BookStore.API.csproj", "BookStore.API/"]
RUN dotnet restore "./BookStore.API/BookStore.API.csproj"
COPY . .
WORKDIR "/src/BookStore.API"
RUN dotnet build "./BookStore.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BookStore.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy SSL certificates
COPY certs/ /app/certs/

# Set environment variables for HTTPS
ENV ASPNETCORE_URLS=https://+:8081;http://+:8080
ENV ASPNETCORE_HTTPS_PORT=8081
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/certs/bookstore-dev.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=bookstore123

ENTRYPOINT ["dotnet", "BookStore.API.dll"]
