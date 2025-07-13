import React from 'react';
import './LoadingSpinner.css';

export interface LoadingSpinnerProps {
  size?: 'sm' | 'md' | 'lg';
  color?: 'primary' | 'secondary' | 'white';
  text?: string;
  className?: string;
}

export const LoadingSpinner: React.FC<LoadingSpinnerProps> = ({
  size = 'md',
  color = 'primary',
  text,
  className = ''
}) => {
  const spinnerClasses = [
    'loading-spinner',
    `loading-spinner--${size}`,
    `loading-spinner--${color}`,
    className
  ].filter(Boolean).join(' ');

  if (text) {
    return (
      <div className="loading-container">
        <div className={spinnerClasses} />
        <span className="loading-text">{text}</span>
      </div>
    );
  }

  return <div className={spinnerClasses} />;
};

export default LoadingSpinner;
