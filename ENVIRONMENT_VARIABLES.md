# Environment Variables Configuration Guide

This guide explains all the environment variables needed for your BookStore application deployment.

## 🔐 Required Environment Variables

### **Database Configuration**
```env
ConnectionStrings__DefaultConnection=Server=your-server;Database=BookStoreDb;User Id=your-user;Password=your-password;TrustServerCertificate=true;MultipleActiveResultSets=true
```

### **JWT Authentication**
```env
JwtSettings__SecretKey=BookStore-Super-Secret-Key-For-JWT-Tokens-2024-Must-Be-At-Least-256-Bits-Long
JwtSettings__Issuer=BookStore.API
JwtSettings__Audience=BookStore.Client
JwtSettings__ExpirationHours=24
```

### **📧 Email Configuration (IMPORTANT!)**
```env
EmailSettings__SmtpServer=smtp.gmail.com
EmailSettings__SmtpPort=587
EmailSettings__SmtpUsername=your-email@gmail.com
EmailSettings__SmtpPassword=your-app-password
EmailSettings__FromEmail=noreply@bookstore.com
EmailSettings__FromName=BookStore
EmailSettings__EnableSsl=true
```

### **Frontend Configuration**
```env
VITE_API_BASE_URL=https://your-api-url.com/api
VITE_APP_NAME=BookStore
```

## 📧 Email Setup Instructions

### **Gmail Setup (Recommended)**

1. **Enable 2-Factor Authentication** on your Gmail account
2. **Generate App Password:**
   - Go to Google Account settings
   - Security → 2-Step Verification
   - App passwords → Generate new password
   - Use this password for `EmailSettings__SmtpPassword`

3. **Gmail Settings:**
   ```env
   EmailSettings__SmtpServer=smtp.gmail.com
   EmailSettings__SmtpPort=587
   EmailSettings__SmtpUsername=your-email@gmail.com
   EmailSettings__SmtpPassword=your-16-character-app-password
   EmailSettings__EnableSsl=true
   ```

### **Outlook/Hotmail Setup**
```env
EmailSettings__SmtpServer=smtp-mail.outlook.com
EmailSettings__SmtpPort=587
EmailSettings__SmtpUsername=your-email@outlook.com
EmailSettings__SmtpPassword=your-password
EmailSettings__EnableSsl=true
```

### **SendGrid Setup (Production Recommended)**
```env
EmailSettings__SmtpServer=smtp.sendgrid.net
EmailSettings__SmtpPort=587
EmailSettings__SmtpUsername=apikey
EmailSettings__SmtpPassword=your-sendgrid-api-key
EmailSettings__EnableSsl=true
```

## 🌐 Platform-Specific Configuration

### **Render.com**
Set these as **secrets** in your Render dashboard:
- `EmailSettings__SmtpUsername`
- `EmailSettings__SmtpPassword`
- `JwtSettings__SecretKey`

### **Railway**
Set these as **variables** in your Railway dashboard:
- All email settings
- JWT settings
- Database connection string

### **Azure App Service**
Set these in **Configuration → Application Settings**:
- All environment variables listed above

### **Docker Deployment**
Create a `.env` file in your project root:
```env
DB_PASSWORD=BookStore123!
JWT_SECRET=BookStore-Super-Secret-Key-For-JWT-Tokens-2024-Must-Be-At-Least-256-Bits-Long
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-app-password
SMTP_FROM_EMAIL=noreply@bookstore.com
FRONTEND_URL=http://localhost:3000
```

## 🔧 Testing Email Configuration

### **Test Email Functionality**
1. Deploy your application
2. Register a new user
3. Check if verification email is sent
4. Test password reset functionality

### **Email Features in BookStore**
- ✅ **User Registration** - Email verification
- ✅ **Password Reset** - Reset link via email
- ✅ **Account Notifications** - Status updates
- ✅ **Order Confirmations** - Purchase receipts

## 🚨 Security Best Practices

### **Never Commit Secrets**
- Add `.env` to `.gitignore`
- Use platform secret managers
- Rotate passwords regularly

### **Production Email Security**
- Use dedicated email service (SendGrid, Mailgun)
- Set up SPF, DKIM, DMARC records
- Use app-specific passwords
- Enable SSL/TLS encryption

## 📋 Deployment Checklist

Before deploying, ensure you have:
- [ ] Database connection string
- [ ] JWT secret key (256+ bits)
- [ ] Email SMTP credentials
- [ ] Email service configured
- [ ] Frontend API URL set
- [ ] All secrets stored securely
- [ ] Test email functionality

## 🆘 Troubleshooting

### **Email Not Sending**
1. Check SMTP credentials
2. Verify port and SSL settings
3. Check email service limits
4. Review application logs

### **Authentication Issues**
1. Verify JWT secret key length
2. Check issuer/audience settings
3. Confirm token expiration

### **Database Connection**
1. Verify connection string format
2. Check firewall rules
3. Confirm database exists
4. Test connectivity

## 📞 Support

For email configuration help:
- Gmail: https://support.google.com/accounts/answer/185833
- Outlook: https://support.microsoft.com/en-us/office/
- SendGrid: https://docs.sendgrid.com/

Remember: Email configuration is crucial for user registration and password reset functionality! 📧
