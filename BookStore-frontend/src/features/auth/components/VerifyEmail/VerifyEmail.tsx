import React, { useState, useEffect } from 'react';
import { useSearchParams, Link } from 'react-router-dom';
import { authService } from '../../../../services/authService';
import { MESSAGES } from '../../../../utils/constants';
import './VerifyEmail.css';

const VerifyEmail: React.FC = () => {
  const [searchParams] = useSearchParams();
  const [status, setStatus] = useState<'loading' | 'success' | 'error'>('loading');
  const [message, setMessage] = useState('');

  useEffect(() => {
    const verifyEmail = async () => {
      const userId = searchParams.get('userId');
      const token = searchParams.get('token');

      if (!userId || !token) {
        setStatus('error');
        setMessage(MESSAGES.ERROR.INVALID_RESET_LINK);
        return;
      }

      try {
        await authService.verifyEmail({
          userId,
          token
        });
        setStatus('success');
        setMessage(MESSAGES.SUCCESS.EMAIL_VERIFIED);
      } catch (error) {
        setStatus('error');
        setMessage(MESSAGES.ERROR.VERIFICATION_FAILED);
        console.error('Email verification error:', error);
      }
    };

    verifyEmail();
  }, [searchParams]);

  return (
    <div className="verify-email-container">
      <div className="verify-email-card">
        <h2>Email Verification</h2>
        
        {status === 'loading' && (
          <div className="loading">
            <div className="spinner"></div>
            <p>Verifying your email...</p>
          </div>
        )}

        {status === 'success' && (
          <div className="success">
            <div className="success-icon">✓</div>
            <p>{message}</p>
            <Link to="/login" className="login-link">
              Continue to Login
            </Link>
          </div>
        )}

        {status === 'error' && (
          <div className="error">
            <div className="error-icon">✗</div>
            <p>{message}</p>
            <Link to="/login" className="login-link">
              Go to Login
            </Link>
          </div>
        )}
      </div>
    </div>
  );
};

export default VerifyEmail;
