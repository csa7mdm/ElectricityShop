import 'package:equatable/equatable.dart';

/// Product entity representing a product in the system
class Product extends Equatable {
  /// Unique identifier
  final String id;
  
  /// Product name
  final String name;
  
  /// Detailed description
  final String description;
  
  /// Price in currency
  final double price;
  
  /// Available stock quantity
  final int stockQuantity;
  
  /// Category ID
  final String categoryId;
  
  /// Category name
  final String categoryName;
  
  /// Whether the product is active/available
  final bool isActive;
  
  /// List of image URLs
  final List<String> imageUrls;
  
  /// Average rating (1-5 stars)
  final double? averageRating;
  
  /// Number of reviews
  final int reviewCount;
  
  /// Whether the product is on sale
  final bool onSale;
  
  /// Discount percentage if on sale
  final double? discountPercentage;

  const Product({
    required this.id,
    required this.name,
    required this.description,
    required this.price,
    required this.stockQuantity,
    required this.categoryId,
    required this.categoryName,
    required this.isActive,
    required this.imageUrls,
    this.averageRating,
    required this.reviewCount,
    required this.onSale,
    this.discountPercentage,
  });

  /// Check if the product is in stock
  bool get isInStock => stockQuantity > 0;

  /// Calculate the sale price if the product is on sale
  double get salePrice {
    if (onSale && discountPercentage != null) {
      return price * (1 - discountPercentage! / 100);
    }
    return price;
  }

  /// Get the main image URL or a placeholder if none exists
  String get mainImage {
    if (imageUrls.isNotEmpty) {
      return imageUrls.first;
    }
    return 'https://via.placeholder.com/400x300?text=No+Image';
  }

  @override
  List<Object?> get props => [
        id,
        name,
        description,
        price,
        stockQuantity,
        categoryId,
        categoryName,
        isActive,
        imageUrls,
        averageRating,
        reviewCount,
        onSale,
        discountPercentage,
      ];
}
