# 🚀 BookStore Deployment Guide

## Quick Start Deployment

### 1. Frontend (GitHub Pages) - Already Configured ✅

Your frontend will automatically deploy to GitHub Pages when you push to the main branch.

**Steps:**
1. Push your code to the main branch
2. Go to your GitHub repository settings
3. Enable GitHub Pages (if not already enabled)
4. Your frontend will be available at: `https://godwinsallah16.github.io/Book/`

### 2. Backend Deployment Options

#### Option A: Render.com (Recommended - Easy Setup)

**Steps:**
1. Run the deployment setup:
   ```bash
   node deploy.js
   ```

2. Commit and push changes:
   ```bash
   git add .
   git commit -m "Add Render.com deployment configuration"
   git push origin main
   ```

3. Deploy on Render.com:
   - Go to https://render.com
   - Sign up/Login with GitHub
   - Click "New +" → "Blueprint"
   - Connect your GitHub repository
   - Render will detect render.yaml automatically
   - **Important:** Set these environment variables as secrets:
     - `EmailSettings__SmtpUsername` (your email)
     - `EmailSettings__SmtpPassword` (your app password)
   - Click "Apply" to deploy

#### Option B: Railway (Alternative Easy Setup)

**Steps:**
1. Install Railway CLI:
   ```bash
   npm install -g @railway/cli
   ```

2. Login and deploy:
   ```bash
   railway login
   railway init
   railway up
   ```

3. Set environment variables in Railway dashboard:
   - `ConnectionStrings__DefaultConnection` (Railway will provide a database)
   - `JwtSettings__SecretKey=BookStore-Super-Secret-Key-For-JWT-Tokens-2024-Must-Be-At-Least-256-Bits-Long`
   - `JwtSettings__Issuer=BookStore.API`
   - `JwtSettings__Audience=BookStore.Client`

#### Option C: Azure App Service

**Prerequisites:**
- Azure CLI installed
- Azure subscription

**Steps:**
1. Run the deployment script:
   ```powershell
   ./deploy-azure.ps1
   ```

2. Update the app name in the script with your actual app name
3. Configure database connection string in Azure portal

#### Option D: Docker Deployment

**Local/VPS Deployment:**
```bash
# Production deployment
docker-compose -f docker-compose.prod.yml up -d
```

**Environment Variables Required:**
Create a `.env` file:
```env
DB_PASSWORD=BookStore123!
GITHUB_REPOSITORY=godwinsallah16/Book
JWT_SECRET=BookStore-Super-Secret-Key-For-JWT-Tokens-2024-Must-Be-At-Least-256-Bits-Long
```

### 3. Connect Frontend to Backend

After deploying your backend, update the GitHub repository secrets:

1. Go to your GitHub repository → Settings → Secrets and variables → Actions
2. Add a new secret: `VITE_API_BASE_URL`
3. Value: Your backend URL (e.g., `https://your-railway-app.railway.app/api`)

### 4. Database Setup

**For Railway:**
- Railway provides PostgreSQL - you'll need to update your connection string format
- Or use Railway's database addon

**For Azure:**
- Create Azure SQL Database
- Update connection string in app settings

**For Docker:**
- SQL Server container is included in docker-compose

### 5. Deployment Verification

After deployment, verify:
- ✅ Frontend loads at GitHub Pages URL
- ✅ Backend API responds at `/health` endpoint
- ✅ Swagger documentation available at `/swagger`
- ✅ Database connections working
- ✅ Authentication endpoints working

## Troubleshooting

### Common Issues:

1. **CORS Errors:**
   - Update CORS settings in your API to allow your frontend domain

2. **Database Connection Issues:**
   - Verify connection string format matches your database provider
   - Check firewall rules

3. **JWT Token Issues:**
   - Ensure JWT secret is at least 256 bits (32 characters)
   - Verify issuer and audience match

4. **Frontend Build Issues:**
   - Check environment variables are set correctly
   - Verify API base URL is correct

## Next Steps

1. **Choose your deployment method** (Railway recommended for beginners)
2. **Deploy backend** using chosen method
3. **Update frontend API URL** in GitHub secrets
4. **Push to main branch** to trigger frontend deployment
5. **Test the full application**

## Production Checklist

- [ ] Backend deployed and accessible
- [ ] Database configured and migrated
- [ ] Environment variables set securely
- [ ] CORS configured for frontend domain
- [ ] HTTPS enabled (most platforms do this automatically)
- [ ] Frontend deployed to GitHub Pages
- [ ] API URL updated in frontend
- [ ] Email service configured (if needed)
- [ ] Error logging configured
- [ ] Monitoring set up

## Support

If you need help with deployment:
1. Check the platform-specific documentation
2. Review the error logs
3. Verify environment variables
4. Test API endpoints individually

Good luck with your deployment! 🚀
