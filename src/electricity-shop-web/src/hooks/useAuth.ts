 import { useSelector, useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { RootState } from '../store';
import { login, register, logout, refreshToken } from '../store/auth/authSlice';

export const useAuth = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { user, isAuthenticated, isLoading, error } = useSelector(
    (state: RootState) => state.auth
  );
  
  const handleLogin = async (email: string, password: string) => {
    try {
      await dispatch(login({ email, password })).unwrap();
      navigate('/');
      return true;
    } catch (error) {
      return false;
    }
  };
  
  const handleRegister = async (
    email: string,
    password: string,
    firstName: string,
    lastName: string
  ) => {
    try {
      await dispatch(register({ email, password, firstName, lastName })).unwrap();
      navigate('/');
      return true;
    } catch (error) {
      return false;
    }
  };
  
  const handleLogout = () => {
    dispatch(logout());
    navigate('/login');
  };
  
  const handleRefreshToken = async () => {
    try {
      await dispatch(refreshToken()).unwrap();
      return true;
    } catch (error) {
      return false;
    }
  };
  
  return {
    user,
    isAuthenticated,
    isLoading,
    error,
    login: handleLogin,
    register: handleRegister,
    logout: handleLogout,
    refreshToken: handleRefreshToken,
    isAdmin: user?.role === 'admin',
  };
};
