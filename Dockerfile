# Multi-stage Dockerfile for Render.com deployment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000

# Install curl for health checks (optional)
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BookStore.API/BookStore.API.csproj", "BookStore.API/"]
RUN dotnet restore "BookStore.API/BookStore.API.csproj"
COPY . .
WORKDIR "/src/BookStore.API"
RUN dotnet build "BookStore.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookStore.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Set environment variables for Render deployment
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

# Default JWT settings (will be overridden by Render environment variables)
ENV JwtSettings__Secret=ThisIsAVeryLongSecretKeyForJWTTokenGenerationThatShouldBeAtLeast256BitsLong
ENV JwtSettings__Issuer=BookStoreAPI
ENV JwtSettings__Audience=BookStoreClient
ENV JwtSettings__ExpiryInHours=24

# Disable HTTPS redirection for Render (Render handles SSL termination)
ENV ASPNETCORE_HTTPS_PORT=""

# Set timezone
ENV TZ=UTC

# Use exec form to ensure proper signal handling
ENTRYPOINT ["dotnet", "BookStore.API.dll"]
