import React, { useEffect } from 'react';
import { 
  Box, 
  Container, 
  Typography, 
  Grid, 
  Button, 
  Paper,
  useTheme
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../hooks/useAppDispatch';
import { fetchProducts } from '../store/slices/productSlice';
import ProductCard from '../components/features/products/ProductCard';
import LoadingScreen from '../components/common/LoadingScreen';
import ErrorDisplay from '../components/common/ErrorDisplay';

const HomePage: React.FC = () => {
  const theme = useTheme();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  
  const { products, isLoading, error } = useAppSelector(state => state.products);

  useEffect(() => {
    dispatch(fetchProducts({ limit: 8 }));
  }, [dispatch]);

  const handleBrowseAll = () => {
    navigate('/products');
  };

  if (isLoading && products.length === 0) {
    return <LoadingScreen />;
  }

  const renderHeroSection = () => (
    <Paper
      sx={{
        position: 'relative',
        backgroundColor: 'grey.800',
        color: '#fff',
        mb: 4,
        backgroundSize: 'cover',
        backgroundRepeat: 'no-repeat',
        backgroundPosition: 'center',
        backgroundImage: `url(https://source.unsplash.com/random?electronics)`,
        height: 400,
      }}
    >
      {/* Increase the priority of the hero background image */}
      {<img style={{ display: 'none' }} src="https://source.unsplash.com/random?electronics" alt="hero" />}
      <Box
        sx={{
          position: 'absolute',
          top: 0,
          bottom: 0,
          right: 0,
          left: 0,
          backgroundColor: 'rgba(0,0,0,.3)',
          display: 'flex',
          alignItems: 'center',
        }}
      >
        <Container maxWidth="md">
          <Box sx={{ maxWidth: 600 }}>
            <Typography component="h1" variant="h2" color="inherit" gutterBottom>
              ElectricityShop
            </Typography>
            <Typography variant="h5" color="inherit" paragraph>
              Your one-stop shop for all electrical products. Browse our wide selection of high-quality electrical products at competitive prices.
            </Typography>
            <Button variant="contained" size="large" onClick={handleBrowseAll}>
              Shop Now
            </Button>
          </Box>
        </Container>
      </Box>
    </Paper>
  );

  return (
    <Box>
      {renderHeroSection()}
      
      <Container>
        {error ? (
          <ErrorDisplay 
            message={error} 
            onRetry={() => dispatch(fetchProducts({ limit: 8 }))} 
          />
        ) : (
          <>
            <Box sx={{ mb: 4 }}>
              <Typography variant="h4" component="h2" gutterBottom>
                Featured Products
              </Typography>
              <Typography variant="body1" color="text.secondary" paragraph>
                Discover our most popular electrical products.
              </Typography>
            </Box>
            
            <Grid container spacing={3}>
              {products.map((product) => (
                <Grid item key={product.id} xs={12} sm={6} md={3}>
                  <ProductCard product={product} />
                </Grid>
              ))}
            </Grid>
            
            <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
              <Button 
                variant="outlined" 
                color="primary" 
                size="large"
                onClick={handleBrowseAll}
              >
                Browse All Products
              </Button>
            </Box>
          </>
        )}
        
        {/* Categories Section */}
        <Box sx={{ my: 8 }}>
          <Typography variant="h4" component="h2" gutterBottom>
            Product Categories
          </Typography>
          <Typography variant="body1" color="text.secondary" paragraph>
            Browse our products by category to find exactly what you need.
          </Typography>
          
          <Grid container spacing={3} sx={{ mt: 2 }}>
            {['Lighting', 'Switches', 'Cables', 'Tools'].map((category) => (
              <Grid item key={category} xs={12} sm={6} md={3}>
                <Paper
                  sx={{
                    p: 3,
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    textAlign: 'center',
                    height: '100%',
                    cursor: 'pointer',
                    transition: 'transform 0.3s, box-shadow 0.3s',
                    '&:hover': {
                      transform: 'translateY(-4px)',
                      boxShadow: 4,
                    },
                  }}
                  onClick={() => navigate(`/products?category=${category}`)}
                >
                  <Typography variant="h6" gutterBottom>
                    {category}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    Browse our {category.toLowerCase()} collection
                  </Typography>
                </Paper>
              </Grid>
            ))}
          </Grid>
        </Box>
        
        {/* Why Choose Us Section */}
        <Box sx={{ my: 8 }}>
          <Typography variant="h4" component="h2" gutterBottom>
            Why Choose ElectricityShop
          </Typography>
          <Typography variant="body1" color="text.secondary" paragraph>
            We are committed to providing the best service and products to our customers.
          </Typography>
          
          <Grid container spacing={3} sx={{ mt: 2 }}>
            {[
              { title: 'Quality Products', description: 'All our products are sourced from trusted manufacturers and undergo rigorous quality checks.' },
              { title: 'Fast Shipping', description: 'We offer quick and reliable shipping options to get your products to you when you need them.' },
              { title: 'Expert Support', description: 'Our team of experts is always ready to help you choose the right products for your needs.' },
              { title: 'Easy Returns', description: 'Not satisfied with your purchase? Our hassle-free return policy has got you covered.' }
            ].map((item, index) => (
              <Grid item key={index} xs={12} sm={6} md={3}>
                <Paper sx={{ p: 3, height: '100%' }}>
                  <Typography variant="h6" gutterBottom>
                    {item.title}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    {item.description}
                  </Typography>
                </Paper>
              </Grid>
            ))}
          </Grid>
        </Box>
      </Container>
    </Box>
  );
};

export default HomePage;
