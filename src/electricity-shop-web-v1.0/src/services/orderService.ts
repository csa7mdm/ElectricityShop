import api from '../config/axios';
import { Order, Address } from '../store/slices/orderSlice';

interface OrdersResponse {
  items: Order[];
  totalItems: number;
  totalPages: number;
  currentPage: number;
}

const orderService = {
  getOrders: async (params?: any): Promise<OrdersResponse> => {
    const response = await api.get('/orders', { params });
    return response.data;
  },

  getOrderById: async (id: string): Promise<Order> => {
    const response = await api.get(`/orders/${id}`);
    return response.data;
  },

  createOrder: async (orderData: { shippingAddress: Address }): Promise<Order> => {
    const response = await api.post('/orders', orderData);
    return response.data;
  },

  cancelOrder: async (id: string): Promise<Order> => {
    const response = await api.put(`/orders/${id}/cancel`);
    return response.data;
  },

  processPayment: async (orderId: string, paymentDetails: any): Promise<Order> => {
    const response = await api.post(`/orders/${orderId}/pay`, paymentDetails);
    return response.data;
  },
};

export default orderService;
