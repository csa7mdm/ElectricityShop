import api from '../config/axios';
import { Product, Category } from '../store/slices/productSlice';

interface ProductsResponse {
  items: Product[];
  totalItems: number;
  totalPages: number;
  currentPage: number;
}

const productService = {
  getAllProducts: async (params?: any): Promise<ProductsResponse> => {
    const response = await api.get('/products', { params });
    return response.data;
  },

  getProductById: async (id: string): Promise<Product> => {
    const response = await api.get(`/products/${id}`);
    return response.data;
  },

  getCategories: async (): Promise<Category[]> => {
    const response = await api.get('/products/categories');
    return response.data;
  },

  createProduct: async (productData: Partial<Product>): Promise<Product> => {
    const response = await api.post('/products', productData);
    return response.data;
  },

  updateProduct: async (id: string, productData: Partial<Product>): Promise<Product> => {
    const response = await api.put(`/products/${id}`, productData);
    return response.data;
  },

  deleteProduct: async (id: string): Promise<void> => {
    await api.delete(`/products/${id}`);
  },
};

export default productService;
