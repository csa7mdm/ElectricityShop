export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  category: 'solar' | 'battery' | 'installation' | 'service';
  image: string;
  rating: number;
  inStock: boolean;
  specifications?: {
    [key: string]: string | number;
  };
}

export const products: Product[] = [
  {
    id: 'solar-panel-basic',
    name: 'Solar Panel - Basic',
    description: 'Standard solar panel for residential use. Easy to install and maintain.',
    price: 299.99,
    category: 'solar',
    image: '/images/solar-panel-basic.jpg',
    rating: 4.2,
    inStock: true,
    specifications: {
      wattage: 300,
      dimensions: '65" x 39" x 1.4"',
      weight: 18.8,
      efficiency: '19.3%',
      warranty: '25 years',
    }
  },
  {
    id: 'solar-panel-premium',
    name: 'Solar Panel - Premium',
    description: 'High-efficiency solar panel with advanced technology for maximum power output.',
    price: 499.99,
    category: 'solar',
    image: '/images/solar-panel-premium.jpg',
    rating: 4.7,
    inStock: true,
    specifications: {
      wattage: 400,
      dimensions: '65" x 39" x 1.2"',
      weight: 19.5,
      efficiency: '22.7%',
      warranty: '30 years',
    }
  },
  {
    id: 'home-battery-system',
    name: 'Home Battery System',
    description: 'Store excess solar energy for use during the night or power outages.',
    price: 1299.99,
    category: 'battery',
    image: '/images/home-battery.jpg',
    rating: 4.8,
    inStock: true,
    specifications: {
      capacity: '13.5kWh',
      dimensions: '45.3" x 29.6" x 5.75"',
      weight: 251.3,
      peakPower: '7kW',
      warranty: '10 years',
    }
  },
  {
    id: 'portable-power-station',
    name: 'Portable Power Station',
    description: 'Compact battery power station for camping, emergencies, or outdoor activities.',
    price: 249.99,
    category: 'battery',
    image: '/images/portable-power.jpg',
    rating: 4.5,
    inStock: true,
    specifications: {
      capacity: '500Wh',
      dimensions: '11.8" x 7.5" x 7.7"',
      weight: 13.6,
      outputs: 'AC, USB-A, USB-C, DC',
      warranty: '2 years',
    }
  },
  {
    id: 'solar-installation-basic',
    name: 'Solar Installation - Basic Package',
    description: 'Professional installation of a basic solar panel system for your home.',
    price: 1999.99,
    category: 'installation',
    image: '/images/installation-basic.jpg',
    rating: 4.6,
    inStock: true,
  },
  {
    id: 'battery-installation',
    name: 'Battery Installation Service',
    description: 'Professional installation of home battery systems with your existing solar setup.',
    price: 699.99,
    category: 'installation',
    image: '/images/battery-installation.jpg',
    rating: 4.4,
    inStock: true,
  },
  {
    id: 'annual-maintenance',
    name: 'Annual Maintenance Plan',
    description: 'Yearly inspection and maintenance of your solar panel system to ensure optimal performance.',
    price: 149.99,
    category: 'service',
    image: '/images/maintenance.jpg',
    rating: 4.3,
    inStock: true,
  },
  {
    id: 'energy-consultation',
    name: 'Energy Consumption Consultation',
    description: 'Expert analysis of your energy usage patterns and recommendations for optimization.',
    price: 99.99,
    category: 'service',
    image: '/images/consultation.jpg',
    rating: 4.1,
    inStock: true,
  }
];

export function getProductById(id: string): Product | undefined {
  return products.find(product => product.id === id);
}

export function getProductsByCategory(category: Product['category']): Product[] {
  return products.filter(product => product.category === category);
}
