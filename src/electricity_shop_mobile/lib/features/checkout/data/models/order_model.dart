import 'package:json_annotation/json_annotation.dart';
import '../../../cart/data/models/cart_item_model.dart';
import '../../domain/entities/order.dart';
import 'address_model.dart';
import 'payment_method_model.dart';

part 'order_model.g.dart';

/// Data model for an order
@JsonSerializable()
class OrderModel extends Order {
  const OrderModel({
    required super.id,
    required super.userId,
    required List<CartItemModel> super.items,
    required AddressModel super.shippingAddress,
    AddressModel? super.billingAddress,
    required PaymentMethodModel super.paymentMethod,
    required super.subtotal,
    required super.tax,
    required super.shippingCost,
    required super.total,
    required super.status,
    required super.createdAt,
    super.updatedAt,
  });

  /// Create model from JSON
  factory OrderModel.fromJson(Map<String, dynamic> json) => _$OrderModelFromJson(json);

  /// Convert model to JSON
  Map<String, dynamic> toJson() => _$OrderModelToJson(this);

  /// Create model from entity
  factory OrderModel.fromEntity(Order entity) {
    return OrderModel(
      id: entity.id,
      userId: entity.userId,
      items: entity.items.map((item) => CartItemModel.fromEntity(item)).toList(),
      shippingAddress: AddressModel.fromEntity(entity.shippingAddress),
      billingAddress: entity.billingAddress != null
          ? AddressModel.fromEntity(entity.billingAddress!)
          : null,
      paymentMethod: PaymentMethodModel.fromEntity(entity.paymentMethod),
      subtotal: entity.subtotal,
      tax: entity.tax,
      shippingCost: entity.shippingCost,
      total: entity.total,
      status: entity.status,
      createdAt: entity.createdAt,
      updatedAt: entity.updatedAt,
    );
  }
}
