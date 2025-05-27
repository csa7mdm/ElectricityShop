// User related types
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: 'admin' | 'user';
  createdAt: string;
  updatedAt: string;
}

export interface Address {
  id: string;
  userId: string;
  firstName: string;
  lastName: string;
  street: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
  isDefault: boolean;
  phone: string;
}

export interface AuthState {
  user: User | null;
  token: string | null;
  refreshToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
}

// Product related types
export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  category: string;
  imageUrl: string;
  placeholderImageUrl: string;
  stock: number;
  rating: number;
  reviews: Review[];
  createdAt: string;
  updatedAt: string;
}

export interface Review {
  id: string;
  productId: string;
  userId: string;
  userName: string;
  rating: number;
  comment: string;
  createdAt: string;
}

export interface ProductState {
  products: Product[];
  product: Product | null;
  categories: string[];
  isLoading: boolean;
  error: string | null;
  totalItems: number;
  currentPage: number;
  totalPages: number;
}

// Cart related types
export interface CartItem {
  id: string;
  productId: string;
  product: Product;
  quantity: number;
  price: number;
}

export interface Cart {
  id: string;
  userId: string;
  items: CartItem[];
  totalQuantity: number;
  totalPrice: number;
  updatedAt: string;
}

export interface CartState {
  cart: Cart | null;
  isLoading: boolean;
  error: string | null;
}

// Order related types
export enum OrderStatus {
  PENDING = 'PENDING',
  PROCESSING = 'PROCESSING',
  SHIPPED = 'SHIPPED',
  DELIVERED = 'DELIVERED',
  CANCELLED = 'CANCELLED',
}

export enum PaymentStatus {
  PENDING = 'PENDING',
  COMPLETED = 'COMPLETED',
  FAILED = 'FAILED',
  REFUNDED = 'REFUNDED',
}

export interface OrderItem {
  id: string;
  orderId: string;
  productId: string;
  productName: string;
  quantity: number;
  price: number;
}

export interface Payment {
  id: string;
  orderId: string;
  amount: number;
  method: string;
  status: PaymentStatus;
  transactionId: string;
  createdAt: string;
}

export interface Order {
  id: string;
  userId: string;
  items: OrderItem[];
  shippingAddress: Address;
  billingAddress: Address;
  totalPrice: number;
  tax: number;
  shippingCost: number;
  status: OrderStatus;
  payment: Payment;
  createdAt: string;
  updatedAt: string;
}

export interface OrderState {
  orders: Order[];
  order: Order | null;
  isLoading: boolean;
  error: string | null;
}

// User state for admin operations
export interface UserState {
  users: User[];
  selectedUser: User | null;
  isLoading: boolean;
  error: string | null;
  totalItems: number;
  currentPage: number;
  totalPages: number;
}

// Pagination params
export interface PaginationParams {
  page: number;
  limit: number;
}

// Filter params for products
export interface ProductFilterParams extends PaginationParams {
  search?: string;
  category?: string;
  minPrice?: number;
  maxPrice?: number;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}
