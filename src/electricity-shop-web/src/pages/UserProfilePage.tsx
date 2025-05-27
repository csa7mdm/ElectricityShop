import React, { useState } from 'react';
import {
  Container,
  Grid,
  Typography,
  Box,
  Paper,
  Avatar,
  Button,
  TextField,
  Tabs,
  Tab,
  Divider,
  List,
  ListItem,
  ListItemText,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Alert,
  Snackbar,
  FormControlLabel,
  Checkbox,
} from '@mui/material';
import {
  Person as PersonIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Add as AddIcon,
  Home as HomeIcon,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';

import { useAuth } from '../hooks/useAuth';
import { addressSchema, profileSchema } from '../utils/validation';
import { Address } from '../types';

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

const TabPanel: React.FC<TabPanelProps> = ({ children, value, index, ...other }) => {
  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`profile-tabpanel-${index}`}
      aria-labelledby={`profile-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ py: 3 }}>{children}</Box>}
    </div>
  );
};

// Mock data
const addresses: Address[] = [
  {
    id: '1',
    userId: '1',
    street: '123 Main St',
    city: 'Circuit City',
    state: 'CA',
    zipCode: '91234',
    country: 'USA',
    isDefault: true,
  },
  {
    id: '2',
    userId: '1',
    street: '456 Oak Ave',
    city: 'Voltage Town',
    state: 'NY',
    zipCode: '12345',
    country: 'USA',
    isDefault: false,
  },
];

const UserProfilePage: React.FC = () => {
  const { user } = useAuth();
  const [tabValue, setTabValue] = useState(0);
  const [editProfileMode, setEditProfileMode] = useState(false);
  const [addressDialogOpen, setAddressDialogOpen] = useState(false);
  const [selectedAddress, setSelectedAddress] = useState<Address | null>(null);
  const [notification, setNotification] = useState({
    open: false,
    message: '',
    severity: 'success' as 'success' | 'error',
  });
  
  // Profile form setup
  const {
    control: profileControl,
    handleSubmit: handleProfileSubmit,
    formState: { errors: profileErrors },
  } = useForm({
    resolver: yupResolver(profileSchema),
    defaultValues: {
      firstName: user?.firstName || '',
      lastName: user?.lastName || '',
      email: user?.email || '',
    },
  });
  
  // Address form setup
  const {
    control: addressControl,
    handleSubmit: handleAddressSubmit,
    formState: { errors: addressErrors },
    reset: resetAddressForm,
  } = useForm({
    resolver: yupResolver(addressSchema),
    defaultValues: {
      street: '',
      city: '',
      state: '',
      zipCode: '',
      country: '',
      isDefault: false,
    },
  });
  
  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };
  
  // Handle profile update
  const onProfileSubmit = (data: any) => {
    console.log('Profile data:', data);
    setEditProfileMode(false);
    setNotification({
      open: true,
      message: 'Profile updated successfully!',
      severity: 'success',
    });
  };
  
  // Handle address dialog open
  const handleAddAddress = () => {
    setSelectedAddress(null);
    resetAddressForm({
      street: '',
      city: '',
      state: '',
      zipCode: '',
      country: '',
      isDefault: false,
    });
    setAddressDialogOpen(true);
  };
  
  // Handle edit address
  const handleEditAddress = (address: Address) => {
    setSelectedAddress(address);
    resetAddressForm({
      street: address.street,
      city: address.city,
      state: address.state,
      zipCode: address.zipCode,
      country: address.country,
      isDefault: address.isDefault,
    });
    setAddressDialogOpen(true);
  };
  
  // Handle delete address
  const handleDeleteAddress = (addressId: string) => {
    console.log('Delete address:', addressId);
    setNotification({
      open: true,
      message: 'Address deleted successfully!',
      severity: 'success',
    });
  };
  
  // Handle address submit
  const onAddressSubmit = (data: any) => {
    console.log('Address data:', data);
    setAddressDialogOpen(false);
    setNotification({
      open: true,
      message: selectedAddress
        ? 'Address updated successfully!'
        : 'Address added successfully!',
      severity: 'success',
    });
  };
  
  // Close notification
  const handleCloseNotification = () => {
    setNotification({ ...notification, open: false });
  };
  
  return (
    <Container maxWidth="lg">
      <Grid container spacing={4}>
        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 3, mb: 3 }}>
            <Box
              sx={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                mb: 3,
              }}
            >
              <Avatar
                sx={{
                  width: 100,
                  height: 100,
                  bgcolor: 'primary.main',
                  mb: 2,
                }}
              >
                <PersonIcon sx={{ fontSize: 60 }} />
              </Avatar>
              <Typography variant="h5" gutterBottom>
                {user?.firstName} {user?.lastName}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {user?.email}
              </Typography>
              <Typography
                variant="body2"
                color="primary"
                sx={{ mt: 1, fontWeight: 'bold' }}
              >
                {user?.role === 'admin' ? 'Administrator' : 'Customer'}
              </Typography>
            </Box>
            
            <Divider sx={{ my: 2 }} />
            
            <Box>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                Account created
              </Typography>
              <Typography variant="body1">
                {new Date(user?.createdAt || '').toLocaleDateString()}
              </Typography>
            </Box>
          </Paper>
          
          {user?.role === 'admin' && (
            <Paper sx={{ p: 3 }}>
              <Typography variant="h6" gutterBottom>
                Admin Access
              </Typography>
              <Button
                variant="contained"
                color="primary"
                fullWidth
                sx={{ mb: 2 }}
                component="a"
                href="/admin/dashboard"
              >
                Admin Dashboard
              </Button>
              <Button
                variant="outlined"
                fullWidth
                sx={{ mb: 2 }}
                component="a"
                href="/admin/products"
              >
                Manage Products
              </Button>
              <Button
                variant="outlined"
                fullWidth
                component="a"
                href="/admin/orders"
              >
                Manage Orders
              </Button>
            </Paper>
          )}
        </Grid>
        
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 3 }}>
            <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
              <Tabs
                value={tabValue}
                onChange={handleTabChange}
                aria-label="profile tabs"
              >
                <Tab label="Profile" id="profile-tab-0" />
                <Tab label="Addresses" id="profile-tab-1" />
                <Tab label="Security" id="profile-tab-2" />
              </Tabs>
            </Box>
            
            <TabPanel value={tabValue} index={0}>
              <Box
                sx={{
                  display: 'flex',
                  justifyContent: 'space-between',
                  alignItems: 'center',
                  mb: 3,
                }}
              >
                <Typography variant="h6">Personal Information</Typography>
                <Button
                  startIcon={<EditIcon />}
                  onClick={() => setEditProfileMode(!editProfileMode)}
                >
                  {editProfileMode ? 'Cancel' : 'Edit'}
                </Button>
              </Box>
              
              {editProfileMode ? (
                <form onSubmit={handleProfileSubmit(onProfileSubmit)}>
                  <Grid container spacing={2}>
                    <Grid item xs={12} sm={6}>
                      <Controller
                        name="firstName"
                        control={profileControl}
                        render={({ field }) => (
                          <TextField
                            {...field}
                            label="First Name"
                            fullWidth
                            required
                            error={!!profileErrors.firstName}
                            helperText={profileErrors.firstName?.message as string}
                          />
                        )}
                      />
                    </Grid>
                    
                    <Grid item xs={12} sm={6}>
                      <Controller
                        name="lastName"
                        control={profileControl}
                        render={({ field }) => (
                          <TextField
                            {...field}
                            label="Last Name"
                            fullWidth
                            required
                            error={!!profileErrors.lastName}
                            helperText={profileErrors.lastName?.message as string}
                          />
                        )}
                      />
                    </Grid>
                    
                    <Grid item xs={12}>
                      <Controller
                        name="email"
                        control={profileControl}
                        render={({ field }) => (
                          <TextField
                            {...field}
                            label="Email Address"
                            fullWidth
                            required
                            error={!!profileErrors.email}
                            helperText={profileErrors.email?.message as string}
                          />
                        )}
                      />
                    </Grid>
                    
                    <Grid item xs={12}>
                      <Box sx={{ display: 'flex', justifyContent: 'flex-end', mt: 2 }}>
                        <Button
                          variant="outlined"
                          sx={{ mr: 2 }}
                          onClick={() => setEditProfileMode(false)}
                        >
                          Cancel
                        </Button>
                        <Button type="submit" variant="contained" color="primary">
                          Save Changes
                        </Button>
                      </Box>
                    </Grid>
                  </Grid>
                </form>
              ) : (
                <Box>
                  <Grid container spacing={2}>
                    <Grid item xs={12} sm={6}>
                      <Typography variant="body2" color="text.secondary" gutterBottom>
                        First Name
                      </Typography>
                      <Typography variant="body1">{user?.firstName}</Typography>
                    </Grid>
                    
                    <Grid item xs={12} sm={6}>
                      <Typography variant="body2" color="text.secondary" gutterBottom>
                        Last Name
                      </Typography>
                      <Typography variant="body1">{user?.lastName}</Typography>
                    </Grid>
                    
                    <Grid item xs={12}>
                      <Typography variant="body2" color="text.secondary" gutterBottom>
                        Email Address
                      </Typography>
                      <Typography variant="body1">{user?.email}</Typography>
                    </Grid>
                  </Grid>
                </Box>
              )}
            </TabPanel>
            
            <TabPanel value={tabValue} index={1}>
              <Box
                sx={{
                  display: 'flex',
                  justifyContent: 'space-between',
                  alignItems: 'center',
                  mb: 3,
                }}
              >
                <Typography variant="h6">Your Addresses</Typography>
                <Button
                  variant="contained"
                  color="primary"
                  startIcon={<AddIcon />}
                  onClick={handleAddAddress}
                >
                  Add New Address
                </Button>
              </Box>
              
              {addresses.length === 0 ? (
                <Alert severity="info">
                  You don't have any saved addresses yet. Add an address to make checkout faster.
                </Alert>
              ) : (
                <List>
                  {addresses.map((address) => (
                    <Paper
                      key={address.id}
                      variant="outlined"
                      sx={{ mb: 2, position: 'relative' }}
                    >
                      <ListItem
                        secondaryAction={
                          <Box>
                            <IconButton
                              edge="end"
                              onClick={() => handleEditAddress(address)}
                            >
                              <EditIcon />
                            </IconButton>
                            <IconButton
                              edge="end"
                              color="error"
                              onClick={() => handleDeleteAddress(address.id)}
                            >
                              <DeleteIcon />
                            </IconButton>
                          </Box>
                        }
                      >
                        <ListItemText
                          primary={
                            <Box sx={{ display: 'flex', alignItems: 'center' }}>
                              <HomeIcon sx={{ mr: 1 }} />
                              <Typography variant="subtitle1">
                                {address.street}
                                {address.isDefault && (
                                  <Box
                                    component="span"
                                    sx={{
                                      ml: 1,
                                      fontWeight: 'normal',
                                      fontSize: '0.75rem',
                                      bgcolor: 'primary.main',
                                      color: 'primary.contrastText',
                                      borderRadius: 1,
                                      px: 1,
                                      py: 0.5,
                                    }}
                                  >
                                    Default
                                  </Box>
                                )}
                              </Typography>
                            </Box>
                          }
                          secondary={
                            <>
                              <Typography variant="body2" component="span" display="block">
                                {address.city}, {address.state} {address.zipCode}
                              </Typography>
                              <Typography variant="body2" component="span" display="block">
                                {address.country}
                              </Typography>
                            </>
                          }
                        />
                      </ListItem>
                    </Paper>
                  ))}
                </List>
              )}
            </TabPanel>
            
            <TabPanel value={tabValue} index={2}>
              <Typography variant="h6" gutterBottom>
                Password
              </Typography>
              
              <Box sx={{ mt: 3 }}>
                <Button variant="outlined" color="primary">
                  Change Password
                </Button>
              </Box>
              
              <Divider sx={{ my: 4 }} />
              
              <Typography variant="h6" gutterBottom>
                Account Settings
              </Typography>
              
              <Box sx={{ mt: 3 }}>
                <FormControlLabel
                  control={<Checkbox defaultChecked />}
                  label="Receive order updates via email"
                />
              </Box>
              
              <Box sx={{ mt: 1 }}>
                <FormControlLabel
                  control={<Checkbox defaultChecked />}
                  label="Receive promotional emails"
                />
              </Box>
              
              <Box sx={{ mt: 4 }}>
                <Button variant="outlined" color="error">
                  Delete Account
                </Button>
              </Box>
            </TabPanel>
          </Paper>
        </Grid>
      </Grid>
      
      {/* Address Dialog */}
      <Dialog open={addressDialogOpen} onClose={() => setAddressDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>
          {selectedAddress ? 'Edit Address' : 'Add New Address'}
        </DialogTitle>
        <form onSubmit={handleAddressSubmit(onAddressSubmit)}>
          <DialogContent>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <Controller
                  name="street"
                  control={addressControl}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      label="Street Address"
                      fullWidth
                      required
                      error={!!addressErrors.street}
                      helperText={addressErrors.street?.message as string}
                    />
                  )}
                />
              </Grid>
              
              <Grid item xs={12} sm={6}>
                <Controller
                  name="city"
                  control={addressControl}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      label="City"
                      fullWidth
                      required
                      error={!!addressErrors.city}
                      helperText={addressErrors.city?.message as string}
                    />
                  )}
                />
              </Grid>
              
              <Grid item xs={12} sm={6}>
                <Controller
                  name="state"
                  control={addressControl}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      label="State/Province"
                      fullWidth
                      required
                      error={!!addressErrors.state}
                      helperText={addressErrors.state?.message as string}
                    />
                  )}
                />
              </Grid>
              
              <Grid item xs={12} sm={6}>
                <Controller
                  name="zipCode"
                  control={addressControl}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      label="ZIP/Postal Code"
                      fullWidth
                      required
                      error={!!addressErrors.zipCode}
                      helperText={addressErrors.zipCode?.message as string}
                    />
                  )}
                />
              </Grid>
              
              <Grid item xs={12} sm={6}>
                <Controller
                  name="country"
                  control={addressControl}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      label="Country"
                      fullWidth
                      required
                      error={!!addressErrors.country}
                      helperText={addressErrors.country?.message as string}
                    />
                  )}
                />
              </Grid>
              
              <Grid item xs={12}>
                <Controller
                  name="isDefault"
                  control={addressControl}
                  render={({ field }) => (
                    <FormControlLabel
                      control={
                        <Checkbox
                          checked={field.value}
                          onChange={(e) => field.onChange(e.target.checked)}
                        />
                      }
                      label="Set as default address"
                    />
                  )}
                />
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setAddressDialogOpen(false)}>Cancel</Button>
            <Button type="submit" variant="contained" color="primary">
              {selectedAddress ? 'Update' : 'Add'}
            </Button>
          </DialogActions>
        </form>
      </Dialog>
      
      {/* Notification Snackbar */}
      <Snackbar
        open={notification.open}
        autoHideDuration={6000}
        onClose={handleCloseNotification}
        message={notification.message}
      />
    </Container>
  );
};

export default UserProfilePage;
