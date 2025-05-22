import React, { useEffect } from 'react';
import {
  Typography,
  List,
  ListItem,
  ListItemText,
  Grid,
  Paper,
  Divider,
  Box,
  Chip,
} from '@mui/material';
import {
  CreditCard as CreditCardIcon,
  AccountBalance as BankIcon,
  Payment as PaymentIcon,
} from '@mui/icons-material';

import { useCart } from '../../../hooks/useCart';
import { formatPrice } from '../../../utils/formatters';
import { Address } from '../../../types';

const TAX_RATE = 0.07; // 7% tax rate
const SHIPPING_COST = 5.99;

interface ReviewOrderProps {
  shippingAddress: Address;
  billingAddress: Address;
  paymentMethod: string;
}

const ReviewOrder: React.FC<ReviewOrderProps> = ({
  shippingAddress,
  billingAddress,
  paymentMethod,
}) => {
  const { cart, fetchCart } = useCart();
  
  useEffect(() => {
    if (!cart) {
      fetchCart();
    }
  }, [cart, fetchCart]);
  
  if (!cart) {
    return <Typography>Loading cart details...</Typography>;
  }
  
  const subtotal = cart.totalPrice;
  const tax = subtotal * TAX_RATE;
  const total = subtotal + tax + SHIPPING_COST;
  
  // Get payment method icon
  const getPaymentIcon = () => {
    switch (paymentMethod) {
      case 'credit-card':
        return <CreditCardIcon />;
      case 'bank-transfer':
        return <BankIcon />;
      case 'paypal':
        return <PaymentIcon />;
      default:
        return <CreditCardIcon />;
    }
  };
  
  // Format payment method for display
  const formatPaymentMethod = (method: string) => {
    switch (method) {
      case 'credit-card':
        return 'Credit Card';
      case 'bank-transfer':
        return 'Bank Transfer';
      case 'paypal':
        return 'PayPal';
      default:
        return method;
    }
  };
  
  // Format address for display
  const formatAddress = (address: Address) => {
    return [
      `${address.firstName} ${address.lastName}`,
      address.street,
      `${address.city}, ${address.state} ${address.zipCode}`,
      address.country,
      address.phone,
    ];
  };
  
  return (
    <>
      <Typography variant="h6" gutterBottom>
        Order Summary
      </Typography>
      
      <Paper variant="outlined" sx={{ p: 2, mb: 3 }}>
        <List disablePadding>
          {cart.items.map((item) => (
            <ListItem key={item.id} sx={{ py: 1 }}>
              <Box sx={{ mr: 2, width: 50, height: 50, flexShrink: 0 }}>
                <img
                  src={item.product.imageUrl}
                  alt={item.product.name}
                  style={{ width: '100%', height: '100%', objectFit: 'contain' }}
                />
              </Box>
              <ListItemText
                primary={item.product.name}
                secondary={`Qty: ${item.quantity}`}
              />
              <Typography variant="body2">{formatPrice(item.price * item.quantity)}</Typography>
            </ListItem>
          ))}
          
          <Divider sx={{ my: 2 }} />
          
          <ListItem>
            <ListItemText primary="Subtotal" />
            <Typography variant="body1">{formatPrice(subtotal)}</Typography>
          </ListItem>
          <ListItem>
            <ListItemText primary="Tax (7%)" />
            <Typography variant="body1">{formatPrice(tax)}</Typography>
          </ListItem>
          <ListItem>
            <ListItemText primary="Shipping" />
            <Typography variant="body1">{formatPrice(SHIPPING_COST)}</Typography>
          </ListItem>
          <ListItem>
            <ListItemText primary="Total" />
            <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
              {formatPrice(total)}
            </Typography>
          </ListItem>
        </List>
      </Paper>
      
      <Grid container spacing={2}>
        <Grid item xs={12} sm={6}>
          <Typography variant="h6" gutterBottom>
            Shipping
          </Typography>
          <Paper variant="outlined" sx={{ p: 2 }}>
            {formatAddress(shippingAddress).map((line, index) => (
              <Typography key={index} variant="body1">
                {line}
              </Typography>
            ))}
          </Paper>
        </Grid>
        <Grid item xs={12} sm={6}>
          <Typography variant="h6" gutterBottom>
            Billing
          </Typography>
          <Paper variant="outlined" sx={{ p: 2 }}>
            {formatAddress(billingAddress).map((line, index) => (
              <Typography key={index} variant="body1">
                {line}
              </Typography>
            ))}
          </Paper>
        </Grid>
        <Grid item xs={12}>
          <Typography variant="h6" gutterBottom>
            Payment Details
          </Typography>
          <Paper variant="outlined" sx={{ p: 2 }}>
            <Box sx={{ display: 'flex', alignItems: 'center' }}>
              {getPaymentIcon()}
              <Typography sx={{ ml: 1 }} variant="body1">
                {formatPaymentMethod(paymentMethod)}
              </Typography>
            </Box>
          </Paper>
        </Grid>
      </Grid>
    </>
  );
};

export default ReviewOrder;
