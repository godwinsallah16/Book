# Azure App Service deployment script
# Run this in PowerShell with Azure CLI installed

# Login to Azure
az login

# Create resource group
az group create --name bookstore-rg --location "East US"

# Create App Service plan
az appservice plan create --name bookstore-plan --resource-group bookstore-rg --sku F1 --is-linux

# Create web app
az webapp create --resource-group bookstore-rg --plan bookstore-plan --name bookstore-api-$(Get-Random) --runtime "DOTNET:8.0"

# Set connection strings and app settings
az webapp config appsettings set --resource-group bookstore-rg --name bookstore-api-$(Get-Random) --settings `
  "ConnectionStrings__DefaultConnection=Server=tcp:your-server.database.windows.net,1433;Database=BookStoreDb;User ID=your-username;Password=your-password;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;" `
  "JwtSettings__SecretKey=BookStore-Super-Secret-Key-For-JWT-Tokens-2024-Must-Be-At-Least-256-Bits-Long" `
  "JwtSettings__Issuer=BookStore.API" `
  "JwtSettings__Audience=BookStore.Client" `
  "JwtSettings__ExpirationHours=24"

# Deploy from GitHub
az webapp deployment source config --name bookstore-api-$(Get-Random) --resource-group bookstore-rg --repo-url https://github.com/godwinsallah16/Book.git --branch main --manual-integration
