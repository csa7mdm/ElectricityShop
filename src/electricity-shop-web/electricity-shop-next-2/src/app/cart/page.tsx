'use client';

import { useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import PageLayout from '../components/layout/PageLayout';
import { useCart } from '../context/CartContext';

export default function CartPage() {
  const { cart, removeFromCart, updateQuantity, clearCart } = useCart();
  const router = useRouter();
  const [promoCode, setPromoCode] = useState('');
  const [promoCodeApplied, setPromoCodeApplied] = useState(false);
  const [promoError, setPromoError] = useState('');
  
  const discount = promoCodeApplied ? cart.totalPrice * 0.1 : 0;
  const shippingCost = cart.totalPrice > 999 ? 0 : 49.99;
  const totalWithShipping = cart.totalPrice - discount + shippingCost;
  
  const handleApplyPromoCode = () => {
    if (promoCode.toLowerCase() === 'solar10') {
      setPromoCodeApplied(true);
      setPromoError('');
    } else {
      setPromoCodeApplied(false);
      setPromoError('Invalid promo code');
    }
  };
  
  const handleCheckout = () => {
    // In a real app, we would navigate to checkout
    router.push('/checkout');
  };
  
  return (
    <PageLayout>
      <div className="container mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold mb-8">Your Cart</h1>
        
        {cart.items.length === 0 ? (
          <div className="text-center py-16">
            <svg 
              xmlns="http://www.w3.org/2000/svg" 
              className="h-16 w-16 mx-auto text-gray-400 mb-4" 
              fill="none" 
              viewBox="0 0 24 24" 
              stroke="currentColor"
            >
              <path 
                strokeLinecap="round" 
                strokeLinejoin="round" 
                strokeWidth={2} 
                d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" 
              />
            </svg>
            <h2 className="text-2xl font-bold mb-2">Your cart is empty</h2>
            <p className="text-gray-600 mb-6">
              Looks like you haven't added any products to your cart yet.
            </p>
            <Link 
              href="/products/solar" 
              className="inline-block bg-green-600 hover:bg-green-700 text-white font-bold py-3 px-6 rounded-lg"
            >
              Browse Products
            </Link>
          </div>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            {/* Cart Items */}
            <div className="lg:col-span-2">
              <div className="bg-white rounded-lg shadow overflow-hidden">
                <div className="px-6 py-4 border-b border-gray-200 bg-gray-50">
                  <h2 className="font-bold text-lg">Cart Items ({cart.totalItems})</h2>
                </div>
                
                <ul className="divide-y divide-gray-200">
                  {cart.items.map((item) => (
                    <li key={item.product.id} className="p-6">
                      <div className="flex flex-col sm:flex-row">
                        {/* Product Image */}
                        <div className="h-24 w-24 bg-gray-200 rounded flex items-center justify-center text-gray-500 mb-4 sm:mb-0">
                          Product Image
                        </div>
                        
                        {/* Product Details */}
                        <div className="sm:ml-6 flex-1">
                          <div className="flex flex-col sm:flex-row sm:justify-between mb-4">
                            <div>
                              <h3 className="text-lg font-medium text-gray-900">
                                {item.product.name}
                              </h3>
                              <p className="mt-1 text-sm text-gray-500">
                                {item.product.category}
                              </p>
                            </div>
                            <p className="mt-1 text-lg font-medium text-gray-900 sm:text-right">
                              ${item.product.price.toFixed(2)} each
                            </p>
                          </div>
                          
                          <div className="flex justify-between items-center">
                            <div className="flex items-center">
                              <button
                                onClick={() => updateQuantity(item.product.id, item.quantity - 1)}
                                className="bg-gray-200 px-3 py-1 rounded-l"
                              >
                                -
                              </button>
                              <input
                                type="number"
                                value={item.quantity}
                                onChange={(e) => updateQuantity(item.product.id, parseInt(e.target.value) || 1)}
                                min="1"
                                className="w-12 text-center border-y border-gray-200 py-1"
                              />
                              <button
                                onClick={() => updateQuantity(item.product.id, item.quantity + 1)}
                                className="bg-gray-200 px-3 py-1 rounded-r"
                              >
                                +
                              </button>
                            </div>
                            
                            <div className="flex items-center">
                              <p className="text-lg font-medium text-gray-900">
                                ${(item.product.price * item.quantity).toFixed(2)}
                              </p>
                              <button
                                onClick={() => removeFromCart(item.product.id)}
                                className="ml-4 text-red-500 hover:text-red-700"
                              >
                                <svg 
                                  xmlns="http://www.w3.org/2000/svg" 
                                  className="h-6 w-6" 
                                  fill="none" 
                                  viewBox="0 0 24 24" 
                                  stroke="currentColor"
                                >
                                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                                </svg>
                              </button>
                            </div>
                          </div>
                        </div>
                      </div>
                    </li>
                  ))}
                </ul>
                
                <div className="px-6 py-4 border-t border-gray-200 bg-gray-50 flex justify-between">
                  <button
                    onClick={() => clearCart()}
                    className="text-red-600 hover:text-red-800 font-medium"
                  >
                    Clear Cart
                  </button>
                  <Link 
                    href="/" 
                    className="text-blue-600 hover:text-blue-800 font-medium"
                  >
                    Continue Shopping
                  </Link>
                </div>
              </div>
            </div>
            
            {/* Order Summary */}
            <div className="lg:col-span-1">
              <div className="bg-white rounded-lg shadow overflow-hidden sticky top-4">
                <div className="px-6 py-4 border-b border-gray-200 bg-gray-50">
                  <h2 className="font-bold text-lg">Order Summary</h2>
                </div>
                
                <div className="p-6">
                  <div className="flex justify-between mb-4">
                    <span className="text-gray-600">Subtotal</span>
                    <span className="font-medium">${cart.totalPrice.toFixed(2)}</span>
                  </div>
                  
                  {promoCodeApplied && (
                    <div className="flex justify-between mb-4 text-green-600">
                      <span>Discount (10%)</span>
                      <span>-${discount.toFixed(2)}</span>
                    </div>
                  )}
                  
                  <div className="flex justify-between mb-4">
                    <span className="text-gray-600">Shipping</span>
                    <span className="font-medium">
                      {shippingCost === 0 ? 'Free' : `$${shippingCost.toFixed(2)}`}
                    </span>
                  </div>
                  
                  <div className="border-t border-gray-200 pt-4 mb-6">
                    <div className="flex justify-between mb-2">
                      <span className="font-bold">Total</span>
                      <span className="font-bold">${totalWithShipping.toFixed(2)}</span>
                    </div>
                    <p className="text-gray-500 text-sm">
                      Including VAT
                    </p>
                  </div>
                  
                  {/* Promo Code */}
                  <div className="mb-6">
                    <label htmlFor="promo" className="block text-sm font-medium text-gray-700 mb-1">
                      Promo Code
                    </label>
                    <div className="flex">
                      <input
                        type="text"
                        id="promo"
                        value={promoCode}
                        onChange={(e) => setPromoCode(e.target.value)}
                        placeholder="Enter promo code"
                        className="flex-grow px-4 py-2 border border-gray-300 rounded-l focus:ring-green-500 focus:border-green-500"
                      />
                      <button
                        onClick={handleApplyPromoCode}
                        className="bg-gray-200 hover:bg-gray-300 px-4 py-2 rounded-r"
                      >
                        Apply
                      </button>
                    </div>
                    {promoError && (
                      <p className="text-red-600 text-sm mt-1">{promoError}</p>
                    )}
                    {promoCodeApplied && (
                      <p className="text-green-600 text-sm mt-1">Promo code applied!</p>
                    )}
                    <p className="text-gray-500 text-xs mt-2">
                      Try "SOLAR10" for 10% off your order
                    </p>
                  </div>
                  
                  <button
                    onClick={handleCheckout}
                    className="w-full bg-green-600 hover:bg-green-700 text-white font-bold py-3 px-4 rounded-lg"
                  >
                    Proceed to Checkout
                  </button>
                  
                  <div className="mt-4 flex items-center justify-center space-x-2">
                    <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                    </svg>
                    <span className="text-sm text-gray-500">Secure Checkout</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </PageLayout>
  );
}
