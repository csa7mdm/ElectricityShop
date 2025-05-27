'use client';

import { useState } from 'react';
import PageLayout from '../components/layout/PageLayout';

export default function ContactPage() {
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    phone: '',
    subject: '',
    message: '',
  });
  
  const [formStatus, setFormStatus] = useState<'idle' | 'submitting' | 'success' | 'error'>('idle');
  
  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };
  
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    // In a real app, we would send the form data to a backend
    setFormStatus('submitting');
    
    // Simulate API call
    setTimeout(() => {
      setFormStatus('success');
      setFormData({
        name: '',
        email: '',
        phone: '',
        subject: '',
        message: '',
      });
      
      // Reset form status after 5 seconds
      setTimeout(() => {
        setFormStatus('idle');
      }, 5000);
    }, 1500);
  };
  
  return (
    <PageLayout>
      <div className="bg-gray-100 py-12">
        <div className="container mx-auto px-4">
          <h1 className="text-3xl font-bold mb-2 text-center">Contact Us</h1>
          <p className="text-gray-600 mb-8 text-center max-w-2xl mx-auto">
            Have questions about our products or services? Need a custom solution for your home or business?
            Our team is here to help. Fill out the form below and we'll get back to you as soon as possible.
          </p>
          
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8 max-w-6xl mx-auto">
            {/* Contact Information */}
            <div className="lg:col-span-1">
              <div className="bg-white rounded-lg shadow-md p-6">
                <h2 className="text-xl font-bold mb-4">Contact Information</h2>
                
                <div className="mb-6">
                  <h3 className="font-medium text-gray-900 mb-2">Address</h3>
                  <p className="text-gray-600">
                    123 Solar Street<br />
                    Electric City, EC 12345<br />
                    United States
                  </p>
                </div>
                
                <div className="mb-6">
                  <h3 className="font-medium text-gray-900 mb-2">Phone</h3>
                  <p className="text-gray-600">
                    <a href="tel:+11234567890" className="hover:text-green-600">
                      (123) 456-7890
                    </a>
                  </p>
                </div>
                
                <div className="mb-6">
                  <h3 className="font-medium text-gray-900 mb-2">Email</h3>
                  <p className="text-gray-600">
                    <a href="mailto:info@electricityshop.com" className="hover:text-green-600">
                      info@electricityshop.com
                    </a>
                  </p>
                </div>
                
                <div>
                  <h3 className="font-medium text-gray-900 mb-2">Business Hours</h3>
                  <p className="text-gray-600 mb-1">Monday - Friday: 9:00 AM - 6:00 PM</p>
                  <p className="text-gray-600 mb-1">Saturday: 10:00 AM - 4:00 PM</p>
                  <p className="text-gray-600">Sunday: Closed</p>
                </div>
              </div>
            </div>
            
            {/* Contact Form */}
            <div className="lg:col-span-2">
              <div className="bg-white rounded-lg shadow-md p-6">
                <h2 className="text-xl font-bold mb-4">Send Us a Message</h2>
                
                {formStatus === 'success' ? (
                  <div className="bg-green-50 text-green-700 p-4 rounded-lg mb-4">
                    <div className="flex items-center mb-2">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        className="h-5 w-5 mr-2"
                        viewBox="0 0 20 20"
                        fill="currentColor"
                      >
                        <path
                          fillRule="evenodd"
                          d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                          clipRule="evenodd"
                        />
                      </svg>
                      <span className="font-medium">Message Sent Successfully!</span>
                    </div>
                    <p>
                      Thank you for contacting us. We'll get back to you as soon as possible.
                    </p>
                  </div>
                ) : (
                  <form onSubmit={handleSubmit}>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                      <div>
                        <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-1">
                          Your Name *
                        </label>
                        <input
                          type="text"
                          id="name"
                          name="name"
                          value={formData.name}
                          onChange={handleChange}
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
                          value={formData.email}
                          onChange={handleChange}
                          required
                          className="w-full px-4 py-2 border border-gray-300 rounded focus:ring-green-500 focus:border-green-500"
                        />
                      </div>
                    </div>
                    
                    <div className="mb-6">
                      <label htmlFor="phone" className="block text-sm font-medium text-gray-700 mb-1">
                        Phone Number
                      </label>
                      <input
                        type="tel"
                        id="phone"
                        name="phone"
                        value={formData.phone}
                        onChange={handleChange}
                        className="w-full px-4 py-2 border border-gray-300 rounded focus:ring-green-500 focus:border-green-500"
                      />
                    </div>
                    
                    <div className="mb-6">
                      <label htmlFor="subject" className="block text-sm font-medium text-gray-700 mb-1">
                        Subject *
                      </label>
                      <select
                        id="subject"
                        name="subject"
                        value={formData.subject}
                        onChange={handleChange}
                        required
                        className="w-full px-4 py-2 border border-gray-300 rounded focus:ring-green-500 focus:border-green-500"
                      >
                        <option value="">Please select</option>
                        <option value="general">General Inquiry</option>
                        <option value="sales">Sales Question</option>
                        <option value="support">Technical Support</option>
                        <option value="consultation">Free Consultation Request</option>
                        <option value="feedback">Feedback</option>
                      </select>
                    </div>
                    
                    <div className="mb-6">
                      <label htmlFor="message" className="block text-sm font-medium text-gray-700 mb-1">
                        Message *
                      </label>
                      <textarea
                        id="message"
                        name="message"
                        value={formData.message}
                        onChange={handleChange}
                        rows={5}
                        required
                        className="w-full px-4 py-2 border border-gray-300 rounded focus:ring-green-500 focus:border-green-500"
                      ></textarea>
                    </div>
                    
                    <div className="flex items-center mb-6">
                      <input
                        id="consent"
                        name="consent"
                        type="checkbox"
                        required
                        className="h-4 w-4 text-green-600 focus:ring-green-500 border-gray-300 rounded"
                      />
                      <label htmlFor="consent" className="ml-2 block text-sm text-gray-700">
                        I agree to the privacy policy and consent to being contacted regarding my inquiry.
                      </label>
                    </div>
                    
                    <button
                      type="submit"
                      disabled={formStatus === 'submitting'}
                      className={`w-full py-3 px-4 border border-transparent rounded-md shadow-sm text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 ${
                        formStatus === 'submitting' ? 'opacity-75 cursor-not-allowed' : ''
                      }`}
                    >
                      {formStatus === 'submitting' ? 'Sending...' : 'Send Message'}
                    </button>
                  </form>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
      
      {/* Map Section */}
      <div className="h-96 bg-gray-200 relative">
        <div className="absolute inset-0 flex items-center justify-center text-gray-500">
          Map Placeholder - Google Maps would be integrated here
        </div>
      </div>
    </PageLayout>
  );
}
