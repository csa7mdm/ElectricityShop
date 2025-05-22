/**
 * API service for handling communication with the .NET backend
 */

// Base API URL - we'll use environment variables in production
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

/**
 * Generic fetch function with error handling
 */
async function fetchApi<T>(
  endpoint: string, 
  options?: RequestInit
): Promise<T> {
  const url = `${API_BASE_URL}${endpoint}`;
  
  const response = await fetch(url, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options?.headers,
    },
  });

  if (!response.ok) {
    // Handle different error status codes
    if (response.status === 401) {
      throw new Error('Unauthorized. Please login to continue.');
    } else if (response.status === 403) {
      throw new Error('You do not have permission to access this resource.');
    } else if (response.status === 404) {
      throw new Error('Resource not found.');
    }
    
    // Generic error handling
    const error = await response.text();
    throw new Error(error || 'Something went wrong with the API request.');
  }

  return response.json();
}

/**
 * Product related API calls
 */
export const ProductsApi = {
  getAllProducts: () => fetchApi<Product[]>('/products'),
  
  getProductById: (id: string) => fetchApi<Product>(`/products/${id}`),
  
  getProductsByCategory: (category: string) => 
    fetchApi<Product[]>(`/products/category/${category}`),
  
  searchProducts: (query: string) => 
    fetchApi<Product[]>(`/products/search?query=${encodeURIComponent(query)}`),
};

/**
 * Order related API calls
 */
export const OrdersApi = {
  placeOrder: (orderData: any) => 
    fetchApi<any>('/orders', {
      method: 'POST',
      body: JSON.stringify(orderData),
    }),
  
  getOrderById: (id: string) => fetchApi<any>(`/orders/${id}`),
  
  getCustomerOrders: (customerId: string) => 
    fetchApi<any[]>(`/orders/customer/${customerId}`),
};

/**
 * Customer related API calls
 */
export const CustomersApi = {
  register: (customerData: any) => 
    fetchApi<any>('/customers/register', {
      method: 'POST',
      body: JSON.stringify(customerData),
    }),
  
  login: (credentials: { email: string; password: string }) => 
    fetchApi<any>('/customers/login', {
      method: 'POST',
      body: JSON.stringify(credentials),
    }),
  
  getProfile: (customerId: string) => 
    fetchApi<any>(`/customers/${customerId}`),
  
  updateProfile: (customerId: string, profileData: any) => 
    fetchApi<any>(`/customers/${customerId}`, {
      method: 'PUT',
      body: JSON.stringify(profileData),
    }),
};

/**
 * Contact form API calls
 */
export const ContactApi = {
  submitContactForm: (formData: any) => 
    fetchApi<any>('/contact', {
      method: 'POST',
      body: JSON.stringify(formData),
    }),
};

export default {
  ProductsApi,
  OrdersApi,
  CustomersApi,
  ContactApi,
};
