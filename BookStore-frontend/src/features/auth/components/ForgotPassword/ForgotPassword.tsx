import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { authService } from '../../../../services';
import { MESSAGES } from '../../../../utils/constants';
import './ForgotPassword.css';

const ForgotPassword: React.FC = () => {
  const [email, setEmail] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState('');
  const [isSuccess, setIsSuccess] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setMessage('');

    try {
      await authService.forgotPassword({ email });
      setIsSuccess(true);
      setMessage(MESSAGES.SUCCESS.PASSWORD_RESET_SENT);
    } catch (err) {
      setIsSuccess(false);
      setMessage(MESSAGES.ERROR.GENERIC);
      console.error('Forgot password error:', err);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="forgot-password-container">
      <div className="forgot-password-card">
        <h2>Forgot Password</h2>
        <p>
          Enter your email address and we'll send you a link to reset your password.
        </p>
        
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="email">Email Address</label>
            <input
              id="email"
              name="email"
              type="email"
              autoComplete="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="Enter your email address"
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
            {isLoading ? MESSAGES.LOADING.SENDING : 'Send Reset Link'}
          </button>
        </form>

        <Link to="/login" className="back-link">
          Back to Login
        </Link>
      </div>
    </div>
  );
};

export default ForgotPassword;
