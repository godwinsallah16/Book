import React, { useState } from 'react';
import { authService } from '../../../services/authService';
import { MESSAGES } from '../../../utils/constants';
import './ResendVerification.css';

interface ResendVerificationProps {
  email?: string;
  onSuccess?: () => void;
  onBack?: () => void;
}

const ResendVerification: React.FC<ResendVerificationProps> = ({ 
  email: initialEmail = '',
  onSuccess,
  onBack 
}) => {
  const [email, setEmail] = useState(initialEmail);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');
  const [messageType, setMessageType] = useState<'success' | 'error' | ''>('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!email) {
      setMessage('Please enter your email address');
      setMessageType('error');
      return;
    }

    setLoading(true);
    setMessage('');
    setMessageType('');

    try {
      await authService.resendVerification({ email });
      setMessage(MESSAGES.SUCCESS.VERIFICATION_EMAIL_SENT);
      setMessageType('success');
      onSuccess?.();
    } catch (error) {
      console.error('Resend verification error:', error);
      setMessage(MESSAGES.ERROR.VERIFICATION_EMAIL_FAILED);
      setMessageType('error');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="resend-verification-container">
      <div className="resend-verification-card">
        <h2>Resend Email Verification</h2>
        <p className="resend-verification-description">
          Didn't receive the verification email? Enter your email address below to get a new verification link.
        </p>
        
        <form onSubmit={handleSubmit} className="resend-verification-form">
          <div className="form-group">
            <label htmlFor="email">Email Address</label>
            <input
              type="email"
              id="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="Enter your email address"
              required
              disabled={loading}
            />
          </div>
          
          {message && (
            <div className={`message ${messageType}`}>
              {message}
            </div>
          )}
          
          <button 
            type="submit" 
            className="resend-verification-button"
            disabled={loading}
          >
            {loading ? 'Sending...' : 'Resend Verification Email'}
          </button>
          
          {onBack && (
            <button 
              type="button" 
              className="back-button"
              onClick={onBack}
              disabled={loading}
            >
              Back to Login
            </button>
          )}
        </form>
        
        <div className="resend-verification-note">
          <p>
            <strong>Note:</strong> Check your email inbox and spam folder after clicking submit.
            The verification link will expire in 24 hours.
          </p>
        </div>
      </div>
    </div>
  );
};

export default ResendVerification;
