import 'package:equatable/equatable.dart';

/// Base class for product-related events
abstract class ProductEvent extends Equatable {
  const ProductEvent();

  @override
  List<Object?> get props => [];
}

/// Event to load a paginated list of products
class LoadProductsEvent extends ProductEvent {
  final int page;
  final int pageSize;
  final String? category;
  final String? searchTerm;
  final double? minPrice;
  final double? maxPrice;
  final bool? onSaleOnly;

  const LoadProductsEvent({
    required this.page,
    required this.pageSize,
    this.category,
    this.searchTerm,
    this.minPrice,
    this.maxPrice,
    this.onSaleOnly,
  });

  @override
  List<Object?> get props => [
        page,
        pageSize,
        category,
        searchTerm,
        minPrice,
        maxPrice,
        onSaleOnly,
      ];
}

/// Event to load a specific product by ID
class LoadProductDetailEvent extends ProductEvent {
  final String id;

  const LoadProductDetailEvent({required this.id});

  @override
  List<Object?> get props => [id];
}

/// Event to apply filters to product list
class ApplyFiltersEvent extends ProductEvent {
  final String? category;
  final String? searchTerm;
  final double? minPrice;
  final double? maxPrice;
  final bool? onSaleOnly;

  const ApplyFiltersEvent({
    this.category,
    this.searchTerm,
    this.minPrice,
    this.maxPrice,
    this.onSaleOnly,
  });

  @override
  List<Object?> get props => [
        category,
        searchTerm,
        minPrice,
        maxPrice,
        onSaleOnly,
      ];
}

/// Event to clear all filters
class ClearFiltersEvent extends ProductEvent {}

/// Event to refresh the product list
class RefreshProductsEvent extends ProductEvent {}
