'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import PageLayout from '../components/layout/PageLayout';
import { useCart } from '../context/CartContext';

type CheckoutStep = 'shipping' | 'payment' | 'confirmation';

export default function CheckoutPage() {
  const { cart, clearCart } = useCart();
  const router = useRouter();
  const [currentStep, setCurrentStep] = useState<CheckoutStep>('shipping');
  const [orderPlaced, setOrderPlaced] = useState(false);
  
  // Form states
  const [shippingForm, setShippingForm] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    address: '',
    city: '',
    state: '',
    zipCode: '',
    country: 'United States',
  });
  
  const [paymentForm, setPaymentForm] = useState({
    cardName: '',
    cardNumber: '',
    expiryDate: '',
    cvv: '',
  });
  
  // Derived values
  const subtotal = cart.totalPrice;
  const shipping = subtotal > 999 ? 0 : 49.99;
  const tax = subtotal * 0.07; // 7% tax
  const total = subtotal + shipping + tax;
  
  // Form handlers
  const handleShippingChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setShippingForm((prev) => ({ ...prev, [name]: value }));
  };
  
  const handlePaymentChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setPaymentForm((prev) => ({ ...prev, [name]: value }));
  };
  
  // Navigation handlers
  const handleShippingSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setCurrentStep('payment');
    window.scrollTo(0, 0);
  };
  
  const handlePaymentSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setCurrentStep('confirmation');
    window.scrollTo(0, 0);
  };
  
  const handlePlaceOrder = () => {
    // In a real app, would submit order to backend
    setOrderPlaced(true);
    clearCart();
    
    // Auto-redirect to success page after 3 seconds
    setTimeout(() => {
      router.push('/checkout/success');
    }, 3000);
  };
  
  const handleBack = () => {
    if (currentStep === 'payment') {
      setCurrentStep('shipping');
    } else if (currentStep === 'confirmation') {
      setCurrentStep('payment');
    }
    window.scrollTo(0, 0);
  };
  
  if (cart.items.length === 0 && !orderPlaced) {
    router.push('/cart');
    return null;
  }
  
  return (
    <PageLayout>
      <div className="container mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold mb-8">Checkout</h1>
        
        {/* Progress Steps */}
        <div className="mb-8">
          <div className="flex items-center">
            <div className={`w-8 h-8 rounded-full flex items-center justify-center ${
              currentStep === 'shipping' ? 'bg-green-600 text-white' : 'bg-green-100 text-green-600'
            } font-bold`}>
              1
            </div>
            <div className={`h-1 flex-grow mx-2 ${
              currentStep === 'shipping' ? 'bg-gray-300' : 'bg-green-600'
            }`}></div>
            <div className={`w-8 h-8 rounded-full flex items-center justify-center ${
              currentStep === 'payment' ? 'bg-green-600 text-white' : currentStep === 'confirmation' ? 'bg-green-100 text-green-600' : 'bg-gray-200 text-gray-600'
            } font-bold`}>
              2
            </div>
            <div className={`h-1 flex-grow mx-2 ${
              currentStep === 'confirmation' ? 'bg-green-600' : 'bg-gray-300'
            }`}></div>
            <div className={`w-8 h-8 rounded-full flex items-center justify-center ${
              currentStep === 'confirmation' ? 'bg-green-600 text-white' : 'bg-gray-200 text-gray-600'
            } font-bold`}>
              3
            </div>
          </div>
          <div className="flex justify-between mt-2 text-sm">
            <span className={currentStep === 'shipping' ? 'text-green-600 font-medium' : ''}>Shipping</span>
            <span className={currentStep === 'payment' ? 'text-green-600 font-medium' : ''}>Payment</span>
            <span className={currentStep === 'confirmation' ? 'text-green-600 font-medium' : ''}>Confirmation</span>
          </div>
        </div>
        
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Main Content */}
          <div className="lg:col-span-2">
            {/* Shipping Form */}
            {currentStep === 'shipping' && (
              <div className="bg-white rounded-lg shadow overflow-hidden">
                <div className="px-6 py-4 border-b border-gray-200 bg-gray-50">
                  <h2 className="font-bold text-lg">Shipping Information</h2>
                </div>
                
                <form onSubmit={handleShippingSubmit} className="p-6">
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                    <div>
                      <label htmlFor="firstName" className="block text-sm font-medium text-gray-700 mb-1">
                        First Name *
                      </label>
                      <input
                        type="text"
                        id="firstName"
                        name="firstName"
                        value={shippingForm.firstName}
                        onChange={handleShippingChange}
                        required
                        className="w-full px-4 py-2 border border-gray-300 rounded focus:ring-green-500 focus:border-green-500"
                      />
                    </div>
                    
                    <div>
                      <label htmlFor="lastName" className="block text-sm font-medium text-gray-700 mb-1">
                        Last Name *
                      </label>
                      <input
                        type="text"
                        id="lastName"
                        name="lastName"
                        value={shippingForm.lastName}
                        onChange={handleShippingChange}
                        required
                        className="w-full px-4 py-2 border border-gray-300 rounded focus:ring-green-500 focus:border-green-500"
                      />
                    </div>
                    
                    <div>
                      <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-1">
                        Email Address *
                      </label>
                      <input
                        type="email"
                        id="email"
                        name="email"
                        value={shippingForm.email}
                        onChange={handleShippingChange}
                        required
                        className="w-full px-4 py-2 border border-gray-300 rounded focus:ring-green-500 focus:border-green-500"
                      />
                    </div>
                    
                    <div>
                      <label htmlFor="phone" className="block text-sm font-medium text-gray-700 mb-1">
                        Phone Number *
                      </label>
                      <input
                        type="tel"
                        id="phone"
                        name="phone"
                        value={shippingForm.phone}
                        onChange={handleShippingChange}
                        required
                        className="w-full px-4 py-2 border border-gray-300 rounded focus:ring-green-500 focus:border-green-500"
                      />
                    </div>
                  </div>
                  
                  <div className="mb-6">
                    <label htmlFor="address" className="block text-sm font-medium text-gray-700 mb-1">
                      Street Address *
                    </label>
                    <input
                      type="text"
                      id="address"
                      name="address"
                      value={shippingForm.address}
                      onChange={handleShippingChange}
                      required
                      className="w-full px-4 py-2 border border-gray-300 rounded focus:ring-green-500 focus:border-green-500"
                    />
                  </div>
                  
                  <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
                    <div>
                      <label htmlFor="city" className="block text-sm font-medium text-gray-700 mb-1">
                        City *
                      </label>
                      <input
                        type="text"
                        id="city"
                        name="city"
                        value={shippingForm.city}
                        onChange={handleShippingChange}
                        required
                        className="w-full px-4 py-2 border border-gray-300 rounded focus:ring-green-500 focus:border-green-500"
                      />
                    </div>
                    
                    <div>
                      <label htmlFor="state" className="block text-sm font-medium text-gray-700 mb-1">
                        State/Province *
                      </label>
                      <input
                        type="text"
                        id="state"
                        name="state"
                        value={shippingForm.state}
                        onChange={handleShippingChange}
                        required
                        className="w-full px-4 py-2 border border-gray-300 rounded focus:ring-green-500 focus:border-green-500"
                      />
                    </div>
                    
                    <div>
                      <label htmlFor="zipCode" className="block text-sm font-medium text-gray-700 mb-1">
                        ZIP/Postal Code *
                      </label>
                      <input
                        type="text"
                        id="zipCode"
                        name="zipCode"
                        value={shippingForm.zipCode}
                        onChange={handleShippingChange}
                        required
                        className="w-full px-4 py-2 border border-gray-300 rounded focus:ring-green-500 focus:border-green-500"
                      />
                    </div>
                  </div>
                  
                  <div className="mb-6">
                    <label htmlFor="country" className="block text-sm font-medium text-gray-700 mb-1">
                      Country *
                    </label>
                    <select
                      id="country"
                      name="country"
                      value={shippingForm.country}
                      onChange={handleShippingChange}
                      required
                      className="w-full px-4 py-2 border border-gray-300 rounded focus:ring-green-500 focus:border-green-500"
                    >
                      <option value="United States">United States</option>
                      <option value="Canada">Canada</option>
                      <option value="United Kingdom">United Kingdom</option>
                      {/* More countries would be added here */}
                    </select>
                  </div>
                  
                  <div className="flex justify-end">
                    <button
                      type="submit"
                      className="bg-green-600 hover:bg-green-700 text-white font-bold py-3 px-6 rounded-lg"
                    >
                      Continue to Payment
                    </button>
                  </div>
                </form>
              </div>
            )}
            
            {/* Payment Form */}
            {currentStep === 'payment' && (
              <div className="bg-white rounded-lg shadow overflow-hidden">
                <div className="px-6 py-4 border-b border-gray-200 bg-gray-50">
                  <h2 className="font-bold text-lg">Payment Information</h2>
                </div>
                
                <form onSubmit={handlePaymentSubmit} className="p-6">
                  <div className="mb-6">
                    <label htmlFor="cardName" className="block text-sm font-medium text-gray-700 mb-1">
                      Name on Card *
                    </label>
                    <input
                      type="text"
                      id="cardName"
                      name="cardName"
                      value={paymentForm.cardName}
                      onChange={handlePaymentChange}
                      required
                      className="w-full px-4 py-2 border border-gray-300 rounded focus:ring-green-500 focus:border-green-500"
                    />
                  </div>
                  
                  <div className="mb-6">
                    <label htmlFor="cardNumber" className="block text-sm font-medium text-gray-700 mb-1">
                      Card Number *
                    </label>
                    <input
                      type="text"
                      id="cardNumber"
                      name="cardNumber"
                      value={paymentForm.cardNumber}
                      onChange={handlePaymentChange}
                      required
                      placeholder="XXXX XXXX XXXX XXXX"
                      className="w-full px-4 py-2 border border-gray-300 rounded focus:ring-green-500 focus:border-green-500"
                    />
                  </div>
                  
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                    <div>
                      <label htmlFor="expiryDate" className="block text-sm font-medium text-gray-700 mb-1">
                        Expiry Date *
                      </label>
                      <input
                        type="text"
                        id="expiryDate"
                        name="expiryDate"
                        value={paymentForm.expiryDate}
                        onChange={handlePaymentChange}
                        required
                        placeholder="MM/YY"
                        className="w-full px-4 py-2 border border-gray-300 rounded focus:ring-green-500 focus:border-green-500"
                      />
                    </div>
                    
                    <div>
                      <label htmlFor="cvv" className="block text-sm font-medium text-gray-700 mb-1">
                        CVV *
                      </label>
                      <input
                        type="text"
                        id="cvv"
                        name="cvv"
                        value={paymentForm.cvv}
                        onChange={handlePaymentChange}
                        required
                        placeholder="XXX"
                        className="w-full px-4 py-2 border border-gray-300 rounded focus:ring-green-500 focus:border-green-500"
                      />
                    </div>
                  </div>
                  
                  <div className="flex justify-between">
                    <button
                      type="button"
                      onClick={handleBack}
                      className="bg-gray-200 hover:bg-gray-300 text-gray-800 font-bold py-3 px-6 rounded-lg"
                    >
                      Back
                    </button>
                    <button
                      type="submit"
                      className="bg-green-600 hover:bg-green-700 text-white font-bold py-3 px-6 rounded-lg"
                    >
                      Review Order
                    </button>
                  </div>
                </form>
              </div>
            )}
            
            {/* Order Confirmation */}
            {currentStep === 'confirmation' && (
              <div className="bg-white rounded-lg shadow overflow-hidden">
                <div className="px-6 py-4 border-b border-gray-200 bg-gray-50">
                  <h2 className="font-bold text-lg">Order Review</h2>
                </div>
                
                <div className="p-6">
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-8 mb-8">
                    <div>
                      <h3 className="font-bold text-gray-800 mb-2">Shipping Address</h3>
                      <p className="text-gray-600">
                        {shippingForm.firstName} {shippingForm.lastName}<br />
                        {shippingForm.address}<br />
                        {shippingForm.city}, {shippingForm.state} {shippingForm.zipCode}<br />
                        {shippingForm.country}<br />
                        {shippingForm.phone}
                      </p>
                    </div>
                    
                    <div>
                      <h3 className="font-bold text-gray-800 mb-2">Payment Method</h3>
                      <p className="text-gray-600">
                        Credit Card ending in {paymentForm.cardNumber.slice(-4)}<br />
                        {paymentForm.cardName}<br />
                        Expires: {paymentForm.expiryDate}
                      </p>
                    </div>
                  </div>
                  
                  <h3 className="font-bold text-gray-800 mb-2">Order Items</h3>
                  <div className="border rounded-lg overflow-hidden mb-6">
                    <table className="w-full">
                      <thead>
                        <tr className="bg-gray-50">
                          <th className="px-4 py-2 text-left text-sm font-medium text-gray-700">Product</th>
                          <th className="px-4 py-2 text-center text-sm font-medium text-gray-700">Quantity</th>
                          <th className="px-4 py-2 text-right text-sm font-medium text-gray-700">Price</th>
                        </tr>
                      </thead>
                      <tbody className="divide-y divide-gray-200">
                        {cart.items.map((item) => (
                          <tr key={item.product.id}>
                            <td className="px-4 py-3 text-sm text-gray-900">
                              {item.product.name}
                            </td>
                            <td className="px-4 py-3 text-sm text-gray-900 text-center">
                              {item.quantity}
                            </td>
                            <td className="px-4 py-3 text-sm text-gray-900 text-right">
                              ${(item.product.price * item.quantity).toFixed(2)}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                  
                  <div className="mb-8">
                    <div className="flex justify-between py-2">
                      <span className="text-gray-600">Subtotal</span>
                      <span className="font-medium">${subtotal.toFixed(2)}</span>
                    </div>
                    <div className="flex justify-between py-2">
                      <span className="text-gray-600">Shipping</span>
                      <span className="font-medium">
                        {shipping === 0 ? 'Free' : `$${shipping.toFixed(2)}`}
                      </span>
                    </div>
                    <div className="flex justify-between py-2">
                      <span className="text-gray-600">Tax (7%)</span>
                      <span className="font-medium">${tax.toFixed(2)}</span>
                    </div>
                    <div className="flex justify-between py-2 font-bold text-lg">
                      <span>Total</span>
                      <span>${total.toFixed(2)}</span>
                    </div>
                  </div>
                  
                  <div className="flex justify-between">
                    <button
                      onClick={handleBack}
                      className="bg-gray-200 hover:bg-gray-300 text-gray-800 font-bold py-3 px-6 rounded-lg"
                    >
                      Back
                    </button>
                    
                    <button
                      onClick={handlePlaceOrder}
                      disabled={orderPlaced}
                      className={`bg-green-600 hover:bg-green-700 text-white font-bold py-3 px-6 rounded-lg ${
                        orderPlaced ? 'opacity-50 cursor-not-allowed' : ''
                      }`}
                    >
                      {orderPlaced ? 'Processing Order...' : 'Place Order'}
                    </button>
                  </div>
                  
                  {orderPlaced && (
                    <div className="mt-4 p-4 bg-green-50 text-green-700 rounded-lg">
                      <p className="text-center">
                        Order placed successfully! Redirecting to order confirmation...
                      </p>
                    </div>
                  )}
                </div>
              </div>
            )}
          </div>
          
          {/* Order Summary */}
          <div className="lg:col-span-1">
            <div className="bg-white rounded-lg shadow overflow-hidden sticky top-4">
              <div className="px-6 py-4 border-b border-gray-200 bg-gray-50">
                <h2 className="font-bold text-lg">Order Summary</h2>
              </div>
              
              <div className="p-6">
                <div className="mb-4">
                  <p className="text-gray-600 mb-2">
                    {cart.totalItems} {cart.totalItems === 1 ? 'item' : 'items'} in cart
                  </p>
                  
                  <div className="max-h-64 overflow-y-auto mb-4">
                    {cart.items.map((item) => (
                      <div key={item.product.id} className="flex justify-between items-center py-2 border-b border-gray-100">
                        <div className="flex items-center">
                          <span className="bg-gray-200 text-gray-800 rounded-full w-5 h-5 flex items-center justify-center text-xs mr-2">
                            {item.quantity}
                          </span>
                          <span className="text-sm truncate max-w-[150px]">{item.product.name}</span>
                        </div>
                        <span className="text-sm font-medium">
                          ${(item.product.price * item.quantity).toFixed(2)}
                        </span>
                      </div>
                    ))}
                  </div>
                </div>
                
                <div className="border-t border-gray-200 pt-4">
                  <div className="flex justify-between mb-2">
                    <span className="text-gray-600">Subtotal</span>
                    <span className="font-medium">${subtotal.toFixed(2)}</span>
                  </div>
                  
                  <div className="flex justify-between mb-2">
                    <span className="text-gray-600">Shipping</span>
                    <span className="font-medium">
                      {shipping === 0 ? 'Free' : `$${shipping.toFixed(2)}`}
                    </span>
                  </div>
                  
                  <div className="flex justify-between mb-2">
                    <span className="text-gray-600">Tax (7%)</span>
                    <span className="font-medium">${tax.toFixed(2)}</span>
                  </div>
                  
                  <div className="flex justify-between pt-2 border-t border-gray-200 text-lg font-bold">
                    <span>Total</span>
                    <span>${total.toFixed(2)}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </PageLayout>
  );
}
