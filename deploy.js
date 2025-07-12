#!/usr/bin/env node

const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');

// Colors for console output
const colors = {
    reset: '\x1b[0m',
    bright: '\x1b[1m',
    red: '\x1b[31m',
    green: '\x1b[32m',
    yellow: '\x1b[33m',
    blue: '\x1b[34m',
    magenta: '\x1b[35m',
    cyan: '\x1b[36m'
};

function log(message, color = colors.reset) {
    console.log(`${color}${message}${colors.reset}`);
}

function execCommand(command, description) {
    log(`\n${description}...`, colors.cyan);
    try {
        const result = execSync(command, { encoding: 'utf8', stdio: 'inherit' });
        log(`✓ ${description} completed`, colors.green);
        return result;
    } catch (error) {
        log(`✗ ${description} failed: ${error.message}`, colors.red);
        throw error;
    }
}

function createRenderYaml() {
    const renderYaml = `# Render.com deployment configuration
services:
  # Backend API Service
  - type: web
    name: bookstore-api
    env: node
    buildCommand: |
      cd BookStore.API
      dotnet publish -c Release -o out
    startCommand: |
      cd BookStore.API/out
      dotnet BookStore.API.dll
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
      - key: ASPNETCORE_URLS
        value: http://0.0.0.0:10000
      - key: ConnectionStrings__DefaultConnection
        fromDatabase:
          name: bookstore-db
          property: connectionString
      - key: JwtSettings__SecretKey
        generateValue: true
      - key: JwtSettings__Issuer
        value: BookStore.API
      - key: JwtSettings__Audience
        value: BookStore.Client
      - key: JwtSettings__ExpirationHours
        value: "24"
    healthCheckPath: /health
    
  # Frontend Static Site
  - type: static
    name: bookstore-frontend
    buildCommand: |
      cd BookStore-frontend
      npm install
      npm run build
    publishPath: BookStore-frontend/dist
    envVars:
      - key: VITE_API_BASE_URL
        value: https://bookstore-api.onrender.com/api
      - key: VITE_APP_NAME
        value: BookStore

# Database
databases:
  - name: bookstore-db
    databaseName: bookstore
    user: bookstore_user
`;

    fs.writeFileSync('render.yaml', renderYaml);
    log('✓ Created render.yaml configuration', colors.green);
}

function createDockerfileForRender() {
    const dockerfile = `# Multi-stage Dockerfile for Render deployment
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
RUN dotnet publish "BookStore.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookStore.API.dll"]
`;

    fs.writeFileSync('Dockerfile.render', dockerfile);
    log('✓ Created Dockerfile.render for backend', colors.green);
}

function createRenderDeployScript() {
    const deployScript = `#!/bin/bash
# Render.com deployment script

echo "🚀 Starting Render.com deployment..."

# Check if render.yaml exists
if [ ! -f "render.yaml" ]; then
    echo "❌ render.yaml not found. Please run 'node deploy.js' first."
    exit 1
fi

# Push to GitHub (Render deploys from GitHub)
echo "📤 Pushing to GitHub..."
git add .
git commit -m "Deploy to Render.com - $(date)"
git push origin main

echo "✅ Code pushed to GitHub!"
echo "🔗 Go to https://render.com to complete deployment:"
echo "   1. Connect your GitHub repository"
echo "   2. Render will automatically detect render.yaml"
echo "   3. Configure environment variables"
echo "   4. Deploy!"

echo ""
echo "📚 Your services will be available at:"
echo "   • API: https://bookstore-api.onrender.com"
echo "   • Frontend: https://bookstore-frontend.onrender.com"
echo "   • Health Check: https://bookstore-api.onrender.com/health"
echo "   • API Docs: https://bookstore-api.onrender.com/swagger"
`;

    fs.writeFileSync('deploy-render.sh', deployScript);
    execSync('chmod +x deploy-render.sh');
    log('✓ Created deploy-render.sh script', colors.green);
}

function updatePackageJson() {
    const packageJsonPath = path.join(__dirname, 'package.json');
    let packageJson = {};
    
    if (fs.existsSync(packageJsonPath)) {
        packageJson = JSON.parse(fs.readFileSync(packageJsonPath, 'utf8'));
    }

    packageJson.scripts = packageJson.scripts || {};
    packageJson.scripts['deploy:render'] = 'node deploy.js';
    packageJson.scripts['deploy:render:manual'] = './deploy-render.sh';

    fs.writeFileSync(packageJsonPath, JSON.stringify(packageJson, null, 2));
    log('✓ Updated package.json with deployment scripts', colors.green);
}

function main() {
    log('🚀 BookStore Render.com Deployment Setup', colors.bright + colors.green);
    log('==========================================', colors.green);

    try {
        // Create deployment configurations
        createRenderYaml();
        createDockerfileForRender();
        createRenderDeployScript();
        updatePackageJson();

        log('\n🎉 Render.com deployment setup completed!', colors.bright + colors.green);
        log('\n📋 Next Steps:', colors.yellow);
        log('1. Commit these changes to your repository:', colors.white);
        log('   git add .', colors.cyan);
        log('   git commit -m "Add Render.com deployment configuration"', colors.cyan);
        log('   git push origin main', colors.cyan);
        
        log('\n2. Go to https://render.com and:', colors.white);
        log('   • Sign up/Login with your GitHub account', colors.cyan);
        log('   • Click "New +" and select "Blueprint"', colors.cyan);
        log('   • Connect your GitHub repository', colors.cyan);
        log('   • Render will automatically detect render.yaml', colors.cyan);
        log('   • Configure environment variables if needed', colors.cyan);
        log('   • Click "Apply" to deploy!', colors.cyan);

        log('\n🔗 Your deployed application will be available at:', colors.yellow);
        log('   • API: https://bookstore-api.onrender.com', colors.cyan);
        log('   • Frontend: https://bookstore-frontend.onrender.com', colors.cyan);
        log('   • API Documentation: https://bookstore-api.onrender.com/swagger', colors.cyan);

        log('\n⚡ Alternative: Quick deploy script:', colors.yellow);
        log('   ./deploy-render.sh', colors.cyan);

        log('\n📚 Documentation:', colors.yellow);
        log('   • Render.com: https://render.com/docs', colors.cyan);
        log('   • Blueprint deployments: https://render.com/docs/blueprint-spec', colors.cyan);

    } catch (error) {
        log(`\n❌ Deployment setup failed: ${error.message}`, colors.red);
        process.exit(1);
    }
}

// Run the main function
if (require.main === module) {
    main();
}

module.exports = { main };
