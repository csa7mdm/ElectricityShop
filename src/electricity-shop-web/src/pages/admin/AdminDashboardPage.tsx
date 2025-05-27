import React, { useEffect, useState } from 'react';
import {
  Container,
  Grid,
  Paper,
  Typography,
  Box,
  Card,
  CardContent,
  Divider,
  List,
  ListItem,
  ListItemText,
  ListItemAvatar,
  Avatar,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from '@mui/material';
import {
  ShoppingBag as ShoppingBagIcon,
  People as PeopleIcon,
  AttachMoney as AttachMoneyIcon,
  Inventory as InventoryIcon,
  TrendingUp as TrendingUpIcon,
  ShoppingCart as ShoppingCartIcon,
} from '@mui/icons-material';
import { Link as RouterLink } from 'react-router-dom';

import { useAuth } from '../../hooks/useAuth';
import { useProduct } from '../../hooks/useProduct';
import { useOrder } from '../../hooks/useOrder';
import { formatPrice, formatDate } from '../../utils/formatters';
import LoadingSpinner from '../../components/common/LoadingSpinner';

// Sample data for the dashboard
const getRandomValue = (min: number, max: number) => {
  return Math.floor(Math.random() * (max - min + 1) + min);
};

const salesData = [
  { month: 'Jan', value: getRandomValue(5000, 15000) },
  { month: 'Feb', value: getRandomValue(5000, 15000) },
  { month: 'Mar', value: getRandomValue(5000, 15000) },
  { month: 'Apr', value: getRandomValue(5000, 15000) },
  { month: 'May', value: getRandomValue(5000, 15000) },
  { month: 'Jun', value: getRandomValue(5000, 15000) },
];

const AdminDashboardPage: React.FC = () => {
  const { user } = useAuth();
  const { products, fetchProducts, isLoading: productsLoading } = useProduct();
  const { orders, fetchAllOrders, isLoading: ordersLoading } = useOrder();
  
  const [statistics, setStatistics] = useState({
    totalSales: 0,
    totalOrders: 0,
    totalProducts: 0,
    totalCustomers: 100, // Placeholder value
    lowStockProducts: 0,
  });
  
  // Fetch products and orders
  useEffect(() => {
    fetchProducts({ page: 1, limit: 1000 });
    fetchAllOrders({ page: 1, limit: 10 });
  }, [fetchProducts, fetchAllOrders]);
  
  // Calculate statistics
  useEffect(() => {
    if (products && orders) {
      const totalSales = orders.reduce((sum, order) => sum + order.totalPrice, 0);
      const lowStockProducts = products.filter((product) => product.stock <= 5).length;
      
      setStatistics({
        totalSales,
        totalOrders: orders.length,
        totalProducts: products.length,
        totalCustomers: 100, // Placeholder value
        lowStockProducts,
      });
    }
  }, [products, orders]);
  
  const isLoading = productsLoading || ordersLoading;
  
  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <LoadingSpinner isOpen={isLoading && !products.length} message="Loading dashboard data..." />
      
      <Typography variant="h4" component="h1" gutterBottom>
        Admin Dashboard
      </Typography>
      <Typography variant="subtitle1" color="text.secondary" paragraph>
        Welcome back, {user?.firstName} {user?.lastName}. Here's what's happening with your store today.
      </Typography>
      
      {/* Stats Cards */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12} sm={6} lg={3}>
          <Paper
            elevation={2}
            sx={{
              p: 3,
              display: 'flex',
              flexDirection: 'column',
              borderRadius: 2,
              height: '100%',
              bgcolor: 'primary.light',
              color: 'primary.contrastText',
            }}
          >
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
              <AttachMoneyIcon sx={{ mr: 1 }} />
              <Typography variant="h6">Total Sales</Typography>
            </Box>
            <Typography variant="h4" component="div" sx={{ mb: 1, fontWeight: 'bold' }}>
              {formatPrice(statistics.totalSales)}
            </Typography>
            <Typography variant="body2">Last 30 days</Typography>
          </Paper>
        </Grid>
        
        <Grid item xs={12} sm={6} lg={3}>
          <Paper
            elevation={2}
            sx={{
              p: 3,
              display: 'flex',
              flexDirection: 'column',
              borderRadius: 2,
              height: '100%',
            }}
          >
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
              <ShoppingBagIcon sx={{ mr: 1 }} />
              <Typography variant="h6">Orders</Typography>
            </Box>
            <Typography variant="h4" component="div" sx={{ mb: 1, fontWeight: 'bold' }}>
              {statistics.totalOrders}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Last 30 days
            </Typography>
          </Paper>
        </Grid>
        
        <Grid item xs={12} sm={6} lg={3}>
          <Paper
            elevation={2}
            sx={{
              p: 3,
              display: 'flex',
              flexDirection: 'column',
              borderRadius: 2,
              height: '100%',
            }}
          >
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
              <PeopleIcon sx={{ mr: 1 }} />
              <Typography variant="h6">Customers</Typography>
            </Box>
            <Typography variant="h4" component="div" sx={{ mb: 1, fontWeight: 'bold' }}>
              {statistics.totalCustomers}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Total registered users
            </Typography>
          </Paper>
        </Grid>
        
        <Grid item xs={12} sm={6} lg={3}>
          <Paper
            elevation={2}
            sx={{
              p: 3,
              display: 'flex',
              flexDirection: 'column',
              borderRadius: 2,
              height: '100%',
              bgcolor: statistics.lowStockProducts > 0 ? 'error.light' : 'success.light',
              color: statistics.lowStockProducts > 0 ? 'error.contrastText' : 'success.contrastText',
            }}
          >
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
              <InventoryIcon sx={{ mr: 1 }} />
              <Typography variant="h6">Inventory Alert</Typography>
            </Box>
            <Typography variant="h4" component="div" sx={{ mb: 1, fontWeight: 'bold' }}>
              {statistics.lowStockProducts}
            </Typography>
            <Typography variant="body2">
              Products with low stock
            </Typography>
          </Paper>
        </Grid>
      </Grid>
      
      {/* Recent Orders & Sales Chart */}
      <Grid container spacing={3}>
        <Grid item xs={12} lg={8}>
          <Paper elevation={2} sx={{ p: 3, borderRadius: 2 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
              <Typography variant="h6" component="h2">
                Recent Orders
              </Typography>
              <Button
                component={RouterLink}
                to="/admin/orders"
                size="small"
              >
                View All
              </Button>
            </Box>
            <TableContainer>
              <Table size="small">
                <TableHead>
                  <TableRow>
                    <TableCell>Order ID</TableCell>
                    <TableCell>Customer</TableCell>
                    <TableCell>Date</TableCell>
                    <TableCell>Status</TableCell>
                    <TableCell align="right">Amount</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {orders && orders.slice(0, 5).map((order) => (
                    <TableRow key={order.id}>
                      <TableCell>{order.id.substring(0, 8)}...</TableCell>
                      <TableCell>{`${order.userId.substring(0, 6)}...`}</TableCell>
                      <TableCell>{formatDate(order.createdAt)}</TableCell>
                      <TableCell>
                        <Box
                          sx={{
                            bgcolor: order.status === 'DELIVERED' ? 'success.light' : 
                                    order.status === 'CANCELLED' ? 'error.light' : 'warning.light',
                            color: order.status === 'DELIVERED' ? 'success.dark' : 
                                   order.status === 'CANCELLED' ? 'error.dark' : 'warning.dark',
                            py: 0.5,
                            px: 1,
                            borderRadius: 1,
                            display: 'inline-block',
                            fontSize: '0.75rem',
                          }}
                        >
                          {order.status}
                        </Box>
                      </TableCell>
                      <TableCell align="right">{formatPrice(order.totalPrice)}</TableCell>
                    </TableRow>
                  ))}
                  
                  {(!orders || orders.length === 0) && (
                    <TableRow>
                      <TableCell colSpan={5} align="center">
                        No recent orders
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>
        </Grid>
        
        <Grid item xs={12} lg={4}>
          <Paper elevation={2} sx={{ p: 3, borderRadius: 2, height: '100%' }}>
            <Typography variant="h6" component="h2" gutterBottom>
              Low Stock Products
            </Typography>
            <List>
              {products && products
                .filter(product => product.stock <= 5)
                .slice(0, 5)
                .map(product => (
                  <ListItem key={product.id} divider>
                    <ListItemAvatar>
                      <Avatar
                        alt={product.name}
                        src={product.imageUrl}
                        variant="rounded"
                        sx={{ bgcolor: 'grey.100' }}
                      />
                    </ListItemAvatar>
                    <ListItemText
                      primary={product.name}
                      secondary={
                        <>
                          <Typography
                            variant="body2"
                            color={product.stock === 0 ? 'error.main' : 'warning.main'}
                            component="span"
                          >
                            {product.stock === 0 ? 'Out of stock' : `Only ${product.stock} left`}
                          </Typography>
                          <br />
                          <Typography variant="body2" component="span">
                            {formatPrice(product.price)}
                          </Typography>
                        </>
                      }
                    />
                  </ListItem>
                ))}
              
              {(!products || products.filter(product => product.stock <= 5).length === 0) && (
                <ListItem>
                  <ListItemText
                    primary="No low stock products"
                    secondary="All products have sufficient inventory"
                  />
                </ListItem>
              )}
            </List>
            <Box sx={{ mt: 2 }}>
              <Button
                component={RouterLink}
                to="/admin/products"
                size="small"
                fullWidth
                variant="outlined"
              >
                Manage Inventory
              </Button>
            </Box>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
};

export default AdminDashboardPage;
