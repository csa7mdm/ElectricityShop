import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, Link as RouterLink } from 'react-router-dom';
import {
  Container,
  Grid,
  Typography,
  Box,
  Button,
  Divider,
  Breadcrumbs,
  Link as MuiLink,
  Paper,
  Rating,
  Chip,
  TextField,
  Alert,
  Skeleton,
  Tab,
  Tabs,
  List,
  ListItem,
  ListItemText,
  Snackbar,
} from '@mui/material';
import {
  ShoppingCart as ShoppingCartIcon,
  Add as AddIcon,
  Remove as RemoveIcon,
  NavigateNext as NavigateNextIcon,
} from '@mui/icons-material';

import { useProduct } from '../hooks/useProduct';
import { useCart } from '../hooks/useCart';
import { formatPrice, formatDate } from '../utils/formatters';
import LoadingSpinner from '../components/common/LoadingSpinner';

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
      id={`product-tabpanel-${index}`}
      aria-labelledby={`product-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  );
};

const ProductDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { product, isLoading, error, fetchProductById } = useProduct();
  const { addToCart, isLoading: isCartLoading } = useCart();
  
  const [quantity, setQuantity] = useState(1);
  const [tabValue, setTabValue] = useState(0);
  const [notification, setNotification] = useState({
    open: false,
    message: '',
  });
  
  useEffect(() => {
    if (id) {
      fetchProductById(id);
    }
  }, [fetchProductById, id]);
  
  useEffect(() => {
    // Reset quantity when product changes
    setQuantity(1);
  }, [product]);
  
  const handleQuantityChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseInt(event.target.value);
    if (!isNaN(value) && value >= 1 && value <= (product?.stock || 1)) {
      setQuantity(value);
    }
  };
  
  const increaseQuantity = () => {
    if (product && quantity < product.stock) {
      setQuantity((prev) => prev + 1);
    }
  };
  
  const decreaseQuantity = () => {
    if (quantity > 1) {
      setQuantity((prev) => prev - 1);
    }
  };
  
  const handleAddToCart = async () => {
    if (product) {
      const success = await addToCart(product.id, quantity);
      if (success) {
        setNotification({
          open: true,
          message: `${product.name} added to cart`,
        });
      }
    }
  };
  
  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };
  
  const handleCloseNotification = () => {
    setNotification({
      ...notification,
      open: false,
    });
  };
  
  if (isLoading || !product) {
    return (
      <Container maxWidth="lg">
        <LoadingSpinner isOpen={isLoading} message="Loading product details..." />
        
        <Box mb={4}>
          <Skeleton variant="text" width={300} height={30} />
        </Box>
        
        <Grid container spacing={4}>
          <Grid item xs={12} md={6}>
            <Skeleton variant="rectangular" height={400} />
          </Grid>
          <Grid item xs={12} md={6}>
            <Skeleton variant="text" height={60} />
            <Skeleton variant="text" width="40%" />
            <Skeleton variant="text" height={30} width="30%" sx={{ my: 2 }} />
            <Skeleton variant="text" height={120} sx={{ mb: 2 }} />
            <Skeleton variant="rectangular" height={60} width="80%" sx={{ mb: 2 }} />
            <Skeleton variant="rectangular" height={50} width="100%" />
          </Grid>
        </Grid>
      </Container>
    );
  }
  
  if (error) {
    return (
      <Container maxWidth="lg">
        <Alert severity="error" sx={{ my: 4 }}>
          {error}
        </Alert>
        <Button
          variant="contained"
          onClick={() => navigate('/products')}
          sx={{ mt: 2 }}
        >
          Back to Products
        </Button>
      </Container>
    );
  }
  
  return (
    <Container maxWidth="lg">
      {/* Breadcrumbs */}
      <Box mb={4}>
        <Breadcrumbs separator={<NavigateNextIcon fontSize="small" />}>
          <MuiLink component={RouterLink} to="/" color="inherit">
            Home
          </MuiLink>
          <MuiLink component={RouterLink} to="/products" color="inherit">
            Products
          </MuiLink>
          {product.category && (
            <MuiLink
              component={RouterLink}
              to={`/products?category=${product.category}`}
              color="inherit"
            >
              {product.category}
            </MuiLink>
          )}
          <Typography color="text.primary">{product.name}</Typography>
        </Breadcrumbs>
      </Box>
      
      <Grid container spacing={4}>
        {/* Product Image */}
        <Grid item xs={12} md={6}>
          <Paper
            sx={{
              p: 2,
              height: 400,
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              backgroundColor: 'grey.100',
            }}
          >
            <img
              src={product.imageUrl}
              alt={product.name}
              style={{ maxWidth: '100%', maxHeight: '100%', objectFit: 'contain' }}
            />
          </Paper>
        </Grid>
        
        {/* Product Details */}
        <Grid item xs={12} md={6}>
          <Typography variant="h4" component="h1" gutterBottom>
            {product.name}
          </Typography>
          
          <Box display="flex" alignItems="center" mb={2}>
            <Rating value={product.rating} precision={0.5} readOnly />
            <Typography variant="body2" sx={{ ml: 1 }}>
              ({product.reviews?.length || 0} reviews)
            </Typography>
          </Box>
          
          <Typography variant="h4" color="primary" fontWeight="bold" mb={2}>
            {formatPrice(product.price)}
          </Typography>
          
          <Typography variant="body1" mb={3}>
            {product.description}
          </Typography>
          
          <Box mb={3}>
            <Grid container spacing={2}>
              <Grid item xs={6}>
                <Typography variant="body2" color="text.secondary">
                  Category
                </Typography>
                <Typography variant="body1">{product.category}</Typography>
              </Grid>
              
              <Grid item xs={6}>
                <Typography variant="body2" color="text.secondary">
                  Availability
                </Typography>
                <Typography variant="body1">
                  {product.stock > 0 ? (
                    product.stock > 5 ? (
                      <Chip label="In Stock" color="success" size="small" />
                    ) : (
                      <Chip label={`Only ${product.stock} left`} color="warning" size="small" />
                    )
                  ) : (
                    <Chip label="Out of Stock" color="error" size="small" />
                  )}
                </Typography>
              </Grid>
            </Grid>
          </Box>
          
          <Divider sx={{ my: 3 }} />
          
          {/* Quantity Selector */}
          <Box mb={3}>
            <Typography variant="body1" gutterBottom>
              Quantity
            </Typography>
            <Box display="flex" alignItems="center">
              <Button
                variant="outlined"
                size="small"
                onClick={decreaseQuantity}
                disabled={quantity <= 1 || product.stock === 0}
              >
                <RemoveIcon fontSize="small" />
              </Button>
              <TextField
                size="small"
                inputProps={{ 
                  min: 1, 
                  max: product.stock,
                  style: { textAlign: 'center' } 
                }}
                sx={{ width: 60, mx: 1 }}
                value={quantity}
                onChange={handleQuantityChange}
                type="number"
                disabled={product.stock === 0}
              />
              <Button
                variant="outlined"
                size="small"
                onClick={increaseQuantity}
                disabled={quantity >= product.stock || product.stock === 0}
              >
                <AddIcon fontSize="small" />
              </Button>
              <Typography variant="body2" color="text.secondary" sx={{ ml: 2 }}>
                {product.stock} items available
              </Typography>
            </Box>
          </Box>
          
          {/* Add to Cart Button */}
          <Button
            variant="contained"
            color="primary"
            size="large"
            fullWidth
            startIcon={<ShoppingCartIcon />}
            onClick={handleAddToCart}
            disabled={isCartLoading || product.stock === 0}
          >
            {product.stock === 0 ? 'Out of Stock' : 'Add to Cart'}
          </Button>
        </Grid>
      </Grid>
      
      {/* Product Tabs (Details, Specifications, Reviews) */}
      <Box sx={{ mt: 6 }}>
        <Paper>
          <Tabs 
            value={tabValue} 
            onChange={handleTabChange} 
            aria-label="product tabs"
            centered
          >
            <Tab label="Description" id="product-tab-0" />
            <Tab label="Specifications" id="product-tab-1" />
            <Tab label="Reviews" id="product-tab-2" />
          </Tabs>
          
          <TabPanel value={tabValue} index={0}>
            <Typography variant="body1" paragraph>
              {product.description}
            </Typography>
            <Typography variant="body1">
              Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla eget felis euismod, 
              tincidunt nibh quis, finibus dui. Mauris placerat purus nec lectus porttitor, 
              eget luctus ipsum facilisis. Praesent ultrices neque sed eros ultrices, ut gravida 
              nisl efficitur. Integer tincidunt sem ac massa finibus, eget lobortis massa lobortis.
            </Typography>
          </TabPanel>
          
          <TabPanel value={tabValue} index={1}>
            <List>
              <ListItem divider>
                <ListItemText primary="Brand" secondary={product.name.split(' ')[0]} />
              </ListItem>
              <ListItem divider>
                <ListItemText primary="Model" secondary={`${product.name.split(' ')[0]}-${product.id.substring(0, 5)}`} />
              </ListItem>
              <ListItem divider>
                <ListItemText primary="Category" secondary={product.category} />
              </ListItem>
              <ListItem divider>
                <ListItemText primary="Weight" secondary="1.5 kg" />
              </ListItem>
              <ListItem divider>
                <ListItemText primary="Dimensions" secondary="25 x 15 x 5 cm" />
              </ListItem>
              <ListItem>
                <ListItemText primary="Warranty" secondary="12 months" />
              </ListItem>
            </List>
          </TabPanel>
          
          <TabPanel value={tabValue} index={2}>
            {product.reviews && product.reviews.length > 0 ? (
              product.reviews.map((review) => (
                <Paper key={review.id} sx={{ mb: 2, p: 2 }}>
                  <Box display="flex" justifyContent="space-between">
                    <Typography variant="body1" fontWeight="bold">
                      {review.userName}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      {formatDate(review.createdAt)}
                    </Typography>
                  </Box>
                  <Rating value={review.rating} size="small" readOnly sx={{ my: 1 }} />
                  <Typography variant="body1">
                    {review.comment}
                  </Typography>
                </Paper>
              ))
            ) : (
              <Box textAlign="center" py={4}>
                <Typography variant="h6" color="text.secondary" gutterBottom>
                  No reviews yet
                </Typography>
                <Typography variant="body1" color="text.secondary">
                  Be the first to review this product
                </Typography>
              </Box>
            )}
          </TabPanel>
        </Paper>
      </Box>
      
      {/* Related Products Section (could be implemented here) */}
      
      {/* Success notification */}
      <Snackbar
        open={notification.open}
        autoHideDuration={3000}
        onClose={handleCloseNotification}
        message={notification.message}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
      />
    </Container>
  );
};

export default ProductDetailPage;
