import React from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { Container, Box, Typography, Button, Paper } from '@mui/material';
import { Home as HomeIcon, ShoppingCart as ShoppingCartIcon } from '@mui/icons-material';

const NotFoundPage: React.FC = () => {
  return (
    <Container maxWidth="md">
      <Paper
        elevation={3}
        sx={{
          textAlign: 'center',
          p: 6,
          my: 8,
          borderRadius: 2,
        }}
      >
        <Typography variant="h1" component="h1" gutterBottom color="error">
          404
        </Typography>
        <Typography variant="h4" component="h2" gutterBottom>
          Page Not Found
        </Typography>
        <Typography variant="body1" paragraph sx={{ mb: 4 }}>
          The page you are looking for does not exist or has been moved.
        </Typography>
        
        <Box 
          component="img" 
          src="/404-image.png" 
          alt="Page not found"
          sx={{ 
            width: '100%', 
            maxWidth: 400, 
            mb: 4,
            display: 'none', // Hide until image is available
          }} 
        />
        
        <Box sx={{ display: 'flex', justifyContent: 'center', gap: 2 }}>
          <Button
            variant="contained"
            color="primary"
            component={RouterLink}
            to="/"
            startIcon={<HomeIcon />}
          >
            Back to Home
          </Button>
          <Button
            variant="outlined"
            color="primary"
            component={RouterLink}
            to="/products"
            startIcon={<ShoppingCartIcon />}
          >
            Browse Products
          </Button>
        </Box>
      </Paper>
    </Container>
  );
};

export default NotFoundPage;
