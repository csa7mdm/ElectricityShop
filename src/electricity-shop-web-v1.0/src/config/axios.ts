import axios from 'axios';
import { refreshTokenAction, logoutAction } from '../store/slices/authSlice';

const baseURL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

export const api = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor for adding token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor for token refresh
let isRefreshing = false;
let failedQueue: { resolve: Function; reject: Function }[] = [];

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });
  
  failedQueue = [];
};

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    
    // If error is 401 and we haven't tried to refresh token yet
    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        // If refresh is already in progress, add to queue
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then((token) => {
            originalRequest.headers.Authorization = `Bearer ${token}`;
            return api(originalRequest);
          })
          .catch((err) => Promise.reject(err));
      }

      originalRequest._retry = true;
      isRefreshing = true;
      
      try {
        // We need to use the store dispatch, which will be set up in the
        // initializeAxios function below
        const dispatch = api.defaults.headers.common.dispatch;
        if (!dispatch) {
          throw new Error('Store dispatch not available');
        }
        
        const result = await dispatch(refreshTokenAction());
        const newToken = result.payload?.token;
        
        if (newToken) {
          // Set new token in local storage
          localStorage.setItem('token', newToken);
          
          // Update Authorization header for future requests
          api.defaults.headers.common.Authorization = `Bearer ${newToken}`;
          originalRequest.headers.Authorization = `Bearer ${newToken}`;
          
          processQueue(null, newToken);
          return api(originalRequest);
        } else {
          processQueue(error, null);
          throw new Error('Failed to refresh token');
        }
      } catch (refreshError) {
        processQueue(refreshError, null);
        
        // Logout user if refresh token failed
        const dispatch = api.defaults.headers.common.dispatch;
        if (dispatch) {
          dispatch(logoutAction());
        }
        
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }
    
    return Promise.reject(error);
  }
);

// This function needs to be called after store is created
export const initializeAxios = (dispatch: any) => {
  api.defaults.headers.common.dispatch = dispatch;
};

export default api;
