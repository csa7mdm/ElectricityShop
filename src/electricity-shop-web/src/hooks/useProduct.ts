import { useSelector, useDispatch } from 'react-redux';
import { RootState } from '../store';
import {
  fetchProducts,
  fetchProductById,
  fetchCategories,
  createProduct,
  updateProduct,
  deleteProduct,
} from '../store/product/productSlice';
import { ProductFilterParams, Product } from '../types';

export const useProduct = () => {
  const dispatch = useDispatch();
  const { products, product, categories, isLoading, error, totalItems, currentPage, totalPages } = 
    useSelector((state: RootState) => state.product);
  
  const handleFetchProducts = async (params: ProductFilterParams) => {
    try {
      await dispatch(fetchProducts(params)).unwrap();
      return true;
    } catch (error) {
      return false;
    }
  };
  
  const handleFetchProductById = async (productId: string) => {
    try {
      await dispatch(fetchProductById(productId)).unwrap();
      return true;
    } catch (error) {
      return false;
    }
  };
  
  const handleFetchCategories = async () => {
    try {
      await dispatch(fetchCategories()).unwrap();
      return true;
    } catch (error) {
      return false;
    }
  };
  
  const handleCreateProduct = async (productData: Partial<Product>) => {
    try {
      await dispatch(createProduct(productData)).unwrap();
      return true;
    } catch (error) {
      return false;
    }
  };
  
  const handleUpdateProduct = async (id: string, productData: Partial<Product>) => {
    try {
      await dispatch(updateProduct({ id, productData })).unwrap();
      return true;
    } catch (error) {
      return false;
    }
  };
  
  const handleDeleteProduct = async (productId: string) => {
    try {
      await dispatch(deleteProduct(productId)).unwrap();
      return true;
    } catch (error) {
      return false;
    }
  };
  
  return {
    products,
    product,
    categories,
    isLoading,
    error,
    totalItems,
    currentPage,
    totalPages,
    fetchProducts: handleFetchProducts,
    fetchProductById: handleFetchProductById,
    fetchCategories: handleFetchCategories,
    createProduct: handleCreateProduct,
    updateProduct: handleUpdateProduct,
    deleteProduct: handleDeleteProduct,
  };
};
