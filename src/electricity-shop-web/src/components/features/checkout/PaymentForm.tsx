import React, { useState } from 'react';
import {
  Typography,
  Grid,
  TextField,
  FormControlLabel,
  Radio,
  RadioGroup,
  FormControl,
  FormLabel,
  Paper,
  InputAdornment,
  Box,
  Divider,
} from '@mui/material';
import {
  CreditCard as CreditCardIcon,
  AccountBalance as BankIcon,
  Payment as PaymentIcon,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';

import { paymentSchema } from '../../../utils/validation';

interface PaymentFormProps {
  onSubmit: (paymentMethod: string) => void;
}

const PaymentForm: React.FC<PaymentFormProps> = ({ onSubmit }) => {
  const [paymentMethod, setPaymentMethod] = useState('credit-card');
  
  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm({
    resolver: yupResolver(paymentSchema),
    defaultValues: {
      cardName: '',
      cardNumber: '',
      expiryDate: '',
      cvv: '',
    },
  });
  
  const handlePaymentMethodChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setPaymentMethod(event.target.value);
    onSubmit(event.target.value);
  };
  
  return (
    <>
      <Typography variant="h6" gutterBottom>
        Payment Method
      </Typography>
      <FormControl component="fieldset" sx={{ width: '100%', mb: 3 }}>
        <RadioGroup
          aria-label="Payment Method"
          name="paymentMethod"
          value={paymentMethod}
          onChange={handlePaymentMethodChange}
        >
          <Grid container spacing={2}>
            <Grid item xs={12} md={4}>
              <Paper
                variant="outlined"
                sx={{
                  p: 2,
                  borderColor: paymentMethod === 'credit-card' ? 'primary.main' : 'divider',
                  bgcolor: paymentMethod === 'credit-card' ? 'primary.light' : 'background.paper',
                  '&:hover': {
                    borderColor: 'primary.main',
                  },
                }}
              >
                <FormControlLabel
                  value="credit-card"
                  control={<Radio />}
                  label={
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                      <CreditCardIcon sx={{ mr: 1 }} />
                      Credit Card
                    </Box>
                  }
                  sx={{ width: '100%' }}
                />
              </Paper>
            </Grid>
            <Grid item xs={12} md={4}>
              <Paper
                variant="outlined"
                sx={{
                  p: 2,
                  borderColor: paymentMethod === 'bank-transfer' ? 'primary.main' : 'divider',
                  bgcolor: paymentMethod === 'bank-transfer' ? 'primary.light' : 'background.paper',
                  '&:hover': {
                    borderColor: 'primary.main',
                  },
                }}
              >
                <FormControlLabel
                  value="bank-transfer"
                  control={<Radio />}
                  label={
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                      <BankIcon sx={{ mr: 1 }} />
                      Bank Transfer
                    </Box>
                  }
                  sx={{ width: '100%' }}
                />
              </Paper>
            </Grid>
            <Grid item xs={12} md={4}>
              <Paper
                variant="outlined"
                sx={{
                  p: 2,
                  borderColor: paymentMethod === 'paypal' ? 'primary.main' : 'divider',
                  bgcolor: paymentMethod === 'paypal' ? 'primary.light' : 'background.paper',
                  '&:hover': {
                    borderColor: 'primary.main',
                  },
                }}
              >
                <FormControlLabel
                  value="paypal"
                  control={<Radio />}
                  label={
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                      <PaymentIcon sx={{ mr: 1 }} />
                      PayPal
                    </Box>
                  }
                  sx={{ width: '100%' }}
                />
              </Paper>
            </Grid>
          </Grid>
        </RadioGroup>
      </FormControl>
      
      <Divider sx={{ mb: 3 }} />
      
      {paymentMethod === 'credit-card' && (
        <form onSubmit={handleSubmit(() => onSubmit(paymentMethod))}>
          <Typography variant="h6" gutterBottom>
            Card Details
          </Typography>
          <Grid container spacing={3}>
            <Grid item xs={12}>
              <Controller
                name="cardName"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    fullWidth
                    label="Name on card"
                    variant="outlined"
                    error={!!errors.cardName}
                    helperText={errors.cardName?.message as string}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12}>
              <Controller
                name="cardNumber"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    fullWidth
                    label="Card number"
                    variant="outlined"
                    inputProps={{ maxLength: 16 }}
                    InputProps={{
                      startAdornment: (
                        <InputAdornment position="start">
                          <CreditCardIcon />
                        </InputAdornment>
                      ),
                    }}
                    error={!!errors.cardNumber}
                    helperText={errors.cardNumber?.message as string}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <Controller
                name="expiryDate"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    fullWidth
                    label="Expiry date"
                    placeholder="MM/YY"
                    variant="outlined"
                    inputProps={{ maxLength: 5 }}
                    error={!!errors.expiryDate}
                    helperText={errors.expiryDate?.message as string}
                  />
                )}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <Controller
                name="cvv"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    fullWidth
                    label="CVV"
                    variant="outlined"
                    type="password"
                    inputProps={{ maxLength: 4 }}
                    error={!!errors.cvv}
                    helperText={errors.cvv?.message as string}
                  />
                )}
              />
            </Grid>
          </Grid>
        </form>
      )}
      
      {paymentMethod === 'bank-transfer' && (
        <Box sx={{ mt: 2 }}>
          <Typography variant="body1" paragraph>
            Please use the following details to make a bank transfer:
          </Typography>
          <Typography variant="body2">
            <strong>Bank Name:</strong> Example Bank
          </Typography>
          <Typography variant="body2">
            <strong>Account Name:</strong> ElectricityShop Inc.
          </Typography>
          <Typography variant="body2">
            <strong>Account Number:</strong> 1234567890
          </Typography>
          <Typography variant="body2">
            <strong>Routing Number:</strong> 123456789
          </Typography>
          <Typography variant="body2">
            <strong>Reference:</strong> Your order number will be provided after confirmation
          </Typography>
        </Box>
      )}
      
      {paymentMethod === 'paypal' && (
        <Box sx={{ mt: 2 }}>
          <Typography variant="body1" paragraph>
            You will be redirected to PayPal to complete your payment after reviewing your order.
          </Typography>
        </Box>
      )}
    </>
  );
};

export default PaymentForm;
