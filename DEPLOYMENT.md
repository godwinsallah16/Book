# 🚀 BookStore Deployment Guide

## Quick Start Deployment

### 1. Frontend (GitHub Pages) - Setup Required

**Initial Setup:**
1. Go to your GitHub repository settings
2. Navigate to "Pages" in the left sidebar
3. Under "Source", select "GitHub Actions"
4. Push your code to the main branch (the workflow will run automatically)
5. Your frontend will be available at: `https://godwinsallah16.github.io/Book/`

**Note:** The GitHub Actions workflow is configured with the proper permissions to deploy to GitHub Pages automatically.

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

#### Option B: Docker Deployment (Local/VPS)

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
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-gmail-app-password
SMTP_FROM_EMAIL=noreply@bookstore.com
FRONTEND_URL=http://localhost:3000
```

### 3. Connect Frontend to Backend

After deploying your backend, update the GitHub repository secrets:

1. Go to your GitHub repository → Settings → Secrets and variables → Actions
2. Add a new secret: `VITE_API_BASE_URL`
3. Value: Your backend URL (e.g., `https://your-railway-app.railway.app/api`)

### 4. Database Setup

**For Render.com:**
- PostgreSQL database is automatically provisioned
- Connection string is automatically configured

**For Docker:**
- SQL Server container is included in docker-compose
- Database is automatically created and seeded

### 5. Deployment Verification

After deployment, verify:
- ✅ Frontend loads at GitHub Pages URL
- ✅ Backend API responds at `/health` endpoint
- ✅ Swagger documentation available at `/swagger`
- ✅ Database connections working
- ✅ Authentication endpoints working

## Troubleshooting

### Common Issues:

1. **Database Connection Issues (LocalDB Error on Render):**
   - **Error**: "LocalDB is not supported on this platform"
   - **Solution**: The app automatically detects PostgreSQL connection strings and uses the appropriate database provider
   - **Fix**: Ensure your connection string contains "postgres" or "postgresql" for cloud deployments

2. **PostgreSQL Connection String Issues:**
   - **Error**: "Couldn't set trusted_connection" or "The given key was not present in the dictionary"
   - **Cause**: SQL Server connection string parameters being passed to PostgreSQL
   - **Solution**: Ensure `DATABASE_URL` environment variable is set correctly on Render.com
   - **Fix**: The app now automatically detects and handles this, but verify your Render database URL is properly set

3. **GitHub Pages Deployment Permission Issues:**
   - **Error**: "Permission denied to github-actions[bot]"
   - **Solution**: Ensure repository settings have:
     - Go to Settings → Actions → General → Workflow permissions
     - Select "Read and write permissions"
     - Check "Allow GitHub Actions to create and approve pull requests"
     - Go to Settings → Pages → Select "GitHub Actions" as source

4. **CORS Errors:**
   - Update CORS settings in your API to allow your frontend domain

3. **JWT Token Issues:**
   - Ensure JWT secret is at least 256 bits (32 characters)
   - Verify issuer and audience match

4. **Frontend Build Issues:**
   - Check environment variables are set correctly
   - Verify API base URL is correct

5. **Email Configuration Issues:**
   - Verify SMTP credentials are correct
   - Check if Gmail App Password is properly configured
   - Ensure email settings environment variables are set

## Next Steps

1. **Choose your deployment method:**
   - **Render.com** (recommended for cloud deployment)
   - **Docker** (for local/VPS deployment)
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
