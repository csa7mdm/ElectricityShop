import 'package:equatable/equatable.dart';
import '../../../cart/domain/entities/cart_item.dart';
import 'address.dart';
import 'payment_method.dart';

/// Order status enum
enum OrderStatus {
  pending,
  processing,
  shipped,
  delivered,
  cancelled,
  refunded,
}

/// Entity class for an order
class Order extends Equatable {
  final String id;
  final String userId;
  final List<CartItem> items;
  final Address shippingAddress;
  final Address? billingAddress;
  final PaymentMethod paymentMethod;
  final double subtotal;
  final double tax;
  final double shippingCost;
  final double total;
  final OrderStatus status;
  final DateTime createdAt;
  final DateTime? updatedAt;

  const Order({
    required this.id,
    required this.userId,
    required this.items,
    required this.shippingAddress,
    this.billingAddress,
    required this.paymentMethod,
    required this.subtotal,
    required this.tax,
    required this.shippingCost,
    required this.total,
    required this.status,
    required this.createdAt,
    this.updatedAt,
  });

  /// Create a copy of this Order with provided parameters
  Order copyWith({
    String? id,
    String? userId,
    List<CartItem>? items,
    Address? shippingAddress,
    Address? billingAddress,
    PaymentMethod? paymentMethod,
    double? subtotal,
    double? tax,
    double? shippingCost,
    double? total,
    OrderStatus? status,
    DateTime? createdAt,
    DateTime? updatedAt,
  }) {
    return Order(
      id: id ?? this.id,
      userId: userId ?? this.userId,
      items: items ?? this.items,
      shippingAddress: shippingAddress ?? this.shippingAddress,
      billingAddress: billingAddress ?? this.billingAddress,
      paymentMethod: paymentMethod ?? this.paymentMethod,
      subtotal: subtotal ?? this.subtotal,
      tax: tax ?? this.tax,
      shippingCost: shippingCost ?? this.shippingCost,
      total: total ?? this.total,
      status: status ?? this.status,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  /// Get the total number of items in the order
  int get itemCount => items.fold<int>(0, (sum, item) => sum + item.quantity);

  /// Get status text for display
  String get statusText {
    switch (status) {
      case OrderStatus.pending:
        return 'Pending';
      case OrderStatus.processing:
        return 'Processing';
      case OrderStatus.shipped:
        return 'Shipped';
      case OrderStatus.delivered:
        return 'Delivered';
      case OrderStatus.cancelled:
        return 'Cancelled';
      case OrderStatus.refunded:
        return 'Refunded';
      default:
        return 'Unknown';
    }
  }

  @override
  List<Object?> get props => [
    id,
    userId,
    items,
    shippingAddress,
    billingAddress,
    paymentMethod,
    subtotal,
    tax,
    shippingCost,
    total,
    status,
    createdAt,
    updatedAt,
  ];
}
