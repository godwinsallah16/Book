# Multi-stage Dockerfile for Render.com deployment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000

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

# Set environment variables for HTTP only (Render handles HTTPS)
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

# Add debugging script to check environment variables
RUN echo '#!/bin/bash' > /app/debug-env.sh && \
    echo 'echo "=== Environment Variables Debug ==="' >> /app/debug-env.sh && \
    echo 'echo "ASPNETCORE_ENVIRONMENT: $ASPNETCORE_ENVIRONMENT"' >> /app/debug-env.sh && \
    echo 'echo "DATABASE_URL exists: $(if [ -n "$DATABASE_URL" ]; then echo "YES"; else echo "NO"; fi)"' >> /app/debug-env.sh && \
    echo 'echo "DATABASE_URL length: ${#DATABASE_URL}"' >> /app/debug-env.sh && \
    echo 'echo "All env vars with DATABASE:"' >> /app/debug-env.sh && \
    echo 'env | grep -i database || echo "No DATABASE env vars found"' >> /app/debug-env.sh && \
    echo 'echo "=== Starting Application ==="' >> /app/debug-env.sh && \
    echo 'dotnet BookStore.API.dll' >> /app/debug-env.sh && \
    chmod +x /app/debug-env.sh

ENTRYPOINT ["/app/debug-env.sh"]
