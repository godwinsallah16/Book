
import React, { useState } from 'react';
import { useLocation, useNavigate, Link } from 'react-router-dom';
import { authService } from '../../../services';
import './EmailVerificationRequired.css';

const EmailVerificationRequired: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();
  // Try to get email and verification status from location.state or current user
  const currentUser = authService.getCurrentUser();
  const userEmail = location.state?.userEmail || currentUser?.email;
  const isVerified = currentUser?.emailConfirmed === true;
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [resendSuccess, setResendSuccess] = useState(false);

  // If no userEmail, redirect to login. If already verified, redirect to dashboard.
  React.useEffect(() => {
    if (!userEmail) {
      navigate('/login');
    } else if (isVerified) {
      navigate('/dashboard');
    }
  }, [userEmail, isVerified, navigate]);
  if (!userEmail || isVerified) return null;

  const handleResendVerification = async () => {
    setLoading(true);
    setError('');
    setMessage('');
    setResendSuccess(false);

    try {
      await authService.resendVerification({ email: userEmail });
      setResendSuccess(true);
      setMessage('Verification email sent successfully! Please check your inbox and spam folder.');
    } catch (error: unknown) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to resend verification email. Please try again.';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    authService.logout();
    navigate('/login');
  };

  return (
    <div className="email-verification-container">
      <div className="email-verification-card">
        <div className="icon-container">
          <div className="email-icon">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
              strokeWidth={1.5}
              stroke="currentColor"
              className="email-svg"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M21.75 6.75v10.5a2.25 2.25 0 01-2.25 2.25h-15a2.25 2.25 0 01-2.25-2.25V6.75m19.5 0A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25m19.5 0v.243a2.25 2.25 0 01-1.07 1.916l-7.5 4.615a2.25 2.25 0 01-2.36 0L3.32 8.91a2.25 2.25 0 01-1.07-1.916V6.75"
              />
            </svg>
          </div>
        </div>
        <h1>Email Verification Required</h1>
        <div className="verification-content">
          <p className="verification-message">
            To access the BookStore application, you need to verify your email address first.
          </p>
          {userEmail && (
            <p className="user-email">
              We've sent a verification link to: <strong>{userEmail}</strong>
            </p>
          )}
          <div className="verification-steps">
            <h3>What to do next:</h3>
            <ol>
              <li>Check your email inbox (including spam/junk folder)</li>
              <li>Look for an email from BookStore with the subject "Verify Your Email"</li>
              <li>Click the verification link in the email</li>
              <li>Return here and log in again</li>
            </ol>
          </div>
          {error && (
            <div className="error-message">{error}</div>
          )}
          {message && (
            <div className="success-message">{message}</div>
          )}
          <div className="verification-actions">
            {userEmail && !resendSuccess && (
              <button
                onClick={handleResendVerification}
                disabled={loading}
                className="resend-button"
              >
                {loading ? 'Sending...' : 'Resend Verification Email'}
              </button>
            )}
            {resendSuccess && (
              <div className="resend-success">
                ✅ Verification email sent! Check your inbox.
              </div>
            )}
            <div className="action-links">
              <button
                onClick={handleLogout}
                className="logout-button"
              >
                Logout and Login Again
              </button>
              <Link to="/login" className="login-link">
                Go to Login
              </Link>
            </div>
          </div>
        </div>
        <div className="help-section">
          <h4>Need Help?</h4>
          <p>
            If you're not receiving the verification email, try checking your spam folder
            or contact support for assistance.
          </p>
        </div>
      </div>
    </div>
  );
};

export default EmailVerificationRequired;
