import { useState } from 'react';
import { Link } from 'react-router-dom';
import { authService } from '../../../../services';
import type { LoginRequest } from '../../../../services/authService';
import { Button, Input, Card } from '../../../../shared/components/ui';
import './Login.css';

interface LoginProps {
  onLogin: () => void;
}

const Login: React.FC<LoginProps> = ({ onLogin }) => {
  const [formData, setFormData] = useState<LoginRequest>({
    email: '',
    password: ''
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev: LoginRequest) => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      await authService.login(formData);
      onLogin(); // Always redirect to dashboard after login
    } catch (error: unknown) {
      const errorMessage = error instanceof Error ? error.message : 'Login failed. Please try again.';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-container">
      <Card className="auth-card" padding="none">
        <div className="auth-card-content">
          <h1 className="auth-title">Welcome Back</h1>
          
          {error && (
            <div className="error-container">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit} className="auth-form">
            <Input
              label="Email"
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              placeholder="Enter your email"
              required
              fullWidth
            />

            <Input
              label="Password"
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              placeholder="Enter your password"
              required
              fullWidth
            />

            <Button
              type="submit"
              variant="primary"
              size="lg"
              loading={loading}
              fullWidth
            >
              Sign In
            </Button>
          </form>

          <div className="auth-footer">
            <p>
              Don't have an account?{' '}
              <Link to="/register">Sign up here</Link>
            </p>
            <p>
              <Link to="/forgot-password">Forgot your password?</Link>
            </p>
          </div>
        </div>
      </Card>
    </div>
  );
};

export default Login;
