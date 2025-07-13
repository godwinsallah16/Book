import React, { forwardRef } from 'react';
import './Input.css';

export interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  hint?: string;
  leftIcon?: React.ReactNode;
  rightIcon?: React.ReactNode;
  variant?: 'default' | 'search';
  fullWidth?: boolean;
}

export const Input = forwardRef<HTMLInputElement, InputProps>(({
  label,
  error,
  hint,
  leftIcon,
  rightIcon,
  variant = 'default',
  fullWidth = false,
  className = '',
  id,
  ...props
}, ref) => {
  const inputId = id || `input-${Math.random().toString(36).substr(2, 9)}`;
  
  const inputClasses = [
    'input',
    `input--${variant}`,
    leftIcon ? 'input--with-left-icon' : '',
    rightIcon ? 'input--with-right-icon' : '',
    error ? 'input--error' : '',
    fullWidth ? 'input--full-width' : '',
    className
  ].filter(Boolean).join(' ');

  return (
    <div className={`input-group ${fullWidth ? 'input-group--full-width' : ''}`}>
      {label && (
        <label htmlFor={inputId} className="input-label">
          {label}
        </label>
      )}
      <div className="input-wrapper">
        {leftIcon && (
          <span className="input-icon input-icon--left">
            {leftIcon}
          </span>
        )}
        <input
          ref={ref}
          id={inputId}
          className={inputClasses}
          {...props}
        />
        {rightIcon && (
          <span className="input-icon input-icon--right">
            {rightIcon}
          </span>
        )}
      </div>
      {error && (
        <span className="input-error" role="alert">
          {error}
        </span>
      )}
      {hint && !error && (
        <span className="input-hint">
          {hint}
        </span>
      )}
    </div>
  );
});

Input.displayName = 'Input';

export default Input;
