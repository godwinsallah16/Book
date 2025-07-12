# 🔧 Manual Render.com Setup Guide

## The Issue
The render.yaml blueprint configuration isn't properly setting the DATABASE_URL environment variable. Let's set up the services manually.

## Step-by-Step Manual Setup

### 1. Create PostgreSQL Database Service

1. **Go to Render.com Dashboard**
   - Click "New +" → "PostgreSQL"
   - Name: `bookstore-db`
   - Database Name: `bookstore`
   - User: `bookstore_user`
   - Region: Choose your preferred region
   - Click "Create Database"

2. **Note the Database Details**
   - After creation, go to the database settings
   - Copy the **External Database URL** (this is your DATABASE_URL)
   - It should look like: `postgresql://username:password@hostname:port/database`

### 2. Create Web Service (Backend API)

1. **Create Web Service**
   - Click "New +" → "Web Service"
   - Connect your GitHub repository: `godwinsallah16/Book`
   - Name: `bookstore-api`
   - Region: Same as your database
   - Branch: `main`
   - Runtime: `Docker`
   - Dockerfile Path: `./Dockerfile`

2. **Configure Environment Variables**
   In the Environment tab, add these variables:

   ```env
   # Core Settings
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://0.0.0.0:10000
   
   # Database (MOST IMPORTANT!)
   DATABASE_URL=postgresql://username:password@hostname:port/database
   # ☝️ Replace with your actual External Database URL from step 1
   
   # JWT Settings
   JwtSettings__SecretKey=BookStore-Super-Secret-Key-For-JWT-Tokens-2024-Must-Be-At-Least-256-Bits-Long
   JwtSettings__Issuer=BookStore.API
   JwtSettings__Audience=BookStore.Client
   JwtSettings__ExpirationHours=24
   
   # Email Settings (Optional - set these if you need email functionality)
   EmailSettings__SmtpServer=smtp.gmail.com
   EmailSettings__SmtpPort=587
   EmailSettings__SmtpUsername=your-email@gmail.com
   EmailSettings__SmtpPassword=your-gmail-app-password
   EmailSettings__FromEmail=noreply@bookstore.com
   EmailSettings__FromName=BookStore
   EmailSettings__EnableSsl=true
   ```

3. **Deploy Settings**
   - Build Command: Leave empty (Docker handles this)
   - Start Command: Leave empty (Docker handles this)
   - Health Check Path: `/health`

### 3. Create Static Site (Frontend)

1. **Create Static Site**
   - Click "New +" → "Static Site"
   - Connect your GitHub repository: `godwinsallah16/Book`
   - Name: `bookstore-frontend`
   - Branch: `main`
   - Root Directory: `BookStore-frontend`
   - Build Command: Leave empty (automatic build via postinstall script)
   - Publish Directory: `dist`

2. **Configure Environment Variables**
   ```env
   VITE_API_BASE_URL=https://your-actual-api-url.onrender.com/api
   VITE_APP_NAME=BookStore
   ```

   **Replace `your-actual-api-url.onrender.com` with your actual API service URL from step 2.**

**IMPORTANT**: 
- Set the Root Directory to `BookStore-frontend` in the Render.com dashboard
- Set the VITE_API_BASE_URL environment variable to your actual backend API URL
- **No build command needed** - the `postinstall` script in package.json automatically runs `npm run build` after `npm install`

### 4. Deploy and Test

1. **Deploy Backend First**
   - The backend service will automatically deploy
   - Check the logs to ensure DATABASE_URL is now available
   - Look for: `DATABASE_URL exists: YES`

2. **Deploy Frontend**
   - Set the `VITE_API_BASE_URL` environment variable to your actual backend URL
   - The frontend will automatically deploy and connect to your API

3. **Test the Application**
   - Backend: `https://your-backend-url.onrender.com/health`
   - Frontend: `https://your-frontend-url.onrender.com`

## Expected Success Logs

After manual setup, you should see:

```
=== Environment Variables Debug ===
ASPNETCORE_ENVIRONMENT: Production
DATABASE_URL exists: YES
DATABASE_URL length: 100+
All env vars with DATABASE:
DATABASE_URL: postgresql://username:pass...
=== Starting Application ===
[INFO] DATABASE_URL exists: True
[INFO] Connection string source: DATABASE_URL environment variable
[INFO] Using DATABASE_URL for PostgreSQL connection
[INFO] Using PostgreSQL database
[INFO] Database connection successful
[INFO] Database migrations applied successfully
[INFO] Application started successfully
```

## Why Manual Setup is Needed

The render.yaml blueprint has these limitations:
1. Database linking sometimes doesn't work properly
2. Environment variables aren't always set correctly
3. Manual setup gives you more control and visibility

## Alternative: Fix render.yaml (Advanced)

If you prefer to use the blueprint approach, the issue is likely:
1. The database service name in render.yaml doesn't match the actual service
2. The `fromDatabase` reference isn't working properly
3. The blueprint needs to be applied in a specific order

## Next Steps

1. **Use the manual setup above** - it's more reliable
2. **Test the deployment** - check the logs for successful DATABASE_URL detection
3. **Update frontend API URL** - use your actual backend URL
4. **Test the full application** - verify all functionality works

The manual setup approach is recommended because it gives you full control over the deployment process and ensures all environment variables are properly configured.
