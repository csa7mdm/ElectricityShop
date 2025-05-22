import React from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';

interface ProtectedRouteProps {
  isAllowed: boolean;
  redirectPath: string;
  children?: React.ReactNode;
}

/**
 * A route component that protects access to specified routes
 * based on specified conditions (like authentication or role)
 */
const ProtectedRoute: React.FC<ProtectedRouteProps> = ({
  isAllowed,
  redirectPath,
  children,
}) => {
  const location = useLocation();

  if (!isAllowed) {
    // Redirect to the specified path if the user is not allowed
    // Save the current location they were trying to go to
    return <Navigate to={redirectPath} state={{ from: location }} replace />;
  }

  // If the user is allowed, render the children or the Outlet (for nested routes)
  return children ? <>{children}</> : <Outlet />;
};

export default ProtectedRoute;
