import 'package:equatable/equatable.dart';
import '../../domain/entities/product.dart';
import '../../domain/repositories/product_repository.dart';

/// Base class for product-related states
abstract class ProductState extends Equatable {
  const ProductState();

  @override
  List<Object?> get props => [];
}

/// Initial state
class ProductInitial extends ProductState {}

/// State for loading products
class ProductsLoading extends ProductState {}

/// State for loading products on next page
class ProductsLoadingMore extends ProductState {
  final List<Product> currentProducts;
  final Pagination<Product> pagination;

  const ProductsLoadingMore({
    required this.currentProducts,
    required this.pagination,
  });

  @override
  List<Object?> get props => [currentProducts, pagination];
}

/// State for successfully loaded products
class ProductsLoaded extends ProductState {
  final List<Product> products;
  final Pagination<Product> pagination;
  final String? category;
  final String? searchTerm;
  final double? minPrice;
  final double? maxPrice;
  final bool? onSaleOnly;

  const ProductsLoaded({
    required this.products,
    required this.pagination,
    this.category,
    this.searchTerm,
    this.minPrice,
    this.maxPrice,
    this.onSaleOnly,
  });

  /// Check if any filters are applied
  bool get hasFilters =>
      category != null ||
      searchTerm != null ||
      minPrice != null ||
      maxPrice != null ||
      (onSaleOnly != null && onSaleOnly!);

  @override
  List<Object?> get props => [
        products,
        pagination,
        category,
        searchTerm,
        minPrice,
        maxPrice,
        onSaleOnly,
      ];

  /// Create a copy with updated filters
  ProductsLoaded copyWithFilters({
    String? category,
    String? searchTerm,
    double? minPrice,
    double? maxPrice,
    bool? onSaleOnly,
  }) {
    return ProductsLoaded(
      products: products,
      pagination: pagination,
      category: category ?? this.category,
      searchTerm: searchTerm ?? this.searchTerm,
      minPrice: minPrice ?? this.minPrice,
      maxPrice: maxPrice ?? this.maxPrice,
      onSaleOnly: onSaleOnly ?? this.onSaleOnly,
    );
  }
}

/// State for loading product details
class ProductDetailLoading extends ProductState {}

/// State for successfully loaded product details
class ProductDetailLoaded extends ProductState {
  final Product product;

  const ProductDetailLoaded({required this.product});

  @override
  List<Object?> get props => [product];
}

/// State for product-related errors
class ProductError extends ProductState {
  final String message;
  final dynamic data;

  const ProductError({
    required this.message,
    this.data,
  });

  @override
  List<Object?> get props => [message, data];
}
