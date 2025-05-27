import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Container, Box, Typography, Grid, Paper } from '@mui/material';
import RegisterForm from '../components/features/auth/RegisterForm';
import { useAuth } from '../hooks/useAuth';

const RegisterPage: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const navigate = useNavigate();
  
  useEffect(() => {
    // If already authenticated, redirect to home
    if (isAuthenticated) {
      navigate('/');
    }
  }, [isAuthenticated, navigate]);
  
  return (
    <Container maxWidth="md">
      <Grid container spacing={4} justifyContent="center" alignItems="center" sx={{ minHeight: '70vh' }}>
        <Grid item xs={12} md={6}>
          <Typography variant="h4" component="h1" gutterBottom fontWeight="bold">
            Create an Account
          </Typography>
          <Typography variant="body1" color="text.secondary" paragraph>
            Join ElectricityShop to enjoy a personalized shopping experience, faster checkout, and exclusive offers.
          </Typography>
          <Box sx={{ mt: 4, display: { xs: 'none', md: 'block' } }}>
            <Paper
              elevation={0}
              sx={{
                p: 3,
                bgcolor: 'secondary.light',
                color: 'secondary.contrastText',
                borderRadius: 2,
              }}
            >
              <Typography variant="h6" gutterBottom>
                Why create an account?
              </Typography>
              <ul>
                <li>Save multiple shipping addresses</li>
                <li>Track and manage your orders</li>
                <li>Receive personalized recommendations</li>
                <li>Get exclusive access to deals and promotions</li>
                <li>Faster checkout process</li>
              </ul>
            </Paper>
          </Box>
        </Grid>
        
        <Grid item xs={12} md={6}>
          <RegisterForm />
        </Grid>
      </Grid>
    </Container>
  );
};

export default RegisterPage;
