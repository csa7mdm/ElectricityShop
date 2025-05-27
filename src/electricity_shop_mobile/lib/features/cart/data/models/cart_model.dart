import 'package:json_annotation/json_annotation.dart';
import '../../domain/entities/cart.dart';
import 'cart_item_model.dart';

part 'cart_model.g.dart';

/// Data model for cart
@JsonSerializable()
class CartModel extends Cart {
  const CartModel({
    required List<CartItemModel> items,
    required double subtotal,
    required double tax,
    required double total,
  }) : super(
          items: items,
          subtotal: subtotal,
          tax: tax,
          total: total,
        );

  /// Create empty cart model
  factory CartModel.empty() {
    return const CartModel(
      items: [],
      subtotal: 0,
      tax: 0,
      total: 0,
    );
  }

  /// Create model from JSON
  factory CartModel.fromJson(Map<String, dynamic> json) => _$CartModelFromJson(json);

  /// Convert model to JSON
  Map<String, dynamic> toJson() => _$CartModelToJson(this);

  /// Create model from entity
  factory CartModel.fromEntity(Cart entity) {
    final items = entity.items.map((item) => CartItemModel.fromEntity(item)).toList();
    
    return CartModel(
      items: items,
      subtotal: entity.subtotal,
      tax: entity.tax,
      total: entity.total,
    );
  }
}
