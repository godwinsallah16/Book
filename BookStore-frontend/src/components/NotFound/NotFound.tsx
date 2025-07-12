import { useNavigate } from 'react-router-dom';
import './NotFound.css';

interface NotFoundProps {
  message?: string;
  redirectTo?: string;
}

export function NotFound({ 
  message = "Sorry, the page you're looking for doesn't exist.", 
  redirectTo = "/dashboard" 
}: NotFoundProps) {
  const navigate = useNavigate();

  const handleGoBack = () => {
    navigate(redirectTo);
  };

  return (
    <div className="not-found">
      <div className="not-found-content">
        <h1>404</h1>
        <h2>Page Not Found</h2>
        <p>{message}</p>
        <button onClick={handleGoBack} className="go-back-btn">
          Go Back to Dashboard
        </button>
      </div>
    </div>
  );
}

export default NotFound;
