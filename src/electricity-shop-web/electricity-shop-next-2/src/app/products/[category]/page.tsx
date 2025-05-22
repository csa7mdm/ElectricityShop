import { Metadata } from 'next';
import { getProductsByCategory } from '../../lib/products';
import PageLayout from '../../components/layout/PageLayout';
import ProductGrid from '../../components/products/ProductGrid';
import { notFound } from 'next/navigation';

export const dynamicParams = true;

type Props = {
  params: { category: string };
};

export function generateMetadata({ params }: Props): Metadata {
  const category = params.category;
  
  const validCategories = ['solar', 'battery', 'installation', 'service'];
  if (!validCategories.includes(category)) {
    return {
      title: 'Category Not Found',
    };
  }
  
  const categoryNames = {
    solar: 'Solar Panels',
    battery: 'Battery Systems',
    installation: 'Installation Services',
    service: 'Support Services',
  };
  
  return {
    title: `${categoryNames[category as keyof typeof categoryNames]} | Electricity Shop`,
    description: `Browse our selection of ${categoryNames[category as keyof typeof categoryNames]} products.`,
  };
}

export default function CategoryPage({ params }: Props) {
  const { category } = params;
  
  const validCategories = ['solar', 'battery', 'installation', 'service'];
  if (!validCategories.includes(category)) {
    notFound();
  }
  
  const categoryNames = {
    solar: 'Solar Panels',
    battery: 'Battery Systems',
    installation: 'Installation Services',
    service: 'Support Services',
  };
  
  const products = getProductsByCategory(category as any);
  
  return (
    <PageLayout>
      <div className="bg-gray-50 py-8">
        <div className="container mx-auto px-4">
          <h1 className="text-3xl font-bold mb-2">
            {categoryNames[category as keyof typeof categoryNames]}
          </h1>
          <p className="text-gray-600 mb-6">
            Browse our selection of high-quality {categoryNames[category as keyof typeof categoryNames].toLowerCase()}.
          </p>
        </div>
      </div>
      
      <ProductGrid 
        products={products} 
      />
    </PageLayout>
  );
}
