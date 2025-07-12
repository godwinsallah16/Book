# Render.com Manual Setup Guide

## The Issue
The automated Blueprint deployment isn't working because the `DATABASE_URL` environment variable is not being set. This requires manual setup on Render.com.

## Manual Setup Steps

### Step 1: Create PostgreSQL Database
1. Go to https://render.com/dashboard
2. Click "New +" → "PostgreSQL"
3. Configure:
   - **Name**: `bookstore-db`
   - **Database**: `bookstore`
   - **User**: `bookstore_user`
   - **Region**: `Ohio` (or your preferred region)
   - **Plan**: `Free`
4. Click "Create Database"
5. **Wait for database to be created** (this takes 2-3 minutes)
6. **Copy the External Database URL** (starts with `postgres://`)

### Step 2: Create Web Service
1. Click "New +" → "Web Service"
2. Connect your GitHub repository: `godwinsallah16/Book`
3. Configure:
   - **Name**: `bookstore-api`
   - **Environment**: `Docker`
   - **Dockerfile Path**: `./Dockerfile`
   - **Region**: `Ohio` (same as database)
   - **Plan**: `Free`

### Step 3: Set Environment Variables
In the Web Service settings, add these environment variables:

**Required Variables:**
```
ASPNETCORE_ENVIRONMENT = Production
ASPNETCORE_URLS = http://0.0.0.0:10000
DATABASE_URL = [paste the External Database URL from Step 1]
JwtSettings__SecretKey = [generate a 32+ character random string]
JwtSettings__Issuer = BookStore.API
JwtSettings__Audience = BookStore.Client
JwtSettings__ExpirationHours = 24
```

**Email Variables (optional):**
```
EmailSettings__SmtpServer = smtp.gmail.com
EmailSettings__SmtpPort = 587
EmailSettings__SmtpUsername = [your email]
EmailSettings__SmtpPassword = [your gmail app password]
EmailSettings__FromEmail = noreply@bookstore.com
EmailSettings__FromName = BookStore
EmailSettings__EnableSsl = true
```

### Step 4: Deploy
1. Click "Create Web Service"
2. The deployment should now succeed with proper database connection

### Step 5: Create Static Site (Frontend)
1. Click "New +" → "Static Site"
2. Connect your GitHub repository: `godwinsallah16/Book`
3. Configure:
   - **Name**: `bookstore-frontend`
   - **Build Command**: `cd BookStore-frontend && npm install && npm run build`
   - **Publish Directory**: `BookStore-frontend/dist`
4. Set environment variables:
   ```
   VITE_API_BASE_URL = https://bookstore-api.onrender.com/api
   VITE_APP_NAME = BookStore
   ```

## Important Notes

### Database URL Format
The DATABASE_URL should look like:
```
postgres://username:password@host:port/database
```

### JWT Secret Generation
Generate a secure JWT secret (32+ characters):
```bash
# You can use this online: https://generate-secret.now.sh/32
# Or use PowerShell:
[System.Web.Security.Membership]::GeneratePassword(32, 0)
```

### Common Issues

1. **Database not ready**: Wait 2-3 minutes after creating the database before deploying the web service
2. **Wrong region**: Ensure both database and web service are in the same region
3. **Invalid DATABASE_URL**: Make sure you copied the complete External Database URL
4. **Missing environment variables**: Double-check all required variables are set

## Expected Success Logs
After correct setup, you should see:
```
[INFO] === Database Configuration Debug ===
[INFO] Environment: Production
[INFO] DATABASE_URL exists: True
[INFO] Connection string source: DATABASE_URL environment variable
[INFO] Using DATABASE_URL for PostgreSQL connection
[INFO] Using PostgreSQL database
[INFO] Database operation attempt 1 of 5
[INFO] Database connection successful
[INFO] Database migrations applied successfully
[INFO] Database seeding completed successfully
[INFO] Starting BookStore API
```

## Troubleshooting

If deployment still fails:
1. Check that DATABASE_URL is properly set in the web service environment variables
2. Verify the database is running and accessible
3. Ensure the database and web service are in the same region
4. Check the logs for specific error messages

This manual setup should resolve the DATABASE_URL issue that's preventing successful deployment.
