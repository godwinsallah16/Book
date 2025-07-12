import React, { useState } from 'react';
import { useSearchParams, useNavigate, Link } from 'react-router-dom';
import { authService } from '../../../services';
import { MESSAGES } from '../../../utils/constants';
import './ResetPassword.css';

const ResetPassword: React.FC = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState('');
  const [isSuccess, setIsSuccess] = useState(false);

  const email = searchParams.get('email');
  const token = searchParams.get('token');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (password !== confirmPassword) {
      setMessage(MESSAGES.ERROR.PASSWORD_MISMATCH);
      setIsSuccess(false);
      return;
    }

    if (!email || !token) {
      setMessage(MESSAGES.ERROR.INVALID_RESET_LINK);
      setIsSuccess(false);
      return;
    }

    setIsLoading(true);
    setMessage('');

    try {
      await authService.resetPassword({
        email,
        token,
        newPassword: password
      });
      setIsSuccess(true);
      setMessage(MESSAGES.SUCCESS.PASSWORD_RESET_SUCCESS);
      setTimeout(() => {
        navigate('/login');
      }, 3000);
    } catch (error) {
      setIsSuccess(false);
      setMessage(MESSAGES.ERROR.RESET_LINK_EXPIRED);
      console.error('Reset password error:', error);
    } finally {
      setIsLoading(false);
    }
  };

  if (!email || !token) {
    return (
      <div className="reset-password-container">
        <div className="reset-password-card">
          <h2>Invalid Reset Link</h2>
          <p>The password reset link is invalid or has expired.</p>
          <Link to="/login" className="back-link">
            Go to Login
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="reset-password-container">
      <div className="reset-password-card">
        <h2>Reset Password</h2>
        <p>
          Enter your new password below.
        </p>
        
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="password">New Password</label>
            <input
              id="password"
              name="password"
              type="password"
              autoComplete="new-password"
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Enter new password"
              minLength={6}
            />
          </div>

          <div className="form-group">
            <label htmlFor="confirmPassword">Confirm New Password</label>
            <input
              id="confirmPassword"
              name="confirmPassword"
              type="password"
              autoComplete="new-password"
              required
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              placeholder="Confirm new password"
              minLength={6}
            />
          </div>

          {message && (
            <div className={`message ${isSuccess ? 'success' : 'error'}`}>
              {message}
            </div>
          )}

          <button
            type="submit"
            disabled={isLoading}
            className="submit-btn"
          >
            {isLoading ? MESSAGES.LOADING.RESETTING : 'Reset Password'}
          </button>
        </form>

        <Link to="/login" className="back-link">
          Back to Login
        </Link>
      </div>
    </div>
  );
};

export default ResetPassword;
