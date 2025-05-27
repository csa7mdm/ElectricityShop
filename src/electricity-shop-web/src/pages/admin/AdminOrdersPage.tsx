import React, { useEffect, useState } from 'react';
import {
  Container,
  Typography,
  Box,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TablePagination,
  TextField,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  MenuItem,
  Chip,
  IconButton,
  Grid,
  Alert,
  Drawer,
  List,
  ListItem,
  ListItemText,
  Divider,
} from '@mui/material';
import {
  Search as SearchIcon,
  Visibility as VisibilityIcon,
  FilterList as FilterListIcon,
} from '@mui/icons-material';

import { useOrder } from '../../hooks/useOrder';
import { Order, OrderStatus, PaginationParams } from '../../types';
import { formatPrice, formatDate, formatOrderStatus } from '../../utils/formatters';
import LoadingSpinner from '../../components/common/LoadingSpinner';

const AdminOrdersPage: React.FC = () => {
  const { orders, order, isLoading, error, fetchAllOrders, fetchOrderById, updateOrderStatus } = useOrder();
  
  // State for drawer and dialog
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);
  const [statusDialogOpen, setStatusDialogOpen] = useState(false);
  const [newStatus, setNewStatus] = useState<OrderStatus>(OrderStatus.PENDING);
  const [filterDrawerOpen, setFilterDrawerOpen] = useState(false);
  
  // State for pagination and filtering
  const [paginationParams, setPaginationParams] = useState<PaginationParams>({
    page: 1,
    limit: 10,
  });
  const [filters, setFilters] = useState({
    search: '',
    status: '',
    startDate: '',
    endDate: '',
  });
  
  // Fetch orders
  useEffect(() => {
    fetchAllOrders(paginationParams);
  }, [fetchAllOrders, paginationParams]);
  
  // Handle view order details
  const handleViewOrder = async (orderId: string) => {
    await fetchOrderById(orderId);
    const foundOrder = orders.find(o => o.id === orderId);
    setSelectedOrder(foundOrder || null);
  };
  
  // Handle close order details drawer
  const handleCloseOrderDetails = () => {
    setSelectedOrder(null);
  };
  
  // Handle status change dialog
  const handleStatusChangeClick = (order: Order) => {
    setSelectedOrder(order);
    setNewStatus(order.status);
    setStatusDialogOpen(true);
  };
  
  // Handle status update
  const handleUpdateStatus = async () => {
    if (selectedOrder && newStatus) {
      await updateOrderStatus(selectedOrder.id, newStatus);
      setStatusDialogOpen(false);
      fetchAllOrders(paginationParams); // Refresh orders
    }
  };
  
  // Handle search
  const handleSearch = (event: React.ChangeEvent<HTMLInputElement>) => {
    const value = event.target.value;
    setFilters({
      ...filters,
      search: value,
    });
    setPaginationParams({
      ...paginationParams,
      page: 1, // Reset to first page
    });
  };
  
  // Handle page change
  const handlePageChange = (_: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    setPaginationParams({
      ...paginationParams,
      page: newPage + 1, // MUI pagination is 0-based, but our API is 1-based
    });
  };
  
  // Handle rows per page change
  const handleRowsPerPageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setPaginationParams({
      limit: parseInt(event.target.value, 10),
      page: 1, // Reset to first page
    });
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
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <LoadingSpinner isOpen={isLoading && !orders.length} message="Loading orders..." />
      
      <Typography variant="h4" component="h1" gutterBottom>
        Manage Orders
      </Typography>
      
      {error && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}
      
      <Paper sx={{ p: 2, mb: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
          <Box sx={{ display: 'flex', gap: 2, flexGrow: 1 }}>
            <TextField
              placeholder="Search by order ID or customer..."
              variant="outlined"
              size="small"
              sx={{ minWidth: 300 }}
              value={filters.search}
              onChange={handleSearch}
              InputProps={{
                startAdornment: <SearchIcon fontSize="small" sx={{ mr: 1 }} />,
              }}
            />
            
            <Button
              variant="outlined"
              startIcon={<FilterListIcon />}
              onClick={() => setFilterDrawerOpen(true)}
            >
              Filters
            </Button>
          </Box>
        </Box>
        
        <TableContainer>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Order ID</TableCell>
                <TableCell>Customer</TableCell>
                <TableCell>Date</TableCell>
                <TableCell>Status</TableCell>
                <TableCell align="right">Total</TableCell>
                <TableCell align="right">Items</TableCell>
                <TableCell align="center">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {orders.map((order) => (
                <TableRow key={order.id}>
                  <TableCell>{order.id.substring(0, 8)}...</TableCell>
                  <TableCell>{order.userId.substring(0, 8)}...</TableCell>
                  <TableCell>{formatDate(order.createdAt)}</TableCell>
                  <TableCell>
                    <Chip
                      label={formatOrderStatus(order.status)}
                      size="small"
                      color={getStatusColor(order.status) as any}
                      onClick={() => handleStatusChangeClick(order)}
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
              
              {orders.length === 0 && !isLoading && (
                <TableRow>
                  <TableCell colSpan={7} align="center">
                    No orders found
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
        
        <TablePagination
          component="div"
          count={100} // Total count from API
          page={paginationParams.page - 1} // MUI pagination is 0-based, but our API is 1-based
          rowsPerPage={paginationParams.limit}
          onPageChange={handlePageChange}
          onRowsPerPageChange={handleRowsPerPageChange}
          rowsPerPageOptions={[5, 10, 25, 50]}
        />
      </Paper>
      
      {/* Order Details Drawer */}
      <Drawer
        anchor="right"
        open={!!selectedOrder}
        onClose={handleCloseOrderDetails}
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
                variant="contained"
                color="primary"
                onClick={() => handleStatusChangeClick(selectedOrder)}
                fullWidth
              >
                Update Order Status
              </Button>
              <Button
                variant="outlined"
                onClick={handleCloseOrderDetails}
                sx={{ mt: 2 }}
                fullWidth
              >
                Close
              </Button>
            </Box>
          </>
        )}
      </Drawer>
      
      {/* Filter Drawer */}
      <Drawer
        anchor="right"
        open={filterDrawerOpen}
        onClose={() => setFilterDrawerOpen(false)}
        PaperProps={{ sx: { width: 300, p: 3 } }}
      >
        <Typography variant="h6" gutterBottom>
          Filter Orders
        </Typography>
        
        <TextField
          select
          label="Status"
          fullWidth
          margin="normal"
          value={filters.status}
          onChange={(e) => setFilters({ ...filters, status: e.target.value })}
        >
          <MenuItem value="">All Statuses</MenuItem>
          {Object.values(OrderStatus).map((status) => (
            <MenuItem key={status} value={status}>
              {formatOrderStatus(status)}
            </MenuItem>
          ))}
        </TextField>
        
        <TextField
          label="Start Date"
          type="date"
          fullWidth
          margin="normal"
          InputLabelProps={{ shrink: true }}
          value={filters.startDate}
          onChange={(e) => setFilters({ ...filters, startDate: e.target.value })}
        />
        
        <TextField
          label="End Date"
          type="date"
          fullWidth
          margin="normal"
          InputLabelProps={{ shrink: true }}
          value={filters.endDate}
          onChange={(e) => setFilters({ ...filters, endDate: e.target.value })}
        />
        
        <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
          <Button
            variant="contained"
            onClick={() => {
              setPaginationParams({ ...paginationParams, page: 1 });
              setFilterDrawerOpen(false);
            }}
            fullWidth
          >
            Apply Filters
          </Button>
          <Button
            variant="outlined"
            onClick={() => {
              setFilters({
                search: '',
                status: '',
                startDate: '',
                endDate: '',
              });
              setPaginationParams({ page: 1, limit: 10 });
              setFilterDrawerOpen(false);
            }}
            fullWidth
          >
            Clear Filters
          </Button>
        </Box>
      </Drawer>
      
      {/* Status Change Dialog */}
      <Dialog open={statusDialogOpen} onClose={() => setStatusDialogOpen(false)}>
        <DialogTitle>Update Order Status</DialogTitle>
        <DialogContent>
          <TextField
            select
            label="Status"
            fullWidth
            margin="normal"
            value={newStatus}
            onChange={(e) => setNewStatus(e.target.value as OrderStatus)}
          >
            {Object.values(OrderStatus).map((status) => (
              <MenuItem key={status} value={status}>
                {formatOrderStatus(status)}
              </MenuItem>
            ))}
          </TextField>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setStatusDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" color="primary" onClick={handleUpdateStatus}>
            Update
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default AdminOrdersPage;
