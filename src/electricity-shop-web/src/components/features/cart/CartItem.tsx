import React from 'react';
import { Link as RouterLink } from 'react-router-dom';
import {
  Box,
  Card,
  CardMedia,
  Typography,
  IconButton,
  TextField,
  Link,
  Grid,
  useTheme,
  useMediaQuery,
} from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import AddIcon from '@mui/icons-material/Add';
import RemoveIcon from '@mui/icons-material/Remove';

import { CartItem as CartItemType } from '../../../types';
import { formatPrice } from '../../../utils/formatters';

interface CartItemProps {
  item: CartItemType;
  onUpdateQuantity: (id: string, quantity: number) => void;
  onRemove: (id: string) => void;
}

const CartItem: React.FC<CartItemProps> = ({ item, onUpdateQuantity, onRemove }) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  
  const handleQuantityChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseInt(event.target.value);
    if (!isNaN(value) && value >= 1 && value <= item.product.stock) {
      onUpdateQuantity(item.id, value);
    }
  };
  
  const increaseQuantity = () => {
    if (item.quantity < item.product.stock) {
      onUpdateQuantity(item.id, item.quantity + 1);
    }
  };
  
  const decreaseQuantity = () => {
    if (item.quantity > 1) {
      onUpdateQuantity(item.id, item.quantity - 1);
    }
  };
  
  return (
    <Card 
      sx={{ 
        p: 2, 
        mb: 2, 
        display: 'flex', 
        flexDirection: isMobile ? 'column' : 'row',
        alignItems: isMobile ? 'center' : 'flex-start',
      }}
    >
      <CardMedia
        component="img"
        sx={{ 
          width: isMobile ? '100%' : 100, 
          height: isMobile ? 150 : 100,
          objectFit: 'contain',
          mr: isMobile ? 0 : 2,
          mb: isMobile ? 2 : 0,
          backgroundColor: 'grey.100',
        }}
        image={item.product.imageUrl}
        alt={item.product.name}
      />
      
      <Grid container spacing={2} alignItems="center">
        <Grid item xs={12} sm={4}>
          <Box>
            <Link
              component={RouterLink}
              to={`/products/${item.product.id}`}
              color="inherit"
              underline="hover"
            >
              <Typography variant="h6" component="div">
                {item.product.name}
              </Typography>
            </Link>
            <Typography variant="body2" color="text.secondary" gutterBottom>
              Category: {item.product.category}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Price: {formatPrice(item.price)}
            </Typography>
          </Box>
        </Grid>
        
        <Grid item xs={12} sm={4}>
          <Box 
            sx={{ 
              display: 'flex', 
              alignItems: 'center',
              justifyContent: isMobile ? 'center' : 'flex-start',
            }}
          >
            <IconButton 
              size="small" 
              onClick={decreaseQuantity}
              disabled={item.quantity <= 1}
            >
              <RemoveIcon />
            </IconButton>
            <TextField
              size="small"
              inputProps={{ 
                min: 1, 
                max: item.product.stock,
                style: { textAlign: 'center' } 
              }}
              sx={{ width: 60, mx: 1 }}
              value={item.quantity}
              onChange={handleQuantityChange}
              type="number"
            />
            <IconButton 
              size="small" 
              onClick={increaseQuantity}
              disabled={item.quantity >= item.product.stock}
            >
              <AddIcon />
            </IconButton>
          </Box>
          <Typography 
            variant="body2" 
            color="text.secondary"
            align={isMobile ? "center" : "left"}
            sx={{ mt: 1 }}
          >
            {item.product.stock <= 5 ? (
              <Box component="span" color="error.main">
                Only {item.product.stock} left in stock
              </Box>
            ) : (
              `${item.product.stock} in stock`
            )}
          </Typography>
        </Grid>
        
        <Grid item xs={12} sm={4}>
          <Box 
            sx={{ 
              display: 'flex', 
              flexDirection: 'column', 
              alignItems: isMobile ? 'center' : 'flex-end',
              height: '100%',
              justifyContent: 'space-between',
            }}
          >
            <Typography variant="h6" color="primary">
              {formatPrice(item.price * item.quantity)}
            </Typography>
            <IconButton 
              color="error" 
              onClick={() => onRemove(item.id)}
              sx={{ mt: isMobile ? 1 : 0 }}
            >
              <DeleteIcon />
            </IconButton>
          </Box>
        </Grid>
      </Grid>
    </Card>
  );
};

export default CartItem;
