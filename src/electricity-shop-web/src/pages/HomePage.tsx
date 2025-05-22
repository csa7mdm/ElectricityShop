import React, { useEffect, useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import {
  Box,
  Button,
  Card,
  CardContent,
  Container,
  Grid,
  Typography,
  useTheme,
  useMediaQuery,
  Paper,
  Divider,
} from '@mui/material';
import { 
  ShoppingCart as ShoppingCartIcon,
  ElectricBolt as ElectricBoltIcon,
  LocalShipping as LocalShippingIcon,
  CreditCard as CreditCardIcon,
  Bolt as BoltIcon,
} from '@mui/icons-material';

import ProductCard from '../components/features/products/ProductCard';
import { useProduct } from '../hooks/useProduct';
import { Product } from '../types';

const HomePage: React.FC = () => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const { products, isLoading, fetchProducts } = useProduct();
  const [featuredProducts, setFeaturedProducts] = useState<Product[]>([]);
  
  useEffect(() => {
    fetchProducts({ page: 1, limit: 6, sortBy: 'rating', sortDirection: 'desc' });
  }, [fetchProducts]);
  
  useEffect(() => {
    // Set featured products when products are loaded
    if (products.length > 0) {
      setFeaturedProducts(products.slice(0, 3));
    }
  }, [products]);
  
  const heroSectionStyles = {
    backgroundImage: 'linear-gradient(rgba(0, 0, 0, 0.5), rgba(0, 0, 0, 0.5)), url(/hero-image.jpg)',
    backgroundSize: 'cover',
    backgroundPosition: 'center',
    color: 'white',
    py: 8,
    px: 2,
    mb: 6,
    borderRadius: 2,
    position: 'relative' as const,
  };
  
  return (
    <Container maxWidth="lg">
      {/* Hero Section */}
      <Paper elevation={0} sx={heroSectionStyles}>
        <Box 
          sx={{
            maxWidth: 600,
            mx: 'auto',
            textAlign: 'center',
          }}
        >
          <ElectricBoltIcon sx={{ fontSize: 60, mb: 2 }} />
          <Typography
            variant="h2"
            component="h1"
            fontWeight="bold"
            gutterBottom
            sx={{ fontSize: { xs: '2.5rem', md: '3.75rem' } }}
          >
            ElectricityShop
          </Typography>
          <Typography
            variant="h5"
            paragraph
            sx={{ mb: 4 }}
          >
            Your one-stop destination for all electrical products and accessories
          </Typography>
          <Button
            variant="contained"
            size="large"
            component={RouterLink}
            to="/products"
            startIcon={<ShoppingCartIcon />}
            sx={{ px: 4, py: 1.5 }}
          >
            Shop Now
          </Button>
        </Box>
      </Paper>
      
      {/* Features Section */}
      <Box sx={{ my: 8 }}>
        <Typography
          variant="h4"
          component="h2"
          textAlign="center"
          gutterBottom
          fontWeight="bold"
        >
          Why Choose Us
        </Typography>
        <Typography
          variant="body1"
          color="text.secondary"
          textAlign="center"
          mb={6}
          mx="auto"
          sx={{ maxWidth: 700 }}
        >
          We provide the best products with exceptional service and value
        </Typography>
        
        <Grid container spacing={4}>
          <Grid item xs={12} sm={6} md={3}>
            <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column', alignItems: 'center', p: 2 }}>
              <ElectricBoltIcon color="primary" sx={{ fontSize: 50, mb: 2 }} />
              <CardContent sx={{ textAlign: 'center' }}>
                <Typography variant="h6" component="h3" gutterBottom>
                  Quality Products
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  We source only the highest quality electrical products from trusted manufacturers
                </Typography>
              </CardContent>
            </Card>
          </Grid>
          
          <Grid item xs={12} sm={6} md={3}>
            <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column', alignItems: 'center', p: 2 }}>
              <LocalShippingIcon color="primary" sx={{ fontSize: 50, mb: 2 }} />
              <CardContent sx={{ textAlign: 'center' }}>
                <Typography variant="h6" component="h3" gutterBottom>
                  Fast Delivery
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  Free shipping on all orders with quick and reliable delivery service
                </Typography>
              </CardContent>
            </Card>
          </Grid>
          
          <Grid item xs={12} sm={6} md={3}>
            <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column', alignItems: 'center', p: 2 }}>
              <CreditCardIcon color="primary" sx={{ fontSize: 50, mb: 2 }} />
              <CardContent sx={{ textAlign: 'center' }}>
                <Typography variant="h6" component="h3" gutterBottom>
                  Secure Payment
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  Multiple secure payment options for a hassle-free checkout experience
                </Typography>
              </CardContent>
            </Card>
          </Grid>
          
          <Grid item xs={12} sm={6} md={3}>
            <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column', alignItems: 'center', p: 2 }}>
              <BoltIcon color="primary" sx={{ fontSize: 50, mb: 2 }} />
              <CardContent sx={{ textAlign: 'center' }}>
                <Typography variant="h6" component="h3" gutterBottom>
                  Expert Support
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  Our team of experts is always ready to help you with any questions
                </Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      </Box>
      
      {/* Featured Products Section */}
      <Box sx={{ my: 8 }}>
        <Typography
          variant="h4"
          component="h2"
          textAlign="center"
          gutterBottom
          fontWeight="bold"
        >
          Featured Products
        </Typography>
        <Typography
          variant="body1"
          color="text.secondary"
          textAlign="center"
          mb={6}
          mx="auto"
          sx={{ maxWidth: 700 }}
        >
          Check out our top-rated electrical products
        </Typography>
        
        <Grid container spacing={4}>
          {isLoading
            ? Array.from(new Array(3)).map((_, index) => (
                <Grid item xs={12} sm={6} md={4} key={index}>
                  <ProductCard loading={true} product={{} as Product} />
                </Grid>
              ))
            : featuredProducts.map((product) => (
                <Grid item xs={12} sm={6} md={4} key={product.id}>
                  <ProductCard product={product} />
                </Grid>
              ))}
        </Grid>
        
        <Box sx={{ textAlign: 'center', mt: 4 }}>
          <Button
            variant="outlined"
            color="primary"
            component={RouterLink}
            to="/products"
            size="large"
          >
            View All Products
          </Button>
        </Box>
      </Box>
      
      {/* Call to Action Section */}
      <Paper
        sx={{
          bgcolor: 'primary.main',
          color: 'white',
          p: 6,
          borderRadius: 2,
          textAlign: 'center',
          my: 8,
        }}
      >
        <Typography variant="h4" component="h2" gutterBottom fontWeight="bold">
          Ready to Shop?
        </Typography>
        <Typography variant="body1" paragraph sx={{ maxWidth: 600, mx: 'auto', mb: 4 }}>
          Browse our extensive collection of electrical products and find exactly what you need.
        </Typography>
        <Button
          variant="contained"
          color="secondary"
          size="large"
          component={RouterLink}
          to="/products"
          sx={{ px: 4, py: 1.5 }}
        >
          Shop Now
        </Button>
      </Paper>
    </Container>
  );
};

export default HomePage;
