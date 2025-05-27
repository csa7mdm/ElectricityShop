import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import api from '../../config/axios';
import { OrderState, Order, Address, PaginationParams } from '../../types';
import { resetCart } from '../cart/cartSlice';

// Initial state
const initialState: OrderState = {
  orders: [],
  order: null,
  isLoading: false,
  error: null,
};

// Fetch user orders
export const fetchUserOrders = createAsyncThunk(
  'order/fetchUserOrders',
  async (_, { rejectWithValue }) => {
    try {
      const response = await api.get('/orders');
      return response.data;
    } catch (error: any) {
      return rejectWithValue(
        error.response?.data?.message || 'Failed to fetch orders'
      );
    }
  }
);

// Fetch all orders (admin)
export const fetchAllOrders = createAsyncThunk(
  'order/fetchAllOrders',
  async (params: PaginationParams, { rejectWithValue }) => {
    try {
      const response = await api.get('/orders/all', { params });
      return response.data;
    } catch (error: any) {
      return rejectWithValue(
        error.response?.data?.message || 'Failed to fetch all orders'
      );
    }
  }
);

// Fetch order details
export const fetchOrderById = createAsyncThunk(
  'order/fetchOrderById',
  async (orderId: string, { rejectWithValue }) => {
    try {
      const response = await api.get(`/orders/${orderId}`);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(
        error.response?.data?.message || 'Failed to fetch order details'
      );
    }
  }
);

// Create order
export const createOrder = createAsyncThunk(
  'order/createOrder',
  async (
    {
      shippingAddress,
      billingAddress,
      paymentMethod,
    }: {
      shippingAddress: Address;
      billingAddress: Address;
      paymentMethod: string;
    },
    { dispatch, rejectWithValue }
  ) => {
    try {
      const response = await api.post('/orders', {
        shippingAddress,
        billingAddress,
        paymentMethod,
      });
      
      // Reset cart after successful order
      dispatch(resetCart());
      
      return response.data;
    } catch (error: any) {
      return rejectWithValue(
        error.response?.data?.message || 'Failed to create order'
      );
    }
  }
);

// Update order status (admin)
export const updateOrderStatus = createAsyncThunk(
  'order/updateOrderStatus',
  async (
    { orderId, status }: { orderId: string; status: string },
    { rejectWithValue }
  ) => {
    try {
      const response = await api.put(`/orders/${orderId}/status`, { status });
      return response.data;
    } catch (error: any) {
      return rejectWithValue(
        error.response?.data?.message || 'Failed to update order status'
      );
    }
  }
);

// Order slice
const orderSlice = createSlice({
  name: 'order',
  initialState,
  reducers: {
    clearOrderError: (state) => {
      state.error = null;
    },
    clearOrderDetails: (state) => {
      state.order = null;
    },
  },
  extraReducers: (builder) => {
    // Fetch user orders
    builder.addCase(fetchUserOrders.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(fetchUserOrders.fulfilled, (state, action: PayloadAction<Order[]>) => {
      state.isLoading = false;
      state.orders = action.payload;
    });
    builder.addCase(fetchUserOrders.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string;
    });

    // Fetch all orders (admin)
    builder.addCase(fetchAllOrders.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(fetchAllOrders.fulfilled, (state, action: PayloadAction<Order[]>) => {
      state.isLoading = false;
      state.orders = action.payload;
    });
    builder.addCase(fetchAllOrders.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string;
    });

    // Fetch order by ID
    builder.addCase(fetchOrderById.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(fetchOrderById.fulfilled, (state, action: PayloadAction<Order>) => {
      state.isLoading = false;
      state.order = action.payload;
    });
    builder.addCase(fetchOrderById.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string;
    });

    // Create order
    builder.addCase(createOrder.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(createOrder.fulfilled, (state, action: PayloadAction<Order>) => {
      state.isLoading = false;
      state.order = action.payload;
      state.orders = [action.payload, ...state.orders];
    });
    builder.addCase(createOrder.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string;
    });

    // Update order status
    builder.addCase(updateOrderStatus.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(updateOrderStatus.fulfilled, (state, action: PayloadAction<Order>) => {
      state.isLoading = false;
      state.order = action.payload;
      state.orders = state.orders.map((order) =>
        order.id === action.payload.id ? action.payload : order
      );
    });
    builder.addCase(updateOrderStatus.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string;
    });
  },
});

export const { clearOrderError, clearOrderDetails } = orderSlice.actions;
export default orderSlice.reducer;
