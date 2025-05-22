import React, { useState } from 'react';
import {
  Grid,
  Typography,
  TextField,
  FormControlLabel,
  Checkbox,
  Divider,
  Box,
  Button,
} from '@mui/material';
import { useForm, Controller, FormProvider, useFormContext } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';

import { addressSchema } from '../../../utils/validation';
import { Address } from '../../../types';

interface AddressFormProps {
  type: 'shipping' | 'billing';
}

// Sub-component for address form fields
const AddressFormFields: React.FC<AddressFormProps> = ({ type }) => {
  const { control, formState: { errors } } = useFormContext();
  
  return (
    <Grid container spacing={3}>
      <Grid item xs={12} sm={6}>
        <Controller
          name={`${type}.firstName`}
          control={control}
          defaultValue=""
          render={({ field }) => (
            <TextField
              {...field}
              fullWidth
              label="First Name"
              variant="outlined"
              required
              error={!!errors[`${type}.firstName`]}
              helperText={errors[`${type}.firstName`]?.message as string}
            />
          )}
        />
      </Grid>
      <Grid item xs={12} sm={6}>
        <Controller
          name={`${type}.lastName`}
          control={control}
          defaultValue=""
          render={({ field }) => (
            <TextField
              {...field}
              fullWidth
              label="Last Name"
              variant="outlined"
              required
              error={!!errors[`${type}.lastName`]}
              helperText={errors[`${type}.lastName`]?.message as string}
            />
          )}
        />
      </Grid>
      <Grid item xs={12}>
        <Controller
          name={`${type}.street`}
          control={control}
          defaultValue=""
          render={({ field }) => (
            <TextField
              {...field}
              fullWidth
              label="Street Address"
              variant="outlined"
              required
              error={!!errors[`${type}.street`]}
              helperText={errors[`${type}.street`]?.message as string}
            />
          )}
        />
      </Grid>
      <Grid item xs={12} sm={6}>
        <Controller
          name={`${type}.city`}
          control={control}
          defaultValue=""
          render={({ field }) => (
            <TextField
              {...field}
              fullWidth
              label="City"
              variant="outlined"
              required
              error={!!errors[`${type}.city`]}
              helperText={errors[`${type}.city`]?.message as string}
            />
          )}
        />
      </Grid>
      <Grid item xs={12} sm={6}>
        <Controller
          name={`${type}.state`}
          control={control}
          defaultValue=""
          render={({ field }) => (
            <TextField
              {...field}
              fullWidth
              label="State/Province/Region"
              variant="outlined"
              required
              error={!!errors[`${type}.state`]}
              helperText={errors[`${type}.state`]?.message as string}
            />
          )}
        />
      </Grid>
      <Grid item xs={12} sm={6}>
        <Controller
          name={`${type}.zipCode`}
          control={control}
          defaultValue=""
          render={({ field }) => (
            <TextField
              {...field}
              fullWidth
              label="Zip / Postal Code"
              variant="outlined"
              required
              error={!!errors[`${type}.zipCode`]}
              helperText={errors[`${type}.zipCode`]?.message as string}
            />
          )}
        />
      </Grid>
      <Grid item xs={12} sm={6}>
        <Controller
          name={`${type}.country`}
          control={control}
          defaultValue=""
          render={({ field }) => (
            <TextField
              {...field}
              fullWidth
              label="Country"
              variant="outlined"
              required
              error={!!errors[`${type}.country`]}
              helperText={errors[`${type}.country`]?.message as string}
            />
          )}
        />
      </Grid>
      <Grid item xs={12}>
        <Controller
          name={`${type}.phone`}
          control={control}
          defaultValue=""
          render={({ field }) => (
            <TextField
              {...field}
              fullWidth
              label="Phone Number"
              variant="outlined"
              required
              error={!!errors[`${type}.phone`]}
              helperText={errors[`${type}.phone`]?.message as string}
            />
          )}
        />
      </Grid>
    </Grid>
  );
};

interface ShippingFormProps {
  onSubmit: (shippingAddress: Address, useSameAddress: boolean, billingAddress?: Address) => void;
  initialShippingAddress: Address | null;
  initialBillingAddress: Address | null;
  initialUseSameAddress: boolean;
}

const ShippingForm: React.FC<ShippingFormProps> = ({
  onSubmit,
  initialShippingAddress,
  initialBillingAddress,
  initialUseSameAddress,
}) => {
  const [useSameAddress, setUseSameAddress] = useState(initialUseSameAddress);
  
  const methods = useForm({
    resolver: yupResolver(addressSchema),
defaultValues: {
  firstName: initialShippingAddress?.firstName || '',
  lastName: initialShippingAddress?.lastName || '',
  street: initialShippingAddress?.street || '',
  city: initialShippingAddress?.city || '',
  state: initialShippingAddress?.state || '',
  zipCode: initialShippingAddress?.zipCode || '',
  country: initialShippingAddress?.country || '',
  phone: initialShippingAddress?.phone || '',
},
  });

  const handleSubmit = methods.handleSubmit((data) => {
    onSubmit(data as unknown as Address, useSameAddress);
  });
  
  return (
    <FormProvider {...methods}>
      <form id="shipping-form" onSubmit={handleSubmit}>
        <Typography variant="h6" gutterBottom>
          Shipping Address
        </Typography>
        <AddressFormFields type="shipping" />
        
        <Box sx={{ mt: 3 }}>
          <FormControlLabel
            control={
              <Checkbox
                color="primary"
                checked={useSameAddress}
                onChange={(e) => setUseSameAddress(e.target.checked)}
              />
            }
            label="Use this address for billing"
          />
        </Box>
        
        {!useSameAddress && (
          <>
            <Divider sx={{ mt: 3, mb: 3 }} />
            <Typography variant="h6" gutterBottom>
              Billing Address
            </Typography>
            <AddressFormFields type="billing" />
          </>
        )}
      </form>
    </FormProvider>
  );
};

export default ShippingForm;
