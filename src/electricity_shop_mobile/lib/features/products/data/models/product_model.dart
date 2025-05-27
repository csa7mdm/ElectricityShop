import 'package:json_annotation/json_annotation.dart';
import '../../domain/entities/product.dart';

part 'product_model.g.dart';

/// Product model for data transfer and serialization
@JsonSerializable()
class ProductModel extends Product {
  const ProductModel({
    required String id,
    required String name,
    required String description,
    required double price,
    required int stockQuantity,
    required String categoryId,
    required String categoryName,
    required bool isActive,
    required List<String> imageUrls,
    double? averageRating,
    required int reviewCount,
    required bool onSale,
    double? discountPercentage,
  }) : super(
          id: id,
          name: name,
          description: description,
          price: price,
          stockQuantity: stockQuantity,
          categoryId: categoryId,
          categoryName: categoryName,
          isActive: isActive,
          imageUrls: imageUrls,
          averageRating: averageRating,
          reviewCount: reviewCount,
          onSale: onSale,
          discountPercentage: discountPercentage,
        );

  /// Creates a [ProductModel] from JSON map
  factory ProductModel.fromJson(Map<String, dynamic> json) => _$ProductModelFromJson(json);

  /// Converts [ProductModel] to JSON map
  Map<String, dynamic> toJson() => _$ProductModelToJson(this);

  /// Creates a copy of [ProductModel] with specified fields replaced
  ProductModel copyWith({
    String? id,
    String? name,
    String? description,
    double? price,
    int? stockQuantity,
    String? categoryId,
    String? categoryName,
    bool? isActive,
    List<String>? imageUrls,
    double? averageRating,
    int? reviewCount,
    bool? onSale,
    double? discountPercentage,
  }) {
    return ProductModel(
      id: id ?? this.id,
      name: name ?? this.name,
      description: description ?? this.description,
      price: price ?? this.price,
      stockQuantity: stockQuantity ?? this.stockQuantity,
      categoryId: categoryId ?? this.categoryId,
      categoryName: categoryName ?? this.categoryName,
      isActive: isActive ?? this.isActive,
      imageUrls: imageUrls ?? this.imageUrls,
      averageRating: averageRating ?? this.averageRating,
      reviewCount: reviewCount ?? this.reviewCount,
      onSale: onSale ?? this.onSale,
      discountPercentage: discountPercentage ?? this.discountPercentage,
    );
  }
}
