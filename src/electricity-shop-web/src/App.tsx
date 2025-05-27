import React, { useEffect } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import { useAppDispatch } from './hooks/useAppDispatch';

import MainLayout from './components/layout/MainLayout';
import HomePage from './pages/HomePage';
import ProductListPage from './pages/ProductListPage';
import ProductDetailPage from './pages/ProductDetailPage';
import CartPage from './pages/CartPage';
import CheckoutPage from './pages/CheckoutPage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import UserProfilePage from './pages/UserProfilePage';
import UserOrdersPage from './pages/UserOrdersPage';
import AdminDashboardPage from './pages/admin/AdminDashboardPage';
import AdminProductsPage from './pages/admin/AdminProductsPage';
import AdminOrdersPage from './pages/admin/AdminOrdersPage';
import AdminUsersPage from './pages/admin/AdminUsersPage';
import NotFoundPage from './pages/NotFoundPage';
import ErrorBoundary from './components/common/ErrorBoundary';
import ProtectedRoute from './components/common/ProtectedRoute';
import { RootState } from './store';
import { refreshToken } from './store/auth/authSlice';
import { fetchCart } from './store/cart/cartSlice';

const App: React.FC = () => {
  const dispatch = useAppDispatch();
  const { isAuthenticated, user } = useSelector((state: RootState) => state.auth);

  useEffect(() => {
    // Check if token exists and try to refresh it
    if (localStorage.getItem('token')) {
      dispatch(refreshToken());
    }
  }, [dispatch]);

  useEffect(() => {
    // Fetch cart when authenticated
    if (isAuthenticated) {
      dispatch(fetchCart());
    }
  }, [isAuthenticated, dispatch]);

  return (
    <ErrorBoundary>
      <Routes>
        <Route path="/" element={<MainLayout />}>
          {/* Public routes */}
          <Route index element={<HomePage />} />
          <Route path="products" element={<ProductListPage />} />
          <Route path="products/:id" element={<ProductDetailPage />} />
          <Route path="cart" element={<CartPage />} />
          <Route path="login" element={<LoginPage />} />
          <Route path="register" element={<RegisterPage />} />
          
          {/* Protected user routes */}
          <Route element={<ProtectedRoute isAllowed={isAuthenticated} redirectPath="/login" />}>
            <Route path="checkout" element={<CheckoutPage />} />
            <Route path="profile" element={<UserProfilePage />} />
            <Route path="orders" element={<UserOrdersPage />} />
          </Route>
          
          {/* Protected admin routes */}
          <Route 
            element={
              <ProtectedRoute 
                isAllowed={isAuthenticated && user?.role === 'admin'} 
                redirectPath="/"
              />
            }
          >
            <Route path="admin" element={<Navigate to="/admin/dashboard" replace />} />
            <Route path="admin/dashboard" element={<AdminDashboardPage />} />
            <Route path="admin/products" element={<AdminProductsPage />} />
            <Route path="admin/orders" element={<AdminOrdersPage />} />
            <Route path="admin/users" element={<AdminUsersPage />} />
          </Route>
          
          {/* 404 route */}
          <Route path="*" element={<NotFoundPage />} />
        </Route>
      </Routes>
    </ErrorBoundary>
  );
};

export default App;
