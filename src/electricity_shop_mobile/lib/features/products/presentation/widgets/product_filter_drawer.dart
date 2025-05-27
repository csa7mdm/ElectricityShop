import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import '../../../../core/utils/formatters.dart';

/// Drawer widget for product filtering
class ProductFilterDrawer extends StatefulWidget {
  final Function(
    String? category,
    double? minPrice,
    double? maxPrice,
    bool? onSaleOnly,
  ) onApplyFilters;
  final VoidCallback onClearFilters;
  
  const ProductFilterDrawer({
    super.key,
    required this.onApplyFilters,
    required this.onClearFilters,
  });

  @override
  State<ProductFilterDrawer> createState() => _ProductFilterDrawerState();
}

class _ProductFilterDrawerState extends State<ProductFilterDrawer> {
  final _formKey = GlobalKey<FormBuilderState>();
  
  // Dummy category data (in a real app, fetch from API)
  final List<String> _categories = [
    'All Categories',
    'Lighting',
    'Cables',
    'Switches',
    'Plugs & Sockets',
    'Circuit Breakers',
    'Tools',
    'Home Automation',
    'Smart Devices',
  ];
  
  String _selectedCategory = 'All Categories';
  RangeValues _priceRange = const RangeValues(0, 1000);
  bool _onSaleOnly = false;
  
  @override
  Widget build(BuildContext context) {
    return Drawer(
      child: FormBuilder(
        key: _formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            // Drawer header
            DrawerHeader(
              decoration: BoxDecoration(
                color: Theme.of(context).primaryColor,
              ),
              child: const Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Filter Products',
                    style: TextStyle(
                      color: Colors.white,
                      fontSize: 24,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  SizedBox(height: 8),
                  Text(
                    'Find exactly what you\'re looking for',
                    style: TextStyle(
                      color: Colors.white70,
                    ),
                  ),
                ],
              ),
            ),
            
            // Filter options
            Expanded(
              child: ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  // Category filter
                  const Text(
                    'Category',
                    style: TextStyle(
                      fontWeight: FontWeight.bold,
                      fontSize: 16,
                    ),
                  ),
                  const SizedBox(height: 8),
                  FormBuilderDropdown<String>(
                    name: 'category',
                    initialValue: _selectedCategory,
                    decoration: const InputDecoration(
                      border: OutlineInputBorder(),
                      contentPadding: EdgeInsets.symmetric(
                        horizontal: 16,
                        vertical: 12,
                      ),
                    ),
                    items: _categories
                        .map((category) => DropdownMenuItem(
                              value: category,
                              child: Text(category),
                            ))
                        .toList(),
                    onChanged: (value) {
                      if (value != null) {
                        setState(() {
                          _selectedCategory = value;
                        });
                      }
                    },
                  ),
                  
                  const SizedBox(height: 24),
                  
                  // Price range filter
                  const Text(
                    'Price Range',
                    style: TextStyle(
                      fontWeight: FontWeight.bold,
                      fontSize: 16,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(AppFormatters.formatCurrency(_priceRange.start)),
                      Text(AppFormatters.formatCurrency(_priceRange.end)),
                    ],
                  ),
                  RangeSlider(
                    values: _priceRange,
                    min: 0,
                    max: 1000,
                    divisions: 20,
                    labels: RangeLabels(
                      AppFormatters.formatCurrency(_priceRange.start),
                      AppFormatters.formatCurrency(_priceRange.end),
                    ),
                    onChanged: (values) {
                      setState(() {
                        _priceRange = values;
                      });
                    },
                  ),
                  
                  const SizedBox(height: 24),
                  
                  // On sale only filter
                  FormBuilderSwitch(
                    name: 'onSaleOnly',
                    title: const Text(
                      'On Sale Only',
                      style: TextStyle(
                        fontWeight: FontWeight.bold,
                        fontSize: 16,
                      ),
                    ),
                    initialValue: _onSaleOnly,
                    onChanged: (value) {
                      setState(() {
                        _onSaleOnly = value ?? false;
                      });
                    },
                    activeColor: Theme.of(context).primaryColor,
                  ),
                ],
              ),
            ),
            
            // Action buttons
            Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  ElevatedButton(
                    onPressed: _applyFilters,
                    style: ElevatedButton.styleFrom(
                      padding: const EdgeInsets.symmetric(vertical: 16),
                    ),
                    child: const Text('APPLY FILTERS'),
                  ),
                  const SizedBox(height: 8),
                  OutlinedButton(
                    onPressed: _clearFilters,
                    style: OutlinedButton.styleFrom(
                      padding: const EdgeInsets.symmetric(vertical: 16),
                    ),
                    child: const Text('CLEAR FILTERS'),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
  
  void _applyFilters() {
    if (_formKey.currentState?.saveAndValidate() ?? false) {
      // Get values from form
      String? category = _selectedCategory == 'All Categories' ? null : _selectedCategory;
      double? minPrice = _priceRange.start;
      double? maxPrice = _priceRange.end;
      
      // Call the callback with filter values
      widget.onApplyFilters(
        category,
        minPrice,
        maxPrice,
        _onSaleOnly ? true : null,
      );
      
      // Close the drawer
      Navigator.of(context).pop();
    }
  }
  
  void _clearFilters() {
    // Reset form values
    setState(() {
      _selectedCategory = 'All Categories';
      _priceRange = const RangeValues(0, 1000);
      _onSaleOnly = false;
    });
    
    // Call the clear filters callback
    widget.onClearFilters();
    
    // Close the drawer
    Navigator.of(context).pop();
  }
}
