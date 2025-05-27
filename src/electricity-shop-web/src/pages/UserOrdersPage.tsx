import React, { useEffect, useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import {
  Container,
  Typography,
  Box,
  Grid,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  Button,
  Divider,
  Drawer,
  List,
  ListItem,
  ListItemText,
  IconButton,
  Alert,
} from '@mui/material';
import {
  Visibility as VisibilityIcon,
  NavigateNext as NavigateNextIcon,
} from '@mui/icons-material';

import { useOrder } from '../hooks/useOrder';
import { Order, OrderStatus } from '../types';
import { formatPrice, formatDate, formatOrderStatus } from '../utils/formatters';
import LoadingSpinner from '../components/common/LoadingSpinner';

const UserOrdersPage: React.FC = () => {
  const { orders, order, isLoading, error, fetchUserOrders, fetchOrderById } = useOrder();
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);
  const [orderDetailOpen, setOrderDetailOpen] = useState(false);
  
  // Fetch user orders
  useEffect(() => {
    fetchUserOrders();
  }, [fetchUserOrders]);
  
  // Handle view order details
  const handleViewOrder = async (orderId: string) => {
    await fetchOrderById(orderId);
    const foundOrder = orders.find(o => o.id === orderId);
    setSelectedOrder(foundOrder || null);
    setOrderDetailOpen(true);
  };
  
  // Get status chip color
  const getStatusColor = (status: OrderStatus) => {
    switch (status) {
      case OrderStatus.DELIVERED:
        return 'success';
      case OrderStatus.CANCELLED:
        return 'error';
      case OrderStatus.SHIPPED:
        return 'info';
      default:
        return 'warning';
    }
  };
  
  return (
    <Container maxWidth="lg">
      <LoadingSpinner isOpen={isLoading && !orders.length} message="Loading your orders..." />
      
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          My Orders
        </Typography>
        <Typography variant="body1" color="text.secondary">
          View and track your order history
        </Typography>
      </Box>
      
      {error && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}
      
      {!isLoading && orders.length === 0 ? (
        <Paper sx={{ p: 4, textAlign: 'center' }}>
          <Typography variant="h6" gutterBottom>
            You don't have any orders yet
          </Typography>
          <Typography variant="body1" color="text.secondary" paragraph>
            Once you place an order, it will appear here.
          </Typography>
          <Button
            variant="contained"
            color="primary"
            component={RouterLink}
            to="/products"
            sx={{ mt: 2 }}
          >
            Browse Products
          </Button>
        </Paper>
      ) : (
        <Paper sx={{ p: 2 }}>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Order ID</TableCell>
                  <TableCell>Date</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell align="right">Total</TableCell>
                  <TableCell align="right">Items</TableCell>
                  <TableCell align="center">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {orders.map((order) => (
                  <TableRow key={order.id} hover>
                    <TableCell>{order.id.substring(0, 8)}...</TableCell>
                    <TableCell>{formatDate(order.createdAt)}</TableCell>
                    <TableCell>
                      <Chip
                        label={formatOrderStatus(order.status)}
                        size="small"
                        color={getStatusColor(order.status) as any}
                      />
                    </TableCell>
                    <TableCell align="right">{formatPrice(order.totalPrice)}</TableCell>
                    <TableCell align="right">{order.items.length}</TableCell>
                    <TableCell align="center">
                      <IconButton
                        size="small"
                        onClick={() => handleViewOrder(order.id)}
                      >
                        <VisibilityIcon fontSize="small" />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>
      )}
      
      {/* Order Detail Drawer */}
      <Drawer
        anchor="right"
        open={orderDetailOpen}
        onClose={() => setOrderDetailOpen(false)}
        PaperProps={{ sx: { width: '40%', minWidth: 300, p: 3 } }}
      >
        {selectedOrder && (
          <>
            <Typography variant="h6" gutterBottom>
              Order Details
            </Typography>
            
            <Box sx={{ mb: 3 }}>
              <Typography variant="subtitle1">Order #{selectedOrder.id}</Typography>
              <Typography variant="body2" color="text.secondary">
                Placed on {formatDate(selectedOrder.createdAt)}
              </Typography>
              <Chip
                label={formatOrderStatus(selectedOrder.status)}
                size="small"
                color={getStatusColor(selectedOrder.status) as any}
                sx={{ mt: 1 }}
              />
            </Box>
            
            <Typography variant="subtitle1" gutterBottom>
              Items
            </Typography>
            <List>
              {selectedOrder.items.map((item) => (
                <ListItem key={item.id} divider>
                  <ListItemText
                    primary={item.productName}
                    secondary={`Quantity: ${item.quantity}`}
                  />
                  <Typography variant="body2">
                    {formatPrice(item.price * item.quantity)}
                  </Typography>
                </ListItem>
              ))}
            </List>
            
            <Box sx={{ mt: 3 }}>
              <Typography variant="subtitle1">Order Summary</Typography>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 1 }}>
                <Typography variant="body2">Subtotal:</Typography>
                <Typography variant="body2">{formatPrice(selectedOrder.totalPrice - selectedOrder.tax - selectedOrder.shippingCost)}</Typography>
              </Box>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 1 }}>
                <Typography variant="body2">Shipping:</Typography>
                <Typography variant="body2">{formatPrice(selectedOrder.shippingCost)}</Typography>
              </Box>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 1 }}>
                <Typography variant="body2">Tax:</Typography>
                <Typography variant="body2">{formatPrice(selectedOrder.tax)}</Typography>
              </Box>
              <Divider sx={{ my: 1 }} />
              <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 1 }}>
                <Typography variant="subtitle2">Total:</Typography>
                <Typography variant="subtitle2" fontWeight="bold">
                  {formatPrice(selectedOrder.totalPrice)}
                </Typography>
              </Box>
            </Box>
            
            <Box sx={{ mt: 3 }}>
              <Typography variant="subtitle1" gutterBottom>
                Shipping Address
              </Typography>
              <Paper variant="outlined" sx={{ p: 2 }}>
                <Typography variant="body2">
                  {selectedOrder.shippingAddress.street}
                  <br />
                  {selectedOrder.shippingAddress.city}, {selectedOrder.shippingAddress.state} {selectedOrder.shippingAddress.zipCode}
                  <br />
                  {selectedOrder.shippingAddress.country}
                </Typography>
              </Paper>
            </Box>
            
            <Box sx={{ mt: 3 }}>
              <Typography variant="subtitle1" gutterBottom>
                Payment Information
              </Typography>
              <Paper variant="outlined" sx={{ p: 2 }}>
                <Typography variant="body2">
                  Status: {selectedOrder.payment.status}
                  <br />
                  Method: {selectedOrder.payment.method}
                  <br />
                  Transaction ID: {selectedOrder.payment.transactionId}
                </Typography>
              </Paper>
            </Box>
            
            <Box sx={{ mt: 3 }}>
              <Button
                variant="outlined"
                onClick={() => setOrderDetailOpen(false)}
                fullWidth
              >
                Close
              </Button>
            </Box>
          </>
        )}
      </Drawer>
    </Container>
  );
};

export default UserOrdersPage;
