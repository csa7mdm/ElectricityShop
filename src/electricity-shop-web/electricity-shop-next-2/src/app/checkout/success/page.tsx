'use client';

import { useEffect } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import PageLayout from '../../components/layout/PageLayout';
import { useCart } from '../../context/CartContext';

export default function OrderSuccessPage() {
  const router = useRouter();
  const { cart } = useCart();
  
  // If someone tries to access this page directly without going through checkout, redirect them to home
  useEffect(() => {
    if (!localStorage.getItem('orderCompleted')) {
      router.push('/');
    } else {
      // Clear the flag after 5 minutes
      setTimeout(() => {
        localStorage.removeItem('orderCompleted');
      }, 5 * 60 * 1000);
    }
  }, [router]);
  
  // Set a flag in localStorage when the page loads
  useEffect(() => {
    localStorage.setItem('orderCompleted', 'true');
  }, []);
  
  // Generate order number
  const orderNumber = Math.floor(100000 + Math.random() * 900000);
  
  return (
    <PageLayout>
      <div className="container mx-auto px-4 py-16 text-center">
        <div className="bg-white rounded-lg shadow-lg p-8 max-w-2xl mx-auto">
          <div className="bg-green-100 text-green-800 w-20 h-20 rounded-full mx-auto flex items-center justify-center mb-6">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className="h-10 w-10"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M5 13l4 4L19 7"
              />
            </svg>
          </div>
          
          <h1 className="text-3xl font-bold mb-4">Order Confirmed!</h1>
          
          <p className="text-gray-600 mb-6">
            Thank you for your purchase. Your order has been received and is now being processed.
          </p>
          
          <div className="bg-gray-50 p-4 rounded-lg mb-6">
            <p className="text-lg font-medium">Order #{orderNumber}</p>
            <p className="text-gray-600">
              A confirmation email has been sent to your email address.
            </p>
          </div>
          
          <div className="mb-8">
            <h2 className="text-xl font-bold mb-4">What's Next?</h2>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div className="bg-blue-50 p-4 rounded-lg">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  className="h-8 w-8 text-blue-500 mx-auto mb-2"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12"
                  />
                </svg>
                <p className="text-sm">
                  You'll receive order updates via email
                </p>
              </div>
              
              <div className="bg-green-50 p-4 rounded-lg">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  className="h-8 w-8 text-green-500 mx-auto mb-2"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M17 8l4 4m0 0l-4 4m4-4H3"
                  />
                </svg>
                <p className="text-sm">
                  Our team will contact you to schedule installation
                </p>
              </div>
              
              <div className="bg-purple-50 p-4 rounded-lg">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  className="h-8 w-8 text-purple-500 mx-auto mb-2"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M8 7h12m0 0l-4-4m4 4l-4 4m-6 4H4m0 0l4 4m-4-4l4-4"
                  />
                </svg>
                <p className="text-sm">
                  Your products will be shipped within 2-3 business days
                </p>
              </div>
            </div>
          </div>
          
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Link
              href="/"
              className="bg-green-600 hover:bg-green-700 text-white font-bold py-3 px-6 rounded-lg"
            >
              Back to Home
            </Link>
            
            <Link
              href="/contact"
              className="bg-gray-200 hover:bg-gray-300 text-gray-800 font-bold py-3 px-6 rounded-lg"
            >
              Contact Support
            </Link>
          </div>
        </div>
      </div>
    </PageLayout>
  );
}
