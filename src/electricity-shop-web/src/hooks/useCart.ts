import { useSelector, useDispatch } from 'react-redux';
import { RootState } from '../store';
import {
  fetchCart,
  addToCart,
  updateCartItem,
  removeFromCart,
  clearCart,
} from '../store/cart/cartSlice';

export const useCart = () => {
  const dispatch = useDispatch();
  const { cart, isLoading, error } = useSelector((state: RootState) => state.cart);
  
  const handleFetchCart = async () => {
    try {
      await dispatch(fetchCart()).unwrap();
      return true;
    } catch (error) {
      return false;
    }
  };
  
  const handleAddToCart = async (productId: string, quantity: number) => {
    try {
      await dispatch(addToCart({ productId, quantity })).unwrap();
      return true;
    } catch (error) {
      return false;
    }
  };
  
  const handleUpdateCartItem = async (itemId: string, quantity: number) => {
    try {
      await dispatch(updateCartItem({ itemId, quantity })).unwrap();
      return true;
    } catch (error) {
      return false;
    }
  };
  
  const handleRemoveFromCart = async (itemId: string) => {
    try {
      await dispatch(removeFromCart(itemId)).unwrap();
      return true;
    } catch (error) {
      return false;
    }
  };
  
  const handleClearCart = async () => {
    try {
      await dispatch(clearCart()).unwrap();
      return true;
    } catch (error) {
      return false;
    }
  };
  
  return {
    cart,
    isLoading,
    error,
    fetchCart: handleFetchCart,
    addToCart: handleAddToCart,
    updateCartItem: handleUpdateCartItem,
    removeFromCart: handleRemoveFromCart,
    clearCart: handleClearCart,
    itemCount: cart?.totalQuantity || 0,
    totalPrice: cart?.totalPrice || 0,
    items: cart?.items || [],
  };
};
