'use client';

import { useState } from 'react';
import Link from 'next/link';
import { Product } from '../../lib/products';
import { useCart } from '../../context/CartContext';

interface ProductCardProps {
  product: Product;
}

export default function ProductCard({ product }: ProductCardProps) {
  const { addToCart } = useCart();
  const [isAddingToCart, setIsAddingToCart] = useState(false);

  const handleAddToCart = () => {
    setIsAddingToCart(true);
    addToCart(product);
    
    // Show feedback for a brief moment
    setTimeout(() => {
      setIsAddingToCart(false);
    }, 1000);
  };

  return (
    <div className="bg-white rounded-lg shadow-md overflow-hidden transition-transform hover:shadow-lg">
      <div className="h-48 bg-gray-200 relative">
        {/* Placeholder for product image */}
        <div className="absolute inset-0 flex items-center justify-center text-gray-500">
          Product Image
        </div>
      </div>
      
      <div className="p-4">
        <div className="flex justify-between items-start mb-2">
          <h3 className="font-bold text-lg">{product.name}</h3>
          <div className="flex items-center">
            <svg 
              xmlns="http://www.w3.org/2000/svg" 
              className="h-5 w-5 text-yellow-500" 
              viewBox="0 0 20 20" 
              fill="currentColor"
            >
              <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
            </svg>
            <span className="ml-1 text-sm text-gray-600">{product.rating}</span>
          </div>
        </div>
        
        <p className="text-gray-600 mb-4 text-sm line-clamp-2">{product.description}</p>
        
        <div className="mb-4">
          <span className="text-green-600 font-bold text-xl">${product.price.toFixed(2)}</span>
          {!product.inStock && (
            <span className="text-red-600 text-sm ml-2">Out of Stock</span>
          )}
        </div>
        
        <div className="flex space-x-2">
          <Link 
            href={`/products/${product.category}/${product.id}`}
            className="py-2 px-4 text-sm bg-blue-50 text-blue-600 rounded hover:bg-blue-100 flex-grow text-center"
          >
            Details
          </Link>
          
          <button
            onClick={handleAddToCart}
            disabled={!product.inStock || isAddingToCart}
            className={`py-2 px-4 text-sm rounded flex-grow ${
              !product.inStock
                ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                : isAddingToCart
                ? 'bg-green-700 text-white'
                : 'bg-green-600 text-white hover:bg-green-700'
            }`}
          >
            {isAddingToCart ? 'Added!' : 'Add to Cart'}
          </button>
        </div>
      </div>
    </div>
  );
}
