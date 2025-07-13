# Auth Feature

Handles user authentication, registration, login, and email verification for the BookStore frontend.

## 📁 Structure

```
auth/
├── components/          # Authentication components
│   ├── Login/          # Login component
│   ├── Register/       # Registration component
│   ├── ForgotPassword/ # Password reset
│   ├── ResetPassword/  # Password reset form
│   ├── VerifyEmail/    # Email verification
│   └── ResendVerification/ # Resend verification
├── pages/              # Authentication pages
│   ├── LoginPage.tsx   # Login page
│   ├── RegisterPage.tsx # Registration page
│   └── EmailVerificationRequired.tsx # Email verification required
└── index.ts            # Feature exports
```

## 🔧 Components

- **Login**: Main login form component
- **Register**: User registration form
- **ForgotPassword**: Password reset request form
- **ResetPassword**: New password form
- **VerifyEmail**: Email verification handler
- **ResendVerification**: Resend verification email

## 📄 Pages

- **LoginPage**: Login page wrapper
- **RegisterPage**: Registration page wrapper
- **EmailVerificationRequired**: Email verification required page

## 🔗 Usage

```typescript
import { LoginPage, RegisterPage, ForgotPassword } from './features/auth';
```
