import React, { useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Container, Box, Typography, Grid, Paper } from '@mui/material';
import LoginForm from '../components/features/auth/LoginForm';
import { useAuth } from '../hooks/useAuth';

const LoginPage: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  
  // Get the redirect path from location state or default to home
  const from = location.state?.from || '/';
  
  useEffect(() => {
    // If already authenticated, redirect
    if (isAuthenticated) {
      navigate(from, { replace: true });
    }
  }, [isAuthenticated, navigate, from]);
  
  return (
    <Container maxWidth="md">
      <Grid container spacing={4} justifyContent="center" alignItems="center" sx={{ minHeight: '70vh' }}>
        <Grid item xs={12} md={6}>
          <Typography variant="h4" component="h1" gutterBottom fontWeight="bold">
            Welcome Back
          </Typography>
          <Typography variant="body1" color="text.secondary" paragraph>
            Log in to your ElectricityShop account to access your orders, saved items, and personalized recommendations.
          </Typography>
          <Box sx={{ mt: 4, display: { xs: 'none', md: 'block' } }}>
            <Paper
              elevation={0}
              sx={{
                p: 3,
                bgcolor: 'primary.light',
                color: 'primary.contrastText',
                borderRadius: 2,
              }}
            >
              <Typography variant="h6" gutterBottom>
                Benefits of having an account:
              </Typography>
              <ul>
                <li>Quick checkout process</li>
                <li>Track your orders easily</li>
                <li>Exclusive deals and offers</li>
                <li>Save your favorite products</li>
              </ul>
            </Paper>
          </Box>
        </Grid>
        
        <Grid item xs={12} md={6}>
          <LoginForm />
        </Grid>
      </Grid>
    </Container>
  );
};

export default LoginPage;
