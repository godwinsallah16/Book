# Quick Deployment Script for BookStore
# This script will help you deploy your application quickly

Write-Host "BookStore Deployment Helper" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green

# Check if git is clean
$gitStatus = git status --porcelain
if ($gitStatus) {
    Write-Host "You have uncommitted changes. Please commit them first." -ForegroundColor Yellow
    git status
    $continue = Read-Host "Do you want to continue anyway? (y/N)"
    if ($continue -ne "y") {
        exit
    }
}

Write-Host ""
Write-Host "Choose your deployment option:" -ForegroundColor Cyan
Write-Host "1. Railway (Recommended - Easy setup)" -ForegroundColor White
Write-Host "2. GitHub Pages only (Frontend)" -ForegroundColor White
Write-Host "3. Docker (Local/VPS)" -ForegroundColor White
Write-Host "4. Azure App Service" -ForegroundColor White
Write-Host ""

$choice = Read-Host "Enter your choice (1-4)"

switch ($choice) {
    "1" {
        Write-Host "Deploying to Railway..." -ForegroundColor Green
        
        # Check if Railway CLI is installed
        try {
            railway --version | Out-Null
            Write-Host "Railway CLI found" -ForegroundColor Green
        }
        catch {
            Write-Host "Railway CLI not found. Installing..." -ForegroundColor Yellow
            npm install -g @railway/cli
        }
        
        # Deploy to Railway
        Write-Host "Logging into Railway..." -ForegroundColor Cyan
        railway login
        
        Write-Host "Initializing Railway project..." -ForegroundColor Cyan
        railway init
        
        Write-Host "Deploying to Railway..." -ForegroundColor Cyan
        railway up
        
        Write-Host "Backend deployed to Railway!" -ForegroundColor Green
        Write-Host "Don't forget to:" -ForegroundColor Yellow
        Write-Host "   1. Set environment variables in Railway dashboard" -ForegroundColor White
        Write-Host "   2. Update VITE_API_BASE_URL in GitHub secrets" -ForegroundColor White
        Write-Host "   3. Push to main branch to deploy frontend" -ForegroundColor White
    }
    
    "2" {
        Write-Host "Deploying to GitHub Pages (Frontend only)..." -ForegroundColor Green
        
        # Push to main branch
        Write-Host "Pushing to main branch..." -ForegroundColor Cyan
        git add .
        git commit -m "Deploy to GitHub Pages"
        git push origin main
        
        Write-Host "Frontend deployment triggered!" -ForegroundColor Green
        Write-Host "Your frontend will be available at:" -ForegroundColor Yellow
        Write-Host "   https://godwinsallah16.github.io/Book/" -ForegroundColor White
        Write-Host "Note: You still need to deploy your backend separately" -ForegroundColor Yellow
    }
    
    "3" {
        Write-Host "Deploying with Docker..." -ForegroundColor Green
        
        # Check if Docker is running
        try {
            docker --version | Out-Null
            Write-Host "Docker found" -ForegroundColor Green
        }
        catch {
            Write-Host "Docker not found. Please install Docker first." -ForegroundColor Red
            exit
        }
        
        # Create .env file if not exists
        if (!(Test-Path ".env")) {
            Write-Host "Creating .env file..." -ForegroundColor Cyan
            @"
DB_PASSWORD=BookStore123!
GITHUB_REPOSITORY=godwinsallah16/Book
JWT_SECRET=BookStore-Super-Secret-Key-For-JWT-Tokens-2024-Must-Be-At-Least-256-Bits-Long
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-app-password
SMTP_FROM_EMAIL=noreply@bookstore.com
FRONTEND_URL=http://localhost:3000
"@ | Out-File -FilePath ".env" -Encoding UTF8
            Write-Host "Please update the .env file with your actual email credentials" -ForegroundColor Yellow
        }
        
        # Deploy with Docker Compose
        Write-Host "Starting Docker deployment..." -ForegroundColor Cyan
        docker-compose -f docker-compose.prod.yml up -d
        
        Write-Host "Docker deployment started!" -ForegroundColor Green
        Write-Host "Your application will be available at:" -ForegroundColor Yellow
        Write-Host "   API: http://localhost:8080" -ForegroundColor White
        Write-Host "   Frontend: http://localhost:3000" -ForegroundColor White
    }
    
    "4" {
        Write-Host "Deploying to Azure..." -ForegroundColor Green
        
        # Check if Azure CLI is installed
        try {
            az --version | Out-Null
            Write-Host "Azure CLI found" -ForegroundColor Green
        }
        catch {
            Write-Host "Azure CLI not found. Please install Azure CLI first." -ForegroundColor Red
            Write-Host "Download from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli" -ForegroundColor White
            exit
        }
        
        Write-Host "Running Azure deployment script..." -ForegroundColor Cyan
        ./deploy-azure.ps1
        
        Write-Host "Azure deployment completed!" -ForegroundColor Green
    }
    
    default {
        Write-Host "Invalid choice. Please run the script again." -ForegroundColor Red
        exit
    }
}

Write-Host ""
Write-Host "Deployment process completed!" -ForegroundColor Green
Write-Host "Check DEPLOYMENT.md for detailed instructions" -ForegroundColor Cyan
Write-Host "If you encounter issues, check the troubleshooting section" -ForegroundColor Yellow
