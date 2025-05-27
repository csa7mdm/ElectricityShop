'use client';

import Link from 'next/link';
import { useState } from 'react';
import { useCart } from '../../context/CartContext';

export default function Header() {
  const { cart } = useCart();
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  return (
    <header className="bg-white shadow-md">
      <div className="container mx-auto px-4 py-4">
        <div className="flex justify-between items-center">
          {/* Logo */}
          <Link href="/" className="text-2xl font-bold text-green-600">
            ElectricityShop
          </Link>

          {/* Desktop Navigation */}
          <nav className="hidden md:flex space-x-8">
            <Link href="/" className="text-gray-700 hover:text-green-600">
              Home
            </Link>
            <Link href="/products/solar" className="text-gray-700 hover:text-green-600">
              Solar Panels
            </Link>
            <Link href="/products/battery" className="text-gray-700 hover:text-green-600">
              Batteries
            </Link>
            <Link href="/products/installation" className="text-gray-700 hover:text-green-600">
              Installation
            </Link>
            <Link href="/products/service" className="text-gray-700 hover:text-green-600">
              Services
            </Link>
          </nav>

          {/* Cart Icon */}
          <div className="flex items-center">
            <Link href="/cart" className="relative ml-4 p-2">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                className="h-6 w-6 text-gray-700"
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
              {cart.totalItems > 0 && (
                <span className="absolute top-0 right-0 bg-green-600 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">
                  {cart.totalItems}
                </span>
              )}
            </Link>

            {/* Mobile menu button */}
            <button
              className="md:hidden ml-4"
              onClick={() => setIsMenuOpen(!isMenuOpen)}
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                className="h-6 w-6 text-gray-700"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M4 6h16M4 12h16m-7 6h7"
                />
              </svg>
            </button>
          </div>
        </div>

        {/* Mobile Navigation */}
        {isMenuOpen && (
          <nav className="md:hidden mt-4 space-y-3">
            <Link
              href="/"
              className="block text-gray-700 hover:text-green-600"
              onClick={() => setIsMenuOpen(false)}
            >
              Home
            </Link>
            <Link
              href="/products/solar"
              className="block text-gray-700 hover:text-green-600"
              onClick={() => setIsMenuOpen(false)}
            >
              Solar Panels
            </Link>
            <Link
              href="/products/battery"
              className="block text-gray-700 hover:text-green-600"
              onClick={() => setIsMenuOpen(false)}
            >
              Batteries
            </Link>
            <Link
              href="/products/installation"
              className="block text-gray-700 hover:text-green-600"
              onClick={() => setIsMenuOpen(false)}
            >
              Installation
            </Link>
            <Link
              href="/products/service"
              className="block text-gray-700 hover:text-green-600"
              onClick={() => setIsMenuOpen(false)}
            >
              Services
            </Link>
          </nav>
        )}
      </div>
    </header>
  );
}
