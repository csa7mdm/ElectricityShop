import React, { useEffect } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import {
  Container,
  Grid,
  Typography,
  Box,
  Button,
  Divider,
  Alert,
  AlertTitle,
  Breadcrumbs,
  Link as MuiLink,
  Paper,
} from '@mui/material';
import {
  ShoppingCart as ShoppingCartIcon,
  NavigateNext as NavigateNextIcon,
  DeleteOutline as DeleteOutlineIcon,
  ShoppingCartCheckout as ShoppingCartCheckoutIcon,
} from '@mui/icons-material';

import CartItem from '../components/features/cart/CartItem';
import CartSummary from '../components/features/cart/CartSummary';
import LoadingSpinner from '../components/common/LoadingSpinner';
import { useCart } from '../hooks/useCart';

const CartPage: React.FC = () => {
  const { cart, isLoading, error, fetchCart, updateCartItem, removeFromCart, clearCart } = useCart();
  
  useEffect(() => {
    fetchCart();
  }, [fetchCart]);
  
  const handleUpdateQuantity = (itemId: string, quantity: number) => {
    updateCartItem(itemId, quantity);
  };
  
  const handleRemoveItem = (itemId: string) => {
    removeFromCart(itemId);
  };
  
  const handleClearCart = () => {
    clearCart();
  };
  
  return (
    <Container maxWidth="lg">
      <LoadingSpinner isOpen={isLoading && !cart} message="Loading your cart..." />
      
      {/* Breadcrumbs */}
      <Box mb={4}>
        <Breadcrumbs separator={<NavigateNextIcon fontSize="small" />}>
          <MuiLink component={RouterLink} to="/" color="inherit">
            Home
          </MuiLink>
          <Typography color="text.primary">Shopping Cart</Typography>
        </Breadcrumbs>
      </Box>
      
      <Typography variant="h4" component="h1" gutterBottom sx={{ mb: 4 }}>
        <ShoppingCartIcon sx={{ mr: 1, verticalAlign: 'text-bottom' }} />
        Shopping Cart
      </Typography>
      
      {error && (
        <Alert severity="error" sx={{ mb: 4 }}>
          <AlertTitle>Error</AlertTitle>
          {error}
        </Alert>
      )}
      
      {!isLoading && (!cart || cart.items.length === 0) ? (
        <Paper sx={{ p: 4, textAlign: 'center' }}>
          <Typography variant="h5" gutterBottom>
            Your cart is empty
          </Typography>
          <Typography variant="body1" color="text.secondary" paragraph>
            Looks like you haven't added any products to your cart yet.
          </Typography>
          <Button
            variant="contained"
            color="primary"
            component={RouterLink}
            to="/products"
            startIcon={<ShoppingCartIcon />}
            sx={{ mt: 2 }}
          >
            Continue Shopping
          </Button>
        </Paper>
      ) : (
        <Grid container spacing={4}>
          {/* Cart Items */}
          <Grid item xs={12} md={8}>
            {!isLoading && cart && cart.items.length > 0 && (
              <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
                <Button
                  color="error"
                  startIcon={<DeleteOutlineIcon />}
                  onClick={handleClearCart}
                >
                  Clear Cart
                </Button>
              </Box>
            )}
            
            {isLoading && !cart ? (
              // Loading skeleton would go here
              <div>Loading...</div>
            ) : (
              cart?.items.map((item) => (
                <CartItem
                  key={item.id}
                  item={item}
                  onUpdateQuantity={handleUpdateQuantity}
                  onRemove={handleRemoveItem}
                />
              ))
            )}
            
            <Box sx={{ mt: 4 }}>
              <Button
                variant="outlined"
                component={RouterLink}
                to="/products"
                sx={{ mr: 2 }}
              >
                Continue Shopping
              </Button>
            </Box>
          </Grid>
          
          {/* Order Summary */}
          <Grid item xs={12} md={4}>
            <CartSummary cart={cart} loading={isLoading} />
          </Grid>
        </Grid>
      )}
    </Container>
  );
};

export default CartPage;
