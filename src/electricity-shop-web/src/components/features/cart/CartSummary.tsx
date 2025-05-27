import React from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Button,
  Card,
  CardContent,
  CardActions,
  Divider,
  Typography,
  Paper,
} from '@mui/material';
import ShoppingCartCheckoutIcon from '@mui/icons-material/ShoppingCartCheckout';

import { Cart } from '../../../types';
import { formatPrice } from '../../../utils/formatters';
import { useAuth } from '../../../hooks/useAuth';

interface CartSummaryProps {
  cart: Cart | null;
  loading: boolean;
}

const TAX_RATE = 0.07; // 7% tax rate

const CartSummary: React.FC<CartSummaryProps> = ({ cart, loading }) => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  
  const handleCheckout = () => {
    if (!isAuthenticated) {
      navigate('/login', { state: { from: '/checkout' } });
    } else {
      navigate('/checkout');
    }
  };
  
  if (!cart) {
    return null;
  }
  
  const subtotal = cart.totalPrice || 0;
  const tax = subtotal * TAX_RATE;
  const total = subtotal + tax;
  
  return (
    <Card sx={{ position: 'sticky', top: 20 }}>
      <CardContent>
        <Typography variant="h6" gutterBottom fontWeight="bold">
          Order Summary
        </Typography>
        
        <Box sx={{ my: 2 }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
            <Typography variant="body1">Subtotal ({cart.totalQuantity} items)</Typography>
            <Typography variant="body1">{formatPrice(subtotal)}</Typography>
          </Box>
          
          <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
            <Typography variant="body1">Shipping & Handling</Typography>
            <Typography variant="body1">Free</Typography>
          </Box>
          
          <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
            <Typography variant="body1">Tax (7%)</Typography>
            <Typography variant="body1">{formatPrice(tax)}</Typography>
          </Box>
        </Box>
        
        <Divider sx={{ my: 2 }} />
        
        <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
          <Typography variant="h6">Total</Typography>
          <Typography variant="h6" color="primary" fontWeight="bold">
            {formatPrice(total)}
          </Typography>
        </Box>
      </CardContent>
      
      <CardActions sx={{ p: 2, pt: 0 }}>
        <Button
          variant="contained"
          color="primary"
          fullWidth
          size="large"
          startIcon={<ShoppingCartCheckoutIcon />}
          disabled={cart.items.length === 0 || loading}
          onClick={handleCheckout}
        >
          Proceed to Checkout
        </Button>
      </CardActions>
      
      <Box sx={{ p: 2, pt: 0 }}>
        <Paper 
          variant="outlined" 
          sx={{ 
            p: 1.5, 
            backgroundColor: 'info.light', 
            color: 'info.contrastText',
            borderColor: 'info.main',
          }}
        >
          <Typography variant="body2">
            {isAuthenticated 
              ? 'Free shipping on all orders!'
              : 'Log in to your account for a personalized shopping experience.'}
          </Typography>
        </Paper>
      </Box>
    </Card>
  );
};

export default CartSummary;
