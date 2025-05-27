import React, { useEffect, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  Container,
  Grid,
  Typography,
  Box,
  Breadcrumbs,
  Link as MuiLink,
  Alert,
  AlertTitle,
} from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';
import { NavigateNext as NavigateNextIcon } from '@mui/icons-material';

import ProductList from '../components/features/products/ProductList';
import ProductFilter from '../components/features/products/ProductFilter';
import LoadingSpinner from '../components/common/LoadingSpinner';
import { useProduct } from '../hooks/useProduct';
import { ProductFilterParams } from '../types';

const ProductListPage: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { products, categories, isLoading, error, totalItems, currentPage, totalPages, fetchProducts, fetchCategories } = useProduct();
  
  // Parse query parameters
  const queryParams = new URLSearchParams(location.search);
  const searchParam = queryParams.get('search') || '';
  const categoryParam = queryParams.get('category') || '';
  const pageParam = parseInt(queryParams.get('page') || '1');
  const minPriceParam = parseFloat(queryParams.get('minPrice') || '0');
  const maxPriceParam = parseFloat(queryParams.get('maxPrice') || '10000');
  const sortByParam = queryParams.get('sortBy') || '';
  const sortDirectionParam = (queryParams.get('sortDirection') as 'asc' | 'desc') || 'asc';
  
  // State for filter values
  const [filters, setFilters] = useState<ProductFilterParams>({
    page: pageParam,
    limit: 9,
    search: searchParam,
    category: categoryParam,
    minPrice: minPriceParam,
    maxPrice: maxPriceParam,
    sortBy: sortByParam,
    sortDirection: sortDirectionParam,
  });
  
  // State for price range
  const [priceRange, setPriceRange] = useState<{ min: number; max: number }>({
    min: 0,
    max: 10000,
  });
  
  // Fetch categories on mount
  useEffect(() => {
    fetchCategories();
  }, [fetchCategories]);
  
  // Fetch products when filters change
  useEffect(() => {
    fetchProducts(filters);
    
    // Update URL with filters
    const newUrl = new URLSearchParams();
    if (filters.search) newUrl.set('search', filters.search);
    if (filters.category) newUrl.set('category', filters.category);
    if (filters.page !== 1) newUrl.set('page', filters.page.toString());
    if (filters.minPrice !== priceRange.min) newUrl.set('minPrice', filters.minPrice!.toString());
    if (filters.maxPrice !== priceRange.max) newUrl.set('maxPrice', filters.maxPrice!.toString());
    if (filters.sortBy) newUrl.set('sortBy', filters.sortBy);
    if (filters.sortDirection !== 'asc') newUrl.set('sortDirection', filters.sortDirection!);
    
    navigate(`${location.pathname}?${newUrl.toString()}`);
  }, [fetchProducts, filters, location.pathname, navigate, priceRange.min, priceRange.max]);
  
  // Handle filter changes
  const handleFilterChange = (newFilters: Partial<ProductFilterParams>) => {
    setFilters({
      ...filters,
      ...newFilters,
      page: 1, // Reset to first page when filters change
    });
  };
  
  // Handle pagination
  const handlePageChange = (page: number) => {
    setFilters({
      ...filters,
      page,
    });
    
    // Scroll to top on page change
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };
  
  // Clear all filters
  const handleClearFilters = () => {
    setFilters({
      page: 1,
      limit: 9,
      search: '',
      category: '',
      minPrice: priceRange.min,
      maxPrice: priceRange.max,
      sortBy: '',
      sortDirection: 'asc',
    });
  };
  
  return (
    <Container maxWidth="lg">
      <LoadingSpinner isOpen={isLoading && !products.length} message="Loading products..." />
      
      <Box mb={4}>
        <Breadcrumbs separator={<NavigateNextIcon fontSize="small" />}>
          <MuiLink component={RouterLink} to="/" color="inherit">
            Home
          </MuiLink>
          <Typography color="text.primary">Products</Typography>
          {categoryParam && (
            <Typography color="text.primary">{categoryParam}</Typography>
          )}
        </Breadcrumbs>
      </Box>
      
      <Box mb={4}>
        <Typography variant="h4" component="h1" gutterBottom>
          {searchParam
            ? `Search Results for "${searchParam}"`
            : categoryParam
            ? `${categoryParam} Products`
            : 'All Products'}
        </Typography>
        
        {totalItems > 0 && (
          <Typography variant="body1" color="text.secondary">
            Showing {Math.min((currentPage - 1) * filters.limit + 1, totalItems)} - {Math.min(currentPage * filters.limit, totalItems)} of {totalItems} products
          </Typography>
        )}
      </Box>
      
      {error && (
        <Alert severity="error" sx={{ mb: 4 }}>
          <AlertTitle>Error</AlertTitle>
          {error}
        </Alert>
      )}
      
      <Grid container spacing={4}>
        <Grid item xs={12} md={3}>
          <ProductFilter
            categories={categories}
            minPrice={priceRange.min}
            maxPrice={priceRange.max}
            onFilterChange={handleFilterChange}
            loading={isLoading && !categories.length}
            activeFilters={{
              category: filters.category,
              minPrice: filters.minPrice,
              maxPrice: filters.maxPrice,
              sortBy: filters.sortBy,
              sortDirection: filters.sortDirection,
            }}
            onClearFilters={handleClearFilters}
          />
        </Grid>
        
        <Grid item xs={12} md={9}>
          <ProductList
            products={products}
            loading={isLoading}
            totalPages={totalPages}
            currentPage={currentPage}
            onPageChange={handlePageChange}
          />
        </Grid>
      </Grid>
    </Container>
  );
};

export default ProductListPage;
