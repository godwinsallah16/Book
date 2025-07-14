import React, { useEffect, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';

import { authService } from '../../services/authService';

function useQuery() {
  return new URLSearchParams(useLocation().search);
}

const VerifyEmail: React.FC = () => {
  const query = useQuery();
  const navigate = useNavigate();
  // Removed unused status state
  const [message, setMessage] = useState('Verifying your email...');

  useEffect(() => {
    const userId = query.get('userId');
    const token = query.get('token');
    if (!userId || !token) {
      setMessage('Invalid verification link.');
      return;
    }
    authService.verifyEmail({ userId, token })
      .then(() => {
        setMessage('Email verified! Redirecting to login...');
        setTimeout(() => navigate('/login'), 2000);
      })
      .catch(() => {
        setMessage('Verification failed. Please try again or contact support.');
      });
  }, [query, navigate]);

  return (
    <div style={{ textAlign: 'center', marginTop: '3rem' }}>
      <h2>Email Verification</h2>
      <p>{message}</p>
    </div>
  );
};

export default VerifyEmail;
