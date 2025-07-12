#!/bin/bash
echo "Starting BookStore API..."
echo "Environment: $ASPNETCORE_ENVIRONMENT"
echo "URLs: $ASPNETCORE_URLS"
echo "DATABASE_URL set: $([ -n "$DATABASE_URL" ] && echo "Yes" || echo "No")"
echo "JWT Secret set: $([ -n "$JwtSettings__Secret" ] && echo "Yes" || echo "No")"

# Start the application
exec dotnet BookStore.API.dll
