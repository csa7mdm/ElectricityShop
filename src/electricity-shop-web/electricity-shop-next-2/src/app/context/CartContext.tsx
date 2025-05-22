'use client';

import { createContext, useReducer, useContext, ReactNode } from 'react';
import { CartState, CartAction, cartReducer, initialCartState } from '../lib/cart';
import { Product } from '../lib/products';

type CartContextType = {
  cart: CartState;
  addToCart: (product: Product) => void;
  removeFromCart: (productId: string) => void;
  updateQuantity: (productId: string, quantity: number) => void;
  clearCart: () => void;
};

const CartContext = createContext<CartContextType | undefined>(undefined);

export function CartProvider({ children }: { children: ReactNode }) {
  const [cart, dispatch] = useReducer(cartReducer, initialCartState);

  function addToCart(product: Product) {
    dispatch({ type: 'ADD_TO_CART', payload: product });
  }

  function removeFromCart(productId: string) {
    dispatch({ type: 'REMOVE_FROM_CART', payload: productId });
  }

  function updateQuantity(productId: string, quantity: number) {
    dispatch({
      type: 'UPDATE_QUANTITY',
      payload: { productId, quantity },
    });
  }

  function clearCart() {
    dispatch({ type: 'CLEAR_CART' });
  }

  const value = {
    cart,
    addToCart,
    removeFromCart,
    updateQuantity,
    clearCart,
  };

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
}

export function useCart() {
  const context = useContext(CartContext);
  if (context === undefined) {
    throw new Error('useCart must be used within a CartProvider');
  }
  return context;
}
