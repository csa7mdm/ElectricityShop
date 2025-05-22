import React, { useEffect, useState } from 'react';
import { useNavigate, Link as RouterLink } from 'react-router-dom';
import {
  Container,
  Typography,
  Box,
  Alert,
  AlertTitle,
  Button,
  Breadcrumbs,
  Link as MuiLink,
} from '@mui/material';
import { NavigateNext as NavigateNextIcon } from '@mui/icons-material';

import CheckoutStepper from '../components/features/checkout/CheckoutStepper';
import LoadingSpinner from '../components/common/LoadingSpinner';
import { useCart } from '../hooks/useCart';
import { useAuth } from '../hooks/useAuth';

const CheckoutPage: React.FC = () => {
  const navigate = useNavigate();
  const { cart, isLoading, error, fetchCart, resetCart } = useCart();
  const { isAuthenticated } = useAuth();
  const [orderPlaced, setOrderPlaced] = useState(false);
  
  useEffect(() => {
    // Redirect to login if not authenticated
    if (!isAuthenticated) {
      navigate('/login', { state: { from: '/checkout' } });
      return;
    }
    
    // Fetch cart if needed
    if (!cart) {
      fetchCart();
    }
  }, [isAuthenticated, navigate, cart, fetchCart]);
  
  // Redirect to cart if cart is empty
  useEffect(() => {
    if (!isLoading && cart && cart.items.length === 0 && !orderPlaced) {
      navigate('/cart');
    }
  }, [isLoading, cart, navigate, orderPlaced]);
  
  const handleOrderPlaced = () => {
    setOrderPlaced(true);
    resetCart();
  };
  
  if (isLoading) {
    return <LoadingSpinner isOpen={true} message="Loading checkout..." />;
  }
  
  if (error) {
    return (
      <Container maxWidth="md">
        <Alert severity="error" sx={{ my: 4 }}>
          <AlertTitle>Error</AlertTitle>
          {error}
        </Alert>
        <Button
          variant="contained"
          color="primary"
          component={RouterLink}
          to="/cart"
        >
          Return to Cart
        </Button>
      </Container>
    );
  }
  
  return (
    <Container maxWidth="lg">
      {/* Breadcrumbs */}
      <Box mb={4}>
        <Breadcrumbs separator={<NavigateNextIcon fontSize="small" />}>
          <MuiLink component={RouterLink} to="/" color="inherit">
            Home
          </MuiLink>
          <MuiLink component={RouterLink} to="/cart" color="inherit">
            Cart
          </MuiLink>
          <Typography color="text.primary">Checkout</Typography>
        </Breadcrumbs>
      </Box>
      
      <CheckoutStepper onOrderPlaced={handleOrderPlaced} />
    </Container>
  );
};

export default CheckoutPage;
