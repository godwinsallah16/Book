#!/bin/bash
# Simple test script to verify the application works locally

echo "Building the application..."
cd BookStore.API
dotnet build

if [ $? -eq 0 ]; then
    echo "Build successful!"
    
    echo "Testing with minimal environment variables..."
    export ASPNETCORE_ENVIRONMENT=Production
    export ASPNETCORE_URLS=http://+:5000
    export JwtSettings__Secret=ThisIsAVeryLongSecretKeyForJWTTokenGenerationThatShouldBeAtLeast256BitsLong
    export JwtSettings__Issuer=BookStoreAPI
    export JwtSettings__Audience=BookStoreClient
    
    echo "Starting application..."
    timeout 10s dotnet run --no-build
    
    echo "Application test completed."
else
    echo "Build failed!"
    exit 1
fi
