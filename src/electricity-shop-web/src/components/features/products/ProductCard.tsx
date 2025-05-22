import React from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Card,
  CardActionArea,
  CardActions,
  CardContent,
  CardMedia,
  Button,
  Typography,
  Rating,
  Box,
  Chip,
  Skeleton,
} from '@mui/material';
import { AddShoppingCart as AddShoppingCartIcon } from '@mui/icons-material';

import { Product } from '../../../types';
import { useCart } from '../../../hooks/useCart';
import { formatPrice, truncateText } from '../../../utils/formatters';

interface ProductCardProps {
  product: Product;
  loading?: boolean;
}

const ProductCard: React.FC<ProductCardProps> = ({ product, loading = false }) => {
  const navigate = useNavigate();
  const { addToCart } = useCart();

  const handleAddToCart = async (e: React.MouseEvent) => {
    e.stopPropagation();
    await addToCart(product.id, 1);
  };

  const handleProductClick = () => {
    navigate(`/products/${product.id}`);
  };

  if (loading) {
    return (
      <Card sx={{ maxWidth: 345, height: '100%', display: 'flex', flexDirection: 'column' }}>
        <Skeleton variant="rectangular" height={140} />
        <CardContent>
          <Skeleton variant="text" height={32} width="80%" />
          <Skeleton variant="text" height={20} width="40%" />
          <Skeleton variant="text" height={60} />
          <Box sx={{ display: 'flex', alignItems: 'center', mt: 1 }}>
            <Skeleton variant="text" width={120} height={24} />
          </Box>
        </CardContent>
        <Box sx={{ flexGrow: 1 }} />
        <CardActions>
          <Skeleton variant="rectangular" width={100} height={36} />
          <Box sx={{ flexGrow: 1 }} />
          <Skeleton variant="circular" width={36} height={36} />
        </CardActions>
      </Card>
    );
  }

  return (
    <Card 
      sx={{ 
        maxWidth: 345, 
        height: '100%', 
        display: 'flex', 
        flexDirection: 'column',
        transition: 'transform 0.3s ease, box-shadow 0.3s ease',
        '&:hover': {
          transform: 'translateY(-5px)',
          boxShadow: (theme) => theme.shadows[8],
        }
      }}
    >
      <CardActionArea onClick={handleProductClick}>
        <CardMedia
          component="img"
          height="140"
          image={product.imageUrl || product.placeholderImageUrl}
          alt={product.name}
          sx={{ objectFit: 'contain', backgroundColor: 'grey.100', p: 1 }}
        />
        <CardContent>
          <Typography gutterBottom variant="h6" component="div" sx={{ minHeight: 64 }}>
            {truncateText(product.name, 50)}
          </Typography>
          <Box display="flex" alignItems="center" mb={1}>
            <Typography variant="h6" color="primary" sx={{ fontWeight: 'bold', mr: 1 }}>
              {formatPrice(product.price)}
            </Typography>
            {product.stock <= 5 && (
              <Chip 
                label={product.stock === 0 ? "Out of Stock" : "Low Stock"} 
                color={product.stock === 0 ? "error" : "warning"} 
                size="small"
              />
            )}
          </Box>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2, minHeight: 60 }}>
            {truncateText(product.description, 100)}
          </Typography>
          <Box display="flex" alignItems="center">
            <Rating value={product.rating} precision={0.5} readOnly size="small" />
            <Typography variant="body2" color="text.secondary" sx={{ ml: 1 }}>
              ({product.reviews?.length || 0} reviews)
            </Typography>
          </Box>
        </CardContent>
      </CardActionArea>
      <Box sx={{ flexGrow: 1 }} />
      <CardActions sx={{ justifyContent: 'space-between', p: 2 }}>
        <Button 
          size="small" 
          color="primary" 
          onClick={handleProductClick}
        >
          View Details
        </Button>
        <Button
          variant="contained"
          color="primary"
          startIcon={<AddShoppingCartIcon />}
          onClick={handleAddToCart}
          disabled={product.stock === 0}
          size="small"
        >
          Add to Cart
        </Button>
      </CardActions>
    </Card>
  );
};

export default ProductCard;
