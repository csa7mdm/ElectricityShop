import React, { useState, useEffect } from 'react';
import {
  Paper,
  Typography,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Slider,
  Box,
  Button,
  Chip,
  FormGroup,
  FormControlLabel,
  Checkbox,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Divider,
  SelectChangeEvent,
  Skeleton,
} from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { formatPrice } from '../../../utils/formatters';

interface ProductFilterProps {
  categories: string[];
  minPrice: number;
  maxPrice: number;
  onFilterChange: (filters: {
    category?: string;
    minPrice?: number;
    maxPrice?: number;
    sortBy?: string;
    sortDirection?: 'asc' | 'desc';
  }) => void;
  loading: boolean;
  activeFilters?: {
    category?: string;
    minPrice?: number;
    maxPrice?: number;
    sortBy?: string;
    sortDirection?: 'asc' | 'desc';
  };
  onClearFilters: () => void;
}

const ProductFilter: React.FC<ProductFilterProps> = ({
  categories,
  minPrice,
  maxPrice,
  onFilterChange,
  loading,
  activeFilters = {},
  onClearFilters,
}) => {
  // State for filter values
  const [category, setCategory] = useState<string>('');
  const [priceRange, setPriceRange] = useState<number[]>([minPrice, maxPrice]);
  const [sortBy, setSortBy] = useState<string>('');
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('asc');
  const [expanded, setExpanded] = useState<boolean>(true);

  // Update local state when active filters change
  useEffect(() => {
    setCategory(activeFilters.category || '');
    setPriceRange([
      activeFilters.minPrice || minPrice,
      activeFilters.maxPrice || maxPrice,
    ]);
    setSortBy(activeFilters.sortBy || '');
    setSortDirection(activeFilters.sortDirection || 'asc');
  }, [activeFilters, minPrice, maxPrice]);

  // Update filter when price range changes
  const handlePriceChange = (_: Event, newValue: number | number[]) => {
    if (Array.isArray(newValue)) {
      setPriceRange(newValue);
    }
  };

  // Apply price range filter on slider change complete
  const handlePriceChangeCommitted = (_: Event | React.SyntheticEvent, newValue: number | number[]) => {
    if (Array.isArray(newValue)) {
      onFilterChange({
        ...activeFilters,
        minPrice: newValue[0],
        maxPrice: newValue[1],
      });
    }
  };

  // Update filter when category changes
  const handleCategoryChange = (event: SelectChangeEvent<string>) => {
    const value = event.target.value;
    setCategory(value);
    onFilterChange({
      ...activeFilters,
      category: value === 'all' ? undefined : value,
    });
  };

  // Update filter when sort option changes
  const handleSortChange = (event: SelectChangeEvent<string>) => {
    const value = event.target.value;
    setSortBy(value);
    onFilterChange({
      ...activeFilters,
      sortBy: value === '' ? undefined : value,
    });
  };

  // Update filter when sort direction changes
  const handleSortDirectionChange = (event: SelectChangeEvent<'asc' | 'desc'>) => {
    const value = event.target.value as 'asc' | 'desc';
    setSortDirection(value);
    onFilterChange({
      ...activeFilters,
      sortDirection: value,
    });
  };

  // Handle filter reset
  const handleClearFilters = () => {
    setCategory('');
    setPriceRange([minPrice, maxPrice]);
    setSortBy('');
    setSortDirection('asc');
    onClearFilters();
  };

  // Loading skeleton
  if (loading) {
    return (
      <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
        <Skeleton variant="text" height={40} width="60%" />
        <Skeleton variant="text" height={30} width="100%" sx={{ my: 1 }} />
        <Skeleton variant="rectangular" height={80} width="100%" sx={{ my: 2 }} />
        <Skeleton variant="rectangular" height={80} width="100%" sx={{ my: 2 }} />
        <Skeleton variant="text" height={40} width="40%" />
      </Paper>
    );
  }

  return (
    <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
      <Accordion
        expanded={expanded}
        onChange={() => setExpanded(!expanded)}
        disableGutters
        elevation={0}
        sx={{ '&:before': { display: 'none' } }}
      >
        <AccordionSummary
          expandIcon={<ExpandMoreIcon />}
          sx={{ p: 0, minHeight: 'auto' }}
        >
          <Typography variant="h6" fontWeight="bold">
            Filters
          </Typography>
        </AccordionSummary>
        <AccordionDetails sx={{ p: 0, mt: 2 }}>
          {Object.keys(activeFilters).length > 0 && (
            <Box sx={{ mb: 2, display: 'flex', flexWrap: 'wrap', gap: 1 }}>
              {activeFilters.category && (
                <Chip
                  label={`Category: ${activeFilters.category}`}
                  onDelete={() =>
                    onFilterChange({
                      ...activeFilters,
                      category: undefined,
                    })
                  }
                  color="primary"
                  variant="outlined"
                />
              )}
              {(activeFilters.minPrice !== minPrice ||
                activeFilters.maxPrice !== maxPrice) && (
                <Chip
                  label={`Price: ${formatPrice(activeFilters.minPrice || minPrice)} - ${formatPrice(
                    activeFilters.maxPrice || maxPrice
                  )}`}
                  onDelete={() =>
                    onFilterChange({
                      ...activeFilters,
                      minPrice: undefined,
                      maxPrice: undefined,
                    })
                  }
                  color="primary"
                  variant="outlined"
                />
              )}
              {activeFilters.sortBy && (
                <Chip
                  label={`Sort by: ${activeFilters.sortBy} (${
                    activeFilters.sortDirection === 'asc' ? 'Ascending' : 'Descending'
                  })`}
                  onDelete={() =>
                    onFilterChange({
                      ...activeFilters,
                      sortBy: undefined,
                      sortDirection: 'asc',
                    })
                  }
                  color="primary"
                  variant="outlined"
                />
              )}
              <Button
                variant="outlined"
                color="secondary"
                size="small"
                onClick={handleClearFilters}
                sx={{ ml: 'auto' }}
              >
                Clear All
              </Button>
            </Box>
          )}

          <Divider sx={{ my: 2 }} />

          <Box sx={{ mb: 3 }}>
            <Typography variant="subtitle1" gutterBottom>
              Category
            </Typography>
            <FormControl fullWidth size="small">
              <InputLabel id="category-select-label">Select Category</InputLabel>
              <Select
                labelId="category-select-label"
                id="category-select"
                value={category}
                label="Select Category"
                onChange={handleCategoryChange}
              >
                <MenuItem value="">All Categories</MenuItem>
                {categories.map((cat) => (
                  <MenuItem key={cat} value={cat}>
                    {cat}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Box>

          <Box sx={{ mb: 3 }}>
            <Typography variant="subtitle1" gutterBottom>
              Price Range
            </Typography>
            <Box sx={{ px: 1 }}>
              <Slider
                value={priceRange}
                onChange={handlePriceChange}
                onChangeCommitted={handlePriceChangeCommitted}
                valueLabelDisplay="auto"
                min={minPrice}
                max={maxPrice}
                valueLabelFormat={(value) => formatPrice(value)}
              />
              <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 1 }}>
                <Typography variant="body2" color="text.secondary">
                  {formatPrice(priceRange[0])}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  {formatPrice(priceRange[1])}
                </Typography>
              </Box>
            </Box>
          </Box>

          <Box sx={{ mb: 3 }}>
            <Typography variant="subtitle1" gutterBottom>
              Sort By
            </Typography>
            <FormControl fullWidth size="small" sx={{ mb: 2 }}>
              <InputLabel id="sort-select-label">Sort By</InputLabel>
              <Select
                labelId="sort-select-label"
                id="sort-select"
                value={sortBy}
                label="Sort By"
                onChange={handleSortChange}
              >
                <MenuItem value="">Default</MenuItem>
                <MenuItem value="price">Price</MenuItem>
                <MenuItem value="name">Name</MenuItem>
                <MenuItem value="rating">Rating</MenuItem>
                <MenuItem value="createdAt">Newest</MenuItem>
              </Select>
            </FormControl>
            {sortBy && (
              <FormControl fullWidth size="small">
                <InputLabel id="sort-direction-label">Direction</InputLabel>
                <Select
                  labelId="sort-direction-label"
                  id="sort-direction"
                  value={sortDirection}
                  label="Direction"
                  onChange={handleSortDirectionChange}
                >
                  <MenuItem value="asc">Ascending</MenuItem>
                  <MenuItem value="desc">Descending</MenuItem>
                </Select>
              </FormControl>
            )}
          </Box>
        </AccordionDetails>
      </Accordion>
    </Paper>
  );
};

export default ProductFilter;
