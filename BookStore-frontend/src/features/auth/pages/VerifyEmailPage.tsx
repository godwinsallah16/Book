import { useEffect, useState } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { Button, Card } from '../../../shared/components/ui';
import { publicApiClient, API_CONFIG } from '../../../utils';

const VerifyEmailPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [status, setStatus] = useState<'verifying' | 'success' | 'error'>('verifying');
  const [message, setMessage] = useState('Verifying your email...');

  useEffect(() => {
    const userId = searchParams.get('userId');
    const token = searchParams.get('token');
    if (!userId || !token) {
      setStatus('error');
      setMessage('Invalid verification link.');
      return;
    }
    publicApiClient.post(API_CONFIG.ENDPOINTS.AUTH.VERIFY_EMAIL, { userId, token })
      .then(() => {
        // Clear any cached user info so next login is fresh
        localStorage.removeItem('authToken');
        localStorage.removeItem('user');
        setStatus('success');
        setMessage('Your email has been verified! You can now log in.');
        setTimeout(() => navigate('/login'), 2000);
      })
      .catch(() => {
        setStatus('error');
        setMessage('Verification failed or link expired. Please request a new verification email.');
      });
  }, [searchParams, navigate]);

  return (
    <div className="auth-container">
      <Card className="auth-card" padding="none">
        <div className="auth-card-content">
          <h1 className="auth-title">Email Verification</h1>
          <p>{message}</p>
          {status === 'error' && (
            <Button onClick={() => navigate('/login')} variant="primary">Back to Login</Button>
          )}
        </div>
      </Card>
    </div>
  );
};

export default VerifyEmailPage;
