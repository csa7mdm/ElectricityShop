import React from 'react';
import { 
  Card, 
  CardContent, 
  CardMedia, 
  Typography, 
  Button, 
  CardActions, 
  Box, 
  Chip,
  Rating
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../../hooks/useAppDispatch';
import { addToCart } from '../../../store/slices/cartSlice';
import { Product } from '../../../store/slices/productSlice';
import { showNotification } from '../../../store/slices/uiSlice';

interface ProductCardProps {
  product: Product;
}

const ProductCard: React.FC<ProductCardProps> = ({ product }) => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { token } = useAppSelector((state) => state.auth);

  const handleAddToCart = async (e: React.MouseEvent) => {
    e.stopPropagation();
    
    if (!token) {
      navigate('/login', { state: { from: `/products/${product.id}` } });
      return;
    }
    
    try {
      await dispatch(addToCart({ productId: product.id, quantity: 1 })).unwrap();
      dispatch(showNotification({ 
        message: `${product.name} added to cart`, 
        type: 'success' 
      }));
    } catch (error) {
      dispatch(showNotification({ 
        message: `Failed to add to cart: ${error}`, 
        type: 'error' 
      }));
    }
  };

  const handleViewDetails = () => {
    navigate(`/products/${product.id}`);
  };

  // Get main image or use placeholder
  const imageUrl = product.images?.find(img => img.isMain)?.url || 
    'https://via.placeholder.com/300x200?text=No+Image';

  // Check if product is in stock
  const isInStock = product.stockQuantity > 0;

  return (
    <Card 
      sx={{ 
        height: '100%', 
        display: 'flex', 
        flexDirection: 'column',
        transition: 'transform 0.3s, box-shadow 0.3s',
        '&:hover': {
          transform: 'translateY(-4px)',
          boxShadow: 4,
        },
        cursor: 'pointer'
      }}
      onClick={handleViewDetails}
    >
      <CardMedia
        component="img"
        height="200"
        image={imageUrl}
        alt={product.name}
      />
      <CardContent sx={{ flexGrow: 1 }}>
        <Typography gutterBottom variant="h6" component="h2" noWrap>
          {product.name}
        </Typography>
        <Typography 
          variant="body2" 
          color="text.secondary" 
          sx={{ 
            overflow: 'hidden',
            textOverflow: 'ellipsis',
            display: '-webkit-box',
            WebkitLineClamp: 2,
            WebkitBoxOrient: 'vertical',
            mb: 1
          }}
        >
          {product.description}
        </Typography>
        
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
          <Typography variant="h6" color="primary">
            ${product.price.toFixed(2)}
          </Typography>
          <Chip 
            label={isInStock ? 'In Stock' : 'Out of Stock'} 
            color={isInStock ? 'success' : 'error'} 
            size="small" 
          />
        </Box>
        
        {product.category && (
          <Chip 
            label={product.category.name} 
            size="small" 
            sx={{ mb: 1 }} 
          />
        )}
      </CardContent>
      <CardActions>
        <Button 
          size="small" 
          color="primary" 
          onClick={handleViewDetails}
        >
          View Details
        </Button>
        <Button 
          size="small" 
          color="secondary"
          onClick={handleAddToCart}
          disabled={!isInStock}
        >
          Add to Cart
        </Button>
      </CardActions>
    </Card>
  );
};

export default ProductCard;
