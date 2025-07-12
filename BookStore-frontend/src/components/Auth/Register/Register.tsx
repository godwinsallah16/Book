import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { authService, type RegisterRequest } from '../../../services';
import './Register.css';

interface RegisterProps {
  onRegister: () => void;
}

const Register: React.FC<RegisterProps> = ({ onRegister }) => {
  const [formData, setFormData] = useState<RegisterRequest>({
    email: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: ''
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    if (formData.password !== formData.confirmPassword) {
      setError('Passwords do not match');
      setLoading(false);
      return;
    }

    try {
      await authService.register(formData);
      onRegister();
    } catch (error: unknown) {
      const errorMessage = error instanceof Error ? error.message : 'Registration failed. Please try again.';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="register-container">
      <div className="register-card">
        <h2>Register for BookStore</h2>
        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="firstName">First Name</label>
              <input
                type="text"
                id="firstName"
                name="firstName"
                value={formData.firstName}
                onChange={handleChange}
                required
                placeholder="Enter your first name"
              />
            </div>
            
            <div className="form-group">
              <label htmlFor="lastName">Last Name</label>
              <input
                type="text"
                id="lastName"
                name="lastName"
                value={formData.lastName}
                onChange={handleChange}
                required
                placeholder="Enter your last name"
              />
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="email">Email</label>
            <input
              type="email"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              required
              placeholder="Enter your email"
            />
          </div>
          
          <div className="form-group">
            <label htmlFor="password">Password</label>
            <input
              type="password"
              id="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              required
              placeholder="Enter your password"
              minLength={8}
            />
            <small className="password-requirements">
              Password must contain at least 8 characters including uppercase, lowercase, digit and special character.
            </small>
          </div>

          <div className="form-group">
            <label htmlFor="confirmPassword">Confirm Password</label>
            <input
              type="password"
              id="confirmPassword"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              required
              placeholder="Confirm your password"
              minLength={8}
            />
          </div>

          {error && <div className="error-message">{error}</div>}

          <button type="submit" disabled={loading} className="register-btn">
            {loading ? 'Registering...' : 'Register'}
          </button>
        </form>

        <div className="auth-switch">
          <p>
            Already have an account?{' '}
            <Link to="/login" className="switch-btn">
              Login here
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default Register;
