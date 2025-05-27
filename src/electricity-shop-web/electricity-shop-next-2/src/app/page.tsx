import Image from "next/image";
import Link from "next/link";
import PageLayout from "./components/layout/PageLayout";
import { products } from "./lib/products";

export default function Home() {
  // Get featured products (first 4)
  const featuredProducts = products.slice(0, 4);

  return (
    <PageLayout>
      {/* Hero Section */}
      <section className="relative bg-gray-900 text-white">
        {/* Placeholder for a background image */}
        <div className="absolute inset-0 bg-gradient-to-r from-green-800 to-blue-900 opacity-80"></div>
        
        <div className="container mx-auto px-4 py-24 relative z-10">
          <div className="max-w-2xl">
            <h1 className="text-4xl md:text-5xl font-bold mb-6">
              Power Your Home with Sustainable Energy Solutions
            </h1>
            <p className="text-xl mb-8">
              High-quality solar panels, batteries, and professional installation services.
              Start saving on electricity bills today.
            </p>
            <div className="flex flex-col sm:flex-row gap-4">
              <Link 
                href="/products/solar" 
                className="bg-green-600 hover:bg-green-700 text-white font-bold py-3 px-6 rounded-lg text-center"
              >
                Shop Solar Panels
              </Link>
              <Link 
                href="/contact" 
                className="bg-transparent hover:bg-white hover:text-gray-900 text-white font-bold py-3 px-6 rounded-lg border-2 border-white text-center"
              >
                Get a Free Quote
              </Link>
            </div>
          </div>
        </div>
      </section>

      {/* Featured Products */}
      <section className="py-16 bg-gray-50">
        <div className="container mx-auto px-4">
          <h2 className="text-3xl font-bold mb-12 text-center">Featured Products</h2>
          
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
            {featuredProducts.map((product) => (
              <div key={product.id} className="bg-white rounded-lg shadow-md overflow-hidden">
                <div className="h-48 bg-gray-200 relative">
                  {/* We're using placeholders for now */}
                  <div className="absolute inset-0 flex items-center justify-center text-gray-500">
                    Product Image
                  </div>
                </div>
                <div className="p-4">
                  <h3 className="font-bold text-lg mb-2">{product.name}</h3>
                  <p className="text-gray-600 mb-4 line-clamp-2">{product.description}</p>
                  <div className="flex items-center justify-between">
                    <span className="text-green-600 font-bold">${product.price.toFixed(2)}</span>
                    <Link 
                      href={`/products/${product.category}/${product.id}`}
                      className="text-blue-600 hover:text-blue-800"
                    >
                      View Details
                    </Link>
                  </div>
                </div>
              </div>
            ))}
          </div>
          
          <div className="mt-12 text-center">
            <Link 
              href="/products" 
              className="inline-block bg-green-600 hover:bg-green-700 text-white font-bold py-3 px-6 rounded-lg"
            >
              View All Products
            </Link>
          </div>
        </div>
      </section>

      {/* Benefits */}
      <section className="py-16">
        <div className="container mx-auto px-4">
          <h2 className="text-3xl font-bold mb-12 text-center">Why Choose Us</h2>
          
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <div className="text-center p-6">
              <div className="bg-green-100 h-16 w-16 rounded-full flex items-center justify-center mx-auto mb-4">
                <svg xmlns="http://www.w3.org/2000/svg" className="h-8 w-8 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
                </svg>
              </div>
              <h3 className="text-xl font-bold mb-2">Energy Efficiency</h3>
              <p className="text-gray-600">
                Our products help you reduce energy consumption and lower your electricity bills significantly.
              </p>
            </div>
            
            <div className="text-center p-6">
              <div className="bg-green-100 h-16 w-16 rounded-full flex items-center justify-center mx-auto mb-4">
                <svg xmlns="http://www.w3.org/2000/svg" className="h-8 w-8 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              <h3 className="text-xl font-bold mb-2">Cost Savings</h3>
              <p className="text-gray-600">
                Investing in solar energy provides long-term savings with payback periods as short as 5-7 years.
              </p>
            </div>
            
            <div className="text-center p-6">
              <div className="bg-green-100 h-16 w-16 rounded-full flex items-center justify-center mx-auto mb-4">
                <svg xmlns="http://www.w3.org/2000/svg" className="h-8 w-8 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 11.5V14m0-2.5v-6a1.5 1.5 0 113 0m-3 6a1.5 1.5 0 00-3 0v2a7.5 7.5 0 0015 0v-5a1.5 1.5 0 00-3 0m-6-3V11m0-5.5v-1a1.5 1.5 0 013 0v1m0 0V11m0-5.5a1.5 1.5 0 013 0v3m0 0V11" />
                </svg>
              </div>
              <h3 className="text-xl font-bold mb-2">Expert Installation</h3>
              <p className="text-gray-600">
                Our team of certified professionals ensures proper installation and optimal performance.
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* Call to Action */}
      <section className="py-16 bg-green-600 text-white">
        <div className="container mx-auto px-4 text-center">
          <h2 className="text-3xl font-bold mb-6">Ready to Make the Switch to Solar?</h2>
          <p className="text-xl mb-8 max-w-3xl mx-auto">
            Contact our team today for a free consultation and find the perfect energy solution for your home.
          </p>
          <Link 
            href="/contact" 
            className="inline-block bg-white text-green-600 hover:bg-gray-100 font-bold py-3 px-8 rounded-lg text-lg"
          >
            Get Started
          </Link>
        </div>
      </section>
    </PageLayout>
  );
}
