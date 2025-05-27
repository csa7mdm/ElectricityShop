import 'package:json_annotation/json_annotation.dart';
import '../../domain/entities/cart_item.dart';

part 'cart_item_model.g.dart';

/// Data model for cart item
@JsonSerializable()
class CartItemModel extends CartItem {
  const CartItemModel({
    required super.id,
    required super.productId,
    required super.name,
    required super.imageUrl,
    required super.price,
    required super.quantity,
    required super.subtotal,
  });

  /// Create model from JSON
  factory CartItemModel.fromJson(Map<String, dynamic> json) => _$CartItemModelFromJson(json);

  /// Convert model to JSON
  Map<String, dynamic> toJson() => _$CartItemModelToJson(this);

  /// Create model from entity
  factory CartItemModel.fromEntity(CartItem entity) {
    return CartItemModel(
      id: entity.id,
      productId: entity.productId,
      name: entity.name,
      imageUrl: entity.imageUrl,
      price: entity.price,
      quantity: entity.quantity,
      subtotal: entity.subtotal,
    );
  }
}
