import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:infinite_scroll_pagination/infinite_scroll_pagination.dart';
import '../../../../core/config/app_config.dart';
import '../../../../shared/widgets/error_view.dart';
import '../../domain/entities/product.dart';
import '../bloc/product_bloc.dart';
import '../bloc/product_event.dart';
import '../bloc/product_state.dart';
import '../widgets/product_card.dart';
import '../widgets/product_filter_drawer.dart';
import 'product_detail_page.dart';

/// Page for displaying a list of products with pagination
class ProductListPage extends StatefulWidget {
  static const String routeName = '/products';
  
  const ProductListPage({super.key});

  @override
  State<ProductListPage> createState() => _ProductListPageState();
}

class _ProductListPageState extends State<ProductListPage> {
  final PagingController<int, Product> _pagingController = 
      PagingController(firstPageKey: AppConfig.initialPageKey);
  
  bool _isGridView = true;
  String? _searchQuery;
  
  @override
  void initState() {
    super.initState();
    _pagingController.addPageRequestListener(_fetchPage);
  }
  
  @override
  void dispose() {
    _pagingController.dispose();
    super.dispose();
  }
  
  void _fetchPage(int pageKey) {
    context.read<ProductBloc>().add(
      LoadProductsEvent(
        page: pageKey,
        pageSize: AppConfig.defaultPageSize,
      ),
    );
  }
  
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: _searchQuery != null
            ? Text('Results for: $_searchQuery')
            : const Text('Products'),
        actions: [
          // Search button
          IconButton(
            icon: const Icon(Icons.search),
            onPressed: () => _showSearchDialog(context),
          ),
          // View toggle (grid/list)
          IconButton(
            icon: Icon(_isGridView ? Icons.view_list : Icons.grid_view),
            onPressed: () {
              setState(() {
                _isGridView = !_isGridView;
              });
            },
          ),
        ],
      ),
      drawer: ProductFilterDrawer(
        onApplyFilters: (category, minPrice, maxPrice, onSaleOnly) {
          context.read<ProductBloc>().add(
            ApplyFiltersEvent(
              category: category,
              searchTerm: _searchQuery,
              minPrice: minPrice,
              maxPrice: maxPrice,
              onSaleOnly: onSaleOnly,
            ),
          );
          _pagingController.refresh();
        },
        onClearFilters: () {
          context.read<ProductBloc>().add(ClearFiltersEvent());
          setState(() {
            _searchQuery = null;
          });
          _pagingController.refresh();
        },
      ),
      body: RefreshIndicator(
        onRefresh: () {
          _pagingController.refresh();
          return Future.value();
        },
        child: BlocListener<ProductBloc, ProductState>(
          listener: (context, state) {
            if (state is ProductsLoaded) {
              final isLastPage = !state.pagination.hasNext;
              if (state.pagination.pageNumber == 1) {
                // First page loaded - clear and set new items
                _pagingController.refresh();
                
                if (isLastPage) {
                  _pagingController.appendLastPage(state.products);
                } else {
                  _pagingController.appendPage(
                    state.products, 
                    state.pagination.pageNumber + 1,
                  );
                }
              } else {
                // Additional page loaded - append items
                if (isLastPage) {
                  _pagingController.appendLastPage(state.products);
                } else {
                  _pagingController.appendPage(
                    state.products, 
                    state.pagination.pageNumber + 1,
                  );
                }
              }
            } else if (state is ProductError) {
              _pagingController.error = state.message;
            }
          },
          child: _buildProductList(),
        ),
      ),
    );
  }
  
  Widget _buildProductList() {
    return _isGridView
        ? PagedGridView<int, Product>(
            pagingController: _pagingController,
            gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
              crossAxisCount: 2,
              childAspectRatio: 0.7,
              crossAxisSpacing: 10,
              mainAxisSpacing: 10,
            ),
            padding: const EdgeInsets.all(16),
            builderDelegate: PagedChildBuilderDelegate<Product>(
              itemBuilder: (context, product, index) => ProductCard(
                product: product,
                onTap: () => _navigateToProductDetail(product.id),
                onAddToCart: () => _addToCart(product),
              ),
              firstPageErrorIndicatorBuilder: (context) => ErrorView(
                message: _pagingController.error.toString(),
                onRetry: () => _pagingController.refresh(),
              ),
              noItemsFoundIndicatorBuilder: (context) => const Center(
                child: Text('No products found'),
              ),
            ),
          )
        : PagedListView<int, Product>(
            pagingController: _pagingController,
            padding: const EdgeInsets.all(16),
            builderDelegate: PagedChildBuilderDelegate<Product>(
              itemBuilder: (context, product, index) => ProductCard(
                product: product,
                onTap: () => _navigateToProductDetail(product.id),
                onAddToCart: () => _addToCart(product),
              ),
              firstPageErrorIndicatorBuilder: (context) => ErrorView(
                message: _pagingController.error.toString(),
                onRetry: () => _pagingController.refresh(),
              ),
              noItemsFoundIndicatorBuilder: (context) => const Center(
                child: Text('No products found'),
              ),
            ),
          );
  }
  
  void _navigateToProductDetail(String id) {
    Navigator.of(context).pushNamed(
      ProductDetailPage.routeName,
      arguments: id,
    );
  }
  
  void _addToCart(Product product) {
    // This will be implemented when we add cart functionality
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('${product.name} added to cart'),
        action: SnackBarAction(
          label: 'VIEW CART',
          onPressed: () {
            // Navigate to cart page
            Navigator.of(context).pushNamed('/cart');
          },
        ),
      ),
    );
  }
  
  void _showSearchDialog(BuildContext context) {
    final TextEditingController controller = TextEditingController(text: _searchQuery);
    
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Search Products'),
        content: TextField(
          controller: controller,
          decoration: const InputDecoration(
            hintText: 'Enter search term...',
            prefixIcon: Icon(Icons.search),
          ),
          autofocus: true,
          onSubmitted: (value) {
            _applySearch(value);
            Navigator.of(context).pop();
          },
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('CANCEL'),
          ),
          TextButton(
            onPressed: () {
              _applySearch(controller.text);
              Navigator.of(context).pop();
            },
            child: const Text('SEARCH'),
          ),
        ],
      ),
    );
  }
  
  void _applySearch(String? query) {
    if (query == null || query.isEmpty) {
      // Clear search
      setState(() {
        _searchQuery = null;
      });
      
      // Apply filters without search term
      context.read<ProductBloc>().add(
        ApplyFiltersEvent(
          searchTerm: null,
        ),
      );
    } else {
      // Set search term
      setState(() {
        _searchQuery = query;
      });
      
      // Apply filters with search term
      context.read<ProductBloc>().add(
        ApplyFiltersEvent(
          searchTerm: query,
        ),
      );
    }
    
    // Refresh paging controller
    _pagingController.refresh();
  }
}
