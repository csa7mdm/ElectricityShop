import Link from 'next/link';
import PageLayout from '../components/layout/PageLayout';

export default function AboutPage() {
  return (
    <PageLayout>
      {/* Hero Section */}
      <div className="bg-gray-900 text-white py-16">
        <div className="container mx-auto px-4 text-center">
          <h1 className="text-4xl font-bold mb-4">About ElectricityShop</h1>
          <p className="text-xl mb-8 max-w-3xl mx-auto">
            Powering homes and businesses with sustainable energy solutions since 2015.
          </p>
        </div>
      </div>
      
      {/* Our Story */}
      <section className="py-16">
        <div className="container mx-auto px-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-12 items-center">
            <div>
              <h2 className="text-3xl font-bold mb-6">Our Story</h2>
              <p className="mb-4 text-gray-700">
                Founded in 2015, ElectricityShop began with a simple mission: to make renewable energy accessible and affordable for everyone. Our founder, Jane Smith, saw the challenges many homeowners faced when trying to transition to cleaner energy sources - high costs, complicated installations, and confusing options.
              </p>
              <p className="mb-4 text-gray-700">
                Starting with just a small team of three, we focused on creating simple solar solutions for residential homes. As demand grew, we expanded our product line to include battery storage, professional installation services, and energy consulting.
              </p>
              <p className="text-gray-700">
                Today, we're proud to have helped over 15,000 homes and businesses across the country reduce their carbon footprint and save on energy costs. Our team has grown to over 100 professionals dedicated to powering a sustainable future.
              </p>
            </div>
            <div className="h-96 bg-gray-200 rounded-lg">
              <div className="h-full flex items-center justify-center text-gray-500">
                Company Image Placeholder
              </div>
            </div>
          </div>
        </div>
      </section>
      
      {/* Our Mission */}
      <section className="py-16 bg-green-50">
        <div className="container mx-auto px-4 text-center">
          <h2 className="text-3xl font-bold mb-6">Our Mission</h2>
          <p className="text-xl mb-8 max-w-3xl mx-auto text-gray-700">
            To accelerate the transition to sustainable energy by providing accessible, reliable, and affordable clean energy solutions to homes and businesses around the world.
          </p>
          
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8 max-w-4xl mx-auto">
            <div className="bg-white p-6 rounded-lg shadow-md">
              <div className="bg-green-100 h-16 w-16 rounded-full flex items-center justify-center mx-auto mb-4">
                <svg xmlns="http://www.w3.org/2000/svg" className="h-8 w-8 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
                </svg>
              </div>
              <h3 className="text-xl font-bold mb-2">Quality</h3>
              <p className="text-gray-600">
                We source only the highest quality products and materials, ensuring long-lasting performance and reliability.
              </p>
            </div>
            
            <div className="bg-white p-6 rounded-lg shadow-md">
              <div className="bg-green-100 h-16 w-16 rounded-full flex items-center justify-center mx-auto mb-4">
                <svg xmlns="http://www.w3.org/2000/svg" className="h-8 w-8 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
                </svg>
              </div>
              <h3 className="text-xl font-bold mb-2">Innovation</h3>
              <p className="text-gray-600">
                We continuously research and adopt the latest technologies to provide cutting-edge energy solutions.
              </p>
            </div>
            
            <div className="bg-white p-6 rounded-lg shadow-md">
              <div className="bg-green-100 h-16 w-16 rounded-full flex items-center justify-center mx-auto mb-4">
                <svg xmlns="http://www.w3.org/2000/svg" className="h-8 w-8 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                </svg>
              </div>
              <h3 className="text-xl font-bold mb-2">Customer Focus</h3>
              <p className="text-gray-600">
                We put our customers first, providing personalized solutions and exceptional service every step of the way.
              </p>
            </div>
          </div>
        </div>
      </section>
      
      {/* Our Team */}
      <section className="py-16">
        <div className="container mx-auto px-4">
          <h2 className="text-3xl font-bold mb-6 text-center">Meet Our Team</h2>
          <p className="text-gray-700 mb-12 text-center max-w-3xl mx-auto">
            Our team of experts is dedicated to helping you find the perfect energy solution for your needs.
          </p>
          
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-8">
            {/* Team Member 1 */}
            <div className="bg-white rounded-lg shadow-md overflow-hidden">
              <div className="h-64 bg-gray-200">
                <div className="h-full flex items-center justify-center text-gray-500">
                  Team Member Photo
                </div>
              </div>
              <div className="p-4 text-center">
                <h3 className="font-bold text-lg">Jane Smith</h3>
                <p className="text-green-600 mb-2">Founder & CEO</p>
                <p className="text-gray-600 text-sm mb-4">
                  With over 15 years in renewable energy, Jane leads our company's vision and strategy.
                </p>
              </div>
            </div>
            
            {/* Team Member 2 */}
            <div className="bg-white rounded-lg shadow-md overflow-hidden">
              <div className="h-64 bg-gray-200">
                <div className="h-full flex items-center justify-center text-gray-500">
                  Team Member Photo
                </div>
              </div>
              <div className="p-4 text-center">
                <h3 className="font-bold text-lg">Mike Johnson</h3>
                <p className="text-green-600 mb-2">Technical Director</p>
                <p className="text-gray-600 text-sm mb-4">
                  Mike oversees all technical aspects of our products and installations, ensuring top quality.
                </p>
              </div>
            </div>
            
            {/* Team Member 3 */}
            <div className="bg-white rounded-lg shadow-md overflow-hidden">
              <div className="h-64 bg-gray-200">
                <div className="h-full flex items-center justify-center text-gray-500">
                  Team Member Photo
                </div>
              </div>
              <div className="p-4 text-center">
                <h3 className="font-bold text-lg">Sarah Parker</h3>
                <p className="text-green-600 mb-2">Sales Director</p>
                <p className="text-gray-600 text-sm mb-4">
                  Sarah leads our sales team with a focus on understanding customer needs and providing tailored solutions.
                </p>
              </div>
            </div>
            
            {/* Team Member 4 */}
            <div className="bg-white rounded-lg shadow-md overflow-hidden">
              <div className="h-64 bg-gray-200">
                <div className="h-full flex items-center justify-center text-gray-500">
                  Team Member Photo
                </div>
              </div>
              <div className="p-4 text-center">
                <h3 className="font-bold text-lg">Alex Thompson</h3>
                <p className="text-green-600 mb-2">Lead Engineer</p>
                <p className="text-gray-600 text-sm mb-4">
                  Alex brings innovation to our product development, constantly improving our energy solutions.
                </p>
              </div>
            </div>
          </div>
          
          <div className="text-center mt-12">
            <Link 
              href="/contact" 
              className="bg-green-600 hover:bg-green-700 text-white font-bold py-3 px-6 rounded-lg inline-block"
            >
              Get in Touch
            </Link>
          </div>
        </div>
      </section>
      
      {/* Testimonials */}
      <section className="py-16 bg-gray-50">
        <div className="container mx-auto px-4">
          <h2 className="text-3xl font-bold mb-12 text-center">What Our Customers Say</h2>
          
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            {/* Testimonial 1 */}
            <div className="bg-white p-6 rounded-lg shadow-md">
              <p className="text-gray-600 italic mb-4">
                "The solar panels I purchased from ElectricityShop have reduced my energy bill by 70%. The installation team was professional and efficient. I highly recommend their services!"
              </p>
              <div className="flex items-center">
                <div className="w-10 h-10 bg-gray-300 rounded-full mr-3"></div>
                <div>
                  <p className="font-medium">Robert Davis</p>
                  <p className="text-gray-500 text-sm">Homeowner</p>
                </div>
              </div>
            </div>
            
            {/* Testimonial 2 */}
            <div className="bg-white p-6 rounded-lg shadow-md">
              <p className="text-gray-600 italic mb-4">
                "As a small business owner, I was concerned about the upfront costs of converting to solar energy. The ElectricityShop team provided a clear breakdown of costs and ROI. Two years later, I'm already seeing significant savings!"
              </p>
              <div className="flex items-center">
                <div className="w-10 h-10 bg-gray-300 rounded-full mr-3"></div>
                <div>
                  <p className="font-medium">Maria Rodriguez</p>
                  <p className="text-gray-500 text-sm">Restaurant Owner</p>
                </div>
              </div>
            </div>
            
            {/* Testimonial 3 */}
            <div className="bg-white p-6 rounded-lg shadow-md">
              <p className="text-gray-600 italic mb-4">
                "After experiencing frequent power outages in our area, we decided to invest in a battery backup system. ElectricityShop recommended the perfect solution for our needs. When the next outage came, our home stayed powered while the neighborhood went dark!"
              </p>
              <div className="flex items-center">
                <div className="w-10 h-10 bg-gray-300 rounded-full mr-3"></div>
                <div>
                  <p className="font-medium">James Wilson</p>
                  <p className="text-gray-500 text-sm">Homeowner</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
      
      {/* Call to Action */}
      <section className="py-16 bg-green-600 text-white">
        <div className="container mx-auto px-4 text-center">
          <h2 className="text-3xl font-bold mb-4">Ready to Make the Switch to Sustainable Energy?</h2>
          <p className="max-w-3xl mx-auto mb-8 text-lg">
            Join thousands of satisfied customers who are saving money and reducing their carbon footprint with our solar and battery solutions.
          </p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Link href="/products" className="bg-white text-green-600 hover:bg-gray-100 font-bold py-3 px-6 rounded-lg inline-block">
              Browse Products
            </Link>
            <Link href="/contact" className="bg-transparent hover:bg-white hover:text-green-600 text-white font-bold py-3 px-6 rounded-lg border-2 border-white inline-block">
              Get a Free Quote
            </Link>
          </div>
        </div>
      </section>
    </PageLayout>
  );
}
