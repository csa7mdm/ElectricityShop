import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/product.dart';

/// Interface for product repository
abstract class ProductRepository {
  /// Get a paginated list of products
  /// 
  /// [page] is the page number to fetch (starting from 1)
  /// [pageSize] is the number of items per page
  /// [category] is optional category filter
  /// [searchTerm] is optional search term for filtering
  /// [minPrice] is optional minimum price filter
  /// [maxPrice] is optional maximum price filter
  /// [onSaleOnly] is optional filter for products on sale
  Future<Either<Failure, Pagination<Product>>> getProducts({
    required int page,
    required int pageSize,
    String? category,
    String? searchTerm,
    double? minPrice,
    double? maxPrice,
    bool? onSaleOnly,
  });

  /// Get a specific product by ID
  Future<Either<Failure, Product>> getProductById(String id);
}

/// Class for representing pagination information
class Pagination<T> {
  /// List of items for the current page
  final List<T> items;
  
  /// Total number of items across all pages
  final int totalCount;
  
  /// Current page number
  final int pageNumber;
  
  /// Number of items per page
  final int pageSize;
  
  /// Total number of pages
  final int totalPages;
  
  /// Whether there's a previous page available
  final bool hasPrevious;
  
  /// Whether there's a next page available
  final bool hasNext;

  Pagination({
    required this.items,
    required this.totalCount,
    required this.pageNumber,
    required this.pageSize,
    required this.totalPages,
    required this.hasPrevious,
    required this.hasNext,
  });
}
