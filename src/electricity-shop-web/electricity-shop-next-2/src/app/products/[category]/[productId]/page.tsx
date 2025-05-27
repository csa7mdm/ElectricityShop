'use client';

import { useState } from 'react';
import { useParams, notFound } from 'next/navigation';
import Link from 'next/link';
import { getProductById } from '../../../lib/products';
import PageLayout from '../../../components/layout/PageLayout';
import { useCart } from '../../../context/CartContext';

export default function ProductDetailPage() {
  const params = useParams();
  const { productId } = params;
  const product = getProductById(productId as string);
  
  const [quantity, setQuantity] = useState(1);
  const { addToCart } = useCart();
  const [addingToCart, setAddingToCart] = useState(false);
  
  if (!product) {
    notFound();
  }
  
  const handleAddToCart = () => {
    setAddingToCart(true);
    
    // Add the product to cart the specified number of times
    for (let i = 0; i < quantity; i++) {
      addToCart(product);
    }
    
    setTimeout(() => {
      setAddingToCart(false);
    }, 1500);
  };
  
  return (
    <PageLayout>
      <div className="container mx-auto px-4 py-8">
        <div className="mb-4">
          <Link href={`/products/${product.category}`} className="text-blue-600 hover:underline">
            ← Back to {product.category}
          </Link>
        </div>
        
        <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
          {/* Product Image */}
          <div className="bg-gray-100 rounded-lg overflow-hidden flex items-center justify-center h-96">
            <div className="text-gray-500">Product Image</div>
          </div>
          
          {/* Product Info */}
          <div>
            <h1 className="text-3xl font-bold mb-2">{product.name}</h1>
            
            <div className="flex items-center mb-4">
              <div className="flex text-yellow-500">
                {Array.from({ length: 5 }).map((_, i) => (
                  <svg 
                    key={i}
                    xmlns="http://www.w3.org/2000/svg" 
                    className={`h-5 w-5 ${i < Math.floor(product.rating) ? 'text-yellow-400' : 'text-gray-300'}`}
                    viewBox="0 0 20 20" 
                    fill="currentColor"
                  >
                    <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                  </svg>
                ))}
                <span className="ml-2 text-gray-600">({product.rating})</span>
              </div>
            </div>
            
            <div className="text-2xl font-bold text-green-600 mb-4">
              ${product.price.toFixed(2)}
            </div>
            
            <div className="mb-6">
              <p className="text-gray-700">{product.description}</p>
            </div>
            
            {/* Specifications */}
            {product.specifications && (
              <div className="mb-6">
                <h2 className="text-xl font-bold mb-3">Specifications</h2>
                <div className="bg-gray-50 p-4 rounded-lg">
                  <ul className="divide-y divide-gray-200">
                    {Object.entries(product.specifications).map(([key, value]) => (
                      <li key={key} className="py-2 flex">
                        <span className="font-medium text-gray-600 w-1/3 capitalize">
                          {key.replace(/([A-Z])/g, ' $1').trim()}
                        </span>
                        <span className="text-gray-900 w-2/3">{value}</span>
                      </li>
                    ))}
                  </ul>
                </div>
              </div>
            )}
            
            {/* Add to Cart */}
            {product.inStock ? (
              <div className="mb-6">
                <div className="flex items-center mb-4">
                  <label htmlFor="quantity" className="mr-4 font-medium">
                    Quantity:
                  </label>
                  <div className="flex items-center">
                    <button
                      onClick={() => setQuantity(Math.max(1, quantity - 1))}
                      className="bg-gray-200 px-3 py-1 rounded-l"
                    >
                      -
                    </button>
                    <input
                      type="number"
                      id="quantity"
                      value={quantity}
                      onChange={(e) => setQuantity(Math.max(1, parseInt(e.target.value) || 1))}
                      min="1"
                      className="w-12 text-center border-y border-gray-200 py-1"
                    />
                    <button
                      onClick={() => setQuantity(quantity + 1)}
                      className="bg-gray-200 px-3 py-1 rounded-r"
                    >
                      +
                    </button>
                  </div>
                </div>
                
                <button
                  onClick={handleAddToCart}
                  disabled={addingToCart}
                  className={`w-full py-3 px-4 rounded-lg font-bold ${
                    addingToCart
                      ? 'bg-green-700 text-white'
                      : 'bg-green-600 hover:bg-green-700 text-white'
                  }`}
                >
                  {addingToCart ? 'Added to Cart!' : 'Add to Cart'}
                </button>
              </div>
            ) : (
              <div className="mb-6">
                <p className="text-red-600 font-bold">Out of Stock</p>
                <button
                  className="w-full py-3 px-4 rounded-lg font-bold bg-gray-300 text-gray-600 cursor-not-allowed mt-2"
                  disabled
                >
                  Add to Cart
                </button>
              </div>
            )}
            
            {/* Delivery Information */}
            <div className="border-t border-gray-200 pt-4">
              <h3 className="font-bold mb-2">Delivery Information</h3>
              <p className="text-gray-600 text-sm">
                Free shipping on orders over $999. Standard delivery in 3-5 business days.
              </p>
            </div>
          </div>
        </div>
      </div>
    </PageLayout>
  );
}
