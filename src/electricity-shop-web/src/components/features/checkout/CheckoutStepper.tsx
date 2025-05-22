import React, { useState } from 'react';
import {
  Box,
  Button,
  Paper,
  Step,
  StepLabel,
  Stepper,
  Typography,
  Container,
} from '@mui/material';
import {
  Navigation as NavigationIcon,
  Payment as PaymentIcon,
  Check as CheckIcon,
} from '@mui/icons-material';

import ShippingForm from './ShippingForm';
import PaymentForm from './PaymentForm';
import ReviewOrder from './ReviewOrder';
import { Address } from '../../../types';
import { useOrder } from '../../../hooks/useOrder';

const steps = ['Shipping', 'Payment', 'Review Order'];

interface CheckoutStepperProps {
  onOrderPlaced: () => void;
}

const CheckoutStepper: React.FC<CheckoutStepperProps> = ({ onOrderPlaced }) => {
  const [activeStep, setActiveStep] = useState(0);
  const [shippingAddress, setShippingAddress] = useState<Address | null>(null);
  const [billingAddress, setBillingAddress] = useState<Address | null>(null);
  const [useSameAddress, setUseSameAddress] = useState(true);
  const [paymentMethod, setPaymentMethod] = useState('credit-card');
  
  const { createOrder, isLoading } = useOrder();
  
  const handleNext = () => {
    setActiveStep((prevActiveStep) => prevActiveStep + 1);
  };
  
  const handleBack = () => {
    setActiveStep((prevActiveStep) => prevActiveStep - 1);
  };
  
  const handleShippingSubmit = (shippingData: Address, sameAsBilling: boolean, billingData?: Address) => {
    setShippingAddress(shippingData);
    setUseSameAddress(sameAsBilling);
    
    if (sameAsBilling) {
      setBillingAddress(shippingData);
    } else if (billingData) {
      setBillingAddress(billingData);
    }
    
    handleNext();
  };
  
  const handlePaymentSubmit = (method: string) => {
    setPaymentMethod(method);
    handleNext();
  };
  
  const handlePlaceOrder = async () => {
    if (shippingAddress && billingAddress) {
      const success = await createOrder(
        shippingAddress,
        billingAddress,
        paymentMethod
      );
      
      if (success) {
        handleNext();
        onOrderPlaced();
      }
    }
  };
  
  const getStepContent = (step: number) => {
    switch (step) {
      case 0:
        return (
          <ShippingForm
            onSubmit={handleShippingSubmit}
            initialShippingAddress={shippingAddress}
            initialBillingAddress={billingAddress}
            initialUseSameAddress={useSameAddress}
          />
        );
      case 1:
        return <PaymentForm onSubmit={handlePaymentSubmit} />;
      case 2:
        return (
          <ReviewOrder
            shippingAddress={shippingAddress!}
            billingAddress={billingAddress!}
            paymentMethod={paymentMethod}
          />
        );
      default:
        throw new Error('Unknown step');
    }
  };
  
  return (
    <Container component="main" maxWidth="md" sx={{ mb: 4 }}>
      <Paper variant="outlined" sx={{ my: { xs: 3, md: 6 }, p: { xs: 2, md: 3 } }}>
        <Typography component="h1" variant="h4" align="center" gutterBottom>
          Checkout
        </Typography>
        <Stepper activeStep={activeStep} sx={{ pt: 3, pb: 5 }}>
          {steps.map((label) => (
            <Step key={label}>
              <StepLabel>{label}</StepLabel>
            </Step>
          ))}
        </Stepper>
        
        {activeStep === steps.length ? (
          <Box sx={{ p: 3, textAlign: 'center' }}>
            <CheckIcon color="success" sx={{ fontSize: 60, mb: 2 }} />
            <Typography variant="h5" gutterBottom>
              Thank you for your order!
            </Typography>
            <Typography variant="subtitle1">
              Your order has been placed successfully. We have emailed your order
              confirmation, and will send you an update when your order has shipped.
            </Typography>
            <Button
              variant="contained"
              color="primary"
              href="/orders"
              sx={{ mt: 3 }}
            >
              View Orders
            </Button>
          </Box>
        ) : (
          <>
            {getStepContent(activeStep)}
            <Box sx={{ display: 'flex', justifyContent: 'flex-end', mt: 3 }}>
              {activeStep !== 0 && (
                <Button
                  onClick={handleBack}
                  sx={{ mr: 1 }}
                  disabled={isLoading}
                >
                  Back
                </Button>
              )}
              
              {activeStep === steps.length - 1 ? (
                <Button
                  variant="contained"
                  color="primary"
                  onClick={handlePlaceOrder}
                  startIcon={<NavigationIcon />}
                  disabled={isLoading}
                >
                  {isLoading ? 'Processing...' : 'Place Order'}
                </Button>
              ) : (
                <Button
                  variant="contained"
                  color="primary"
                  type={activeStep === 0 ? 'submit' : 'button'}
                  form={activeStep === 0 ? 'shipping-form' : undefined}
                  onClick={activeStep === 1 ? () => handlePaymentSubmit(paymentMethod) : undefined}
                  disabled={isLoading}
                >
                  Next
                </Button>
              )}
            </Box>
          </>
        )}
      </Paper>
    </Container>
  );
};

export default CheckoutStepper;
