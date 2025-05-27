import 'package:flutter_bloc/flutter_bloc.dart';
import '../../domain/repositories/product_repository.dart';
import 'product_event.dart';
import 'product_state.dart';

/// BLoC for handling product-related state
class ProductBloc extends Bloc<ProductEvent, ProductState> {
  final ProductRepository _productRepository;

  ProductBloc(this._productRepository) : super(ProductInitial()) {
    on<LoadProductsEvent>(_onLoadProducts);
    on<LoadProductDetailEvent>(_onLoadProductDetail);
    on<ApplyFiltersEvent>(_onApplyFilters);
    on<ClearFiltersEvent>(_onClearFilters);
    on<RefreshProductsEvent>(_onRefreshProducts);
  }

  /// Handle loading products
  Future<void> _onLoadProducts(
    LoadProductsEvent event,
    Emitter<ProductState> emit,
  ) async {
    // Determine if we're loading more data or initial data
    final isLoadingMore = state is ProductsLoaded;
    
    if (isLoadingMore) {
      emit(ProductsLoadingMore(
        currentProducts: (state as ProductsLoaded).products,
        pagination: (state as ProductsLoaded).pagination,
      ));
    } else {
      emit(ProductsLoading());
    }
    
    // Get current filters if they exist
    String? category;
    String? searchTerm;
    double? minPrice;
    double? maxPrice;
    bool? onSaleOnly;
    
    if (state is ProductsLoaded) {
      final loadedState = state as ProductsLoaded;
      category = event.category ?? loadedState.category;
      searchTerm = event.searchTerm ?? loadedState.searchTerm;
      minPrice = event.minPrice ?? loadedState.minPrice;
      maxPrice = event.maxPrice ?? loadedState.maxPrice;
      onSaleOnly = event.onSaleOnly ?? loadedState.onSaleOnly;
    } else {
      category = event.category;
      searchTerm = event.searchTerm;
      minPrice = event.minPrice;
      maxPrice = event.maxPrice;
      onSaleOnly = event.onSaleOnly;
    }
    
    // Make API request
    final result = await _productRepository.getProducts(
      page: event.page,
      pageSize: event.pageSize,
      category: category,
      searchTerm: searchTerm,
      minPrice: minPrice,
      maxPrice: maxPrice,
      onSaleOnly: onSaleOnly,
    );
    
    result.fold(
      (failure) => emit(ProductError(message: failure.message, data: failure.data)),
      (pagination) {
        // If loading more, combine with existing products
        if (isLoadingMore && event.page > 1) {
          final currentProducts = (state as ProductsLoaded).products;
          final updatedProducts = [...currentProducts, ...pagination.items];
          
          emit(ProductsLoaded(
            products: updatedProducts,
            pagination: pagination,
            category: category,
            searchTerm: searchTerm,
            minPrice: minPrice,
            maxPrice: maxPrice,
            onSaleOnly: onSaleOnly,
          ));
        } else {
          // Initial load or refresh
          emit(ProductsLoaded(
            products: pagination.items,
            pagination: pagination,
            category: category,
            searchTerm: searchTerm,
            minPrice: minPrice,
            maxPrice: maxPrice,
            onSaleOnly: onSaleOnly,
          ));
        }
      },
    );
  }

  /// Handle loading product details
  Future<void> _onLoadProductDetail(
    LoadProductDetailEvent event,
    Emitter<ProductState> emit,
  ) async {
    emit(ProductDetailLoading());
    
    final result = await _productRepository.getProductById(event.id);
    
    result.fold(
      (failure) => emit(ProductError(message: failure.message, data: failure.data)),
      (product) => emit(ProductDetailLoaded(product: product)),
    );
  }

  /// Handle applying filters
  Future<void> _onApplyFilters(
    ApplyFiltersEvent event,
    Emitter<ProductState> emit,
  ) async {
    // If we have loaded state, update filters and reload
    if (state is ProductsLoaded) {
      final currentState = state as ProductsLoaded;
      
      // Apply new filters
      final updatedState = currentState.copyWithFilters(
        category: event.category,
        searchTerm: event.searchTerm,
        minPrice: event.minPrice,
        maxPrice: event.maxPrice,
        onSaleOnly: event.onSaleOnly,
      );
      
      // Load products with new filters
      add(LoadProductsEvent(
        page: 1, // Reset to first page when applying filters
        pageSize: updatedState.pagination.pageSize,
        category: updatedState.category,
        searchTerm: updatedState.searchTerm,
        minPrice: updatedState.minPrice,
        maxPrice: updatedState.maxPrice,
        onSaleOnly: updatedState.onSaleOnly,
      ));
    } else {
      // If no loaded state yet, just load with filters
      add(LoadProductsEvent(
        page: 1,
        pageSize: 10, // Default page size
        category: event.category,
        searchTerm: event.searchTerm,
        minPrice: event.minPrice,
        maxPrice: event.maxPrice,
        onSaleOnly: event.onSaleOnly,
      ));
    }
  }

  /// Handle clearing filters
  void _onClearFilters(
    ClearFiltersEvent event,
    Emitter<ProductState> emit,
  ) {
    if (state is ProductsLoaded) {
      final currentState = state as ProductsLoaded;
      
      // Load products with cleared filters
      add(LoadProductsEvent(
        page: 1, // Reset to first page when clearing filters
        pageSize: currentState.pagination.pageSize,
      ));
    } else {
      // If no loaded state yet, just load without filters
      add(LoadProductsEvent(
        page: 1,
        pageSize: 10, // Default page size
      ));
    }
  }

  /// Handle refreshing products
  Future<void> _onRefreshProducts(
    RefreshProductsEvent event,
    Emitter<ProductState> emit,
  ) async {
    if (state is ProductsLoaded) {
      final currentState = state as ProductsLoaded;
      
      // Reload first page with current filters
      add(LoadProductsEvent(
        page: 1,
        pageSize: currentState.pagination.pageSize,
        category: currentState.category,
        searchTerm: currentState.searchTerm,
        minPrice: currentState.minPrice,
        maxPrice: currentState.maxPrice,
        onSaleOnly: currentState.onSaleOnly,
      ));
    } else {
      // If no loaded state yet, just load first page
      add(LoadProductsEvent(
        page: 1,
        pageSize: 10, // Default page size
      ));
    }
  }
}
