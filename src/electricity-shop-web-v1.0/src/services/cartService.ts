import api from '../config/axios';
import { Cart } from '../store/slices/cartSlice';

const cartService = {
  getCart: async (): Promise<Cart> => {
    const response = await api.get('/cart');
    return response.data;
  },

  addToCart: async (productId: string, quantity: number): Promise<Cart> => {
    const response = await api.post('/cart/items', { productId, quantity });
    return response.data;
  },

  updateCartItem: async (itemId: string, quantity: number): Promise<Cart> => {
    const response = await api.put(`/cart/items/${itemId}`, { quantity });
    return response.data;
  },

  removeFromCart: async (itemId: string): Promise<void> => {
    await api.delete(`/cart/items/${itemId}`);
  },
};

export default cartService;
