import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import api from '../../config/axios';
import { Product, ProductState, ProductFilterParams } from '../../types';

// Initial state
const initialState: ProductState = {
  products: [
    {
      id: '1',
      name: 'Example Product 1',
      description: 'This is a placeholder description for product 1.',
      price: 19.99,
      category: 'Electronics',
      imageUrl: '',
      placeholderImageUrl: 'https://via.placeholder.com/150',
      stock: 10,
      rating: 4,
      reviews: [],
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    },
    {
      id: '2',
      name: 'Example Product 2',
      description: 'This is a placeholder description for product 2.',
      price: 29.99,
      category: 'Appliances',
      imageUrl: '',
      placeholderImageUrl: 'https://via.placeholder.com/150',
      stock: 5,
      rating: 3,
      reviews: [],
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    },
  ],
  product: null,
  categories: [],
  isLoading: false,
  error: null,
  totalItems: 2,
  currentPage: 1,
  totalPages: 1,
};

// Fetch all products with filters
export const fetchProducts = createAsyncThunk(
  'product/fetchProducts',
  async (params: ProductFilterParams, { rejectWithValue }) => {
    try {
      const response = await api.get('/products', { params });
      return response.data;
    } catch (error: any) {
      return rejectWithValue(
        error.response?.data?.message || 'Failed to fetch products'
      );
    }
  }
);

// Fetch single product
export const fetchProductById = createAsyncThunk(
  'product/fetchProductById',
  async (productId: string, { rejectWithValue }) => {
    try {
      const response = await api.get(`/products/${productId}`);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(
        error.response?.data?.message || 'Failed to fetch product details'
      );
    }
  }
);

// Fetch all categories
export const fetchCategories = createAsyncThunk(
  'product/fetchCategories',
  async (_, { rejectWithValue }) => {
    try {
      const response = await api.get('/products/categories');
      return response.data;
    } catch (error: any) {
      return rejectWithValue(
        error.response?.data?.message || 'Failed to fetch categories'
      );
    }
  }
);

// Create product (admin)
export const createProduct = createAsyncThunk(
  'product/createProduct',
  async (productData: Partial<Product>, { rejectWithValue }) => {
    try {
      const response = await api.post('/products', productData);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(
        error.response?.data?.message || 'Failed to create product'
      );
    }
  }
);

// Update product (admin)
export const updateProduct = createAsyncThunk(
  'product/updateProduct',
  async (
    { id, productData }: { id: string; productData: Partial<Product> },
    { rejectWithValue }
  ) => {
    try {
      const response = await api.put(`/products/${id}`, productData);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(
        error.response?.data?.message || 'Failed to update product'
      );
    }
  }
);

// Delete product (admin)
export const deleteProduct = createAsyncThunk(
  'product/deleteProduct',
  async (productId: string, { rejectWithValue }) => {
    try {
      await api.delete(`/products/${productId}`);
      return productId;
    } catch (error: any) {
      return rejectWithValue(
        error.response?.data?.message || 'Failed to delete product'
      );
    }
  }
);

// Product slice
const productSlice = createSlice({
  name: 'product',
  initialState,
  reducers: {
    clearProductError: (state) => {
      state.error = null;
    },
    clearProductDetails: (state) => {
      state.product = null;
    },
  },
  extraReducers: (builder) => {
    // Fetch products
    builder.addCase(fetchProducts.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(fetchProducts.fulfilled, (state, action: PayloadAction<any>) => {
      state.isLoading = false;
      state.products = action.payload.products;
      state.totalItems = action.payload.totalItems;
      state.currentPage = action.payload.currentPage;
      state.totalPages = action.payload.totalPages;
    });
    builder.addCase(fetchProducts.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string;
    });

    // Fetch product by ID
    builder.addCase(fetchProductById.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(fetchProductById.fulfilled, (state, action: PayloadAction<Product>) => {
      state.isLoading = false;
      state.product = action.payload;
    });
    builder.addCase(fetchProductById.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string;
    });

    // Fetch categories
    builder.addCase(fetchCategories.pending, (state) => {
      state.isLoading = true;
    });
    builder.addCase(fetchCategories.fulfilled, (state, action: PayloadAction<string[]>) => {
      state.isLoading = false;
      state.categories = action.payload;
    });
    builder.addCase(fetchCategories.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string;
    });

    // Create product
    builder.addCase(createProduct.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(createProduct.fulfilled, (state, action: PayloadAction<Product>) => {
      state.isLoading = false;
      state.products = [action.payload, ...state.products];
    });
    builder.addCase(createProduct.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string;
    });

    // Update product
    builder.addCase(updateProduct.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(updateProduct.fulfilled, (state, action: PayloadAction<Product>) => {
      state.isLoading = false;
      state.product = action.payload;
      state.products = state.products.map((product) =>
        product.id === action.payload.id ? action.payload : product
      );
    });
    builder.addCase(updateProduct.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string;
    });

    // Delete product
    builder.addCase(deleteProduct.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(deleteProduct.fulfilled, (state, action: PayloadAction<string>) => {
      state.isLoading = false;
      state.products = state.products.filter((product) => product.id !== action.payload);
    });
    builder.addCase(deleteProduct.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string;
    });
  },
});

export const { clearProductError, clearProductDetails } = productSlice.actions;
export default productSlice.reducer;
