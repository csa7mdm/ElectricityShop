import 'package:equatable/equatable.dart';
import 'cart_item.dart';

/// Entity class representing a user's shopping cart
class Cart extends Equatable {
  final List<CartItem> items;
  final double subtotal;
  final double tax;
  final double total;

  const Cart({
    required this.items,
    required this.subtotal,
    required this.tax,
    required this.total,
  });

  /// Create an empty cart
  factory Cart.empty() {
    return const Cart(
      items: [],
      subtotal: 0,
      tax: 0,
      total: 0,
    );
  }

  /// Create a copy of this Cart with provided parameters
  Cart copyWith({
    List<CartItem>? items,
    double? subtotal,
    double? tax,
    double? total,
  }) {
    return Cart(
      items: items ?? this.items,
      subtotal: subtotal ?? this.subtotal,
      tax: tax ?? this.tax,
      total: total ?? this.total,
    );
  }

  /// Calculate cart totals based on items
  Cart recalculate() {
    final newSubtotal = items.fold<double>(
      0,
      (sum, item) => sum + (item.price * item.quantity),
    );
    final newTax = newSubtotal * 0.10; // Assuming 10% tax
    final newTotal = newSubtotal + newTax;

    return Cart(
      items: items,
      subtotal: newSubtotal,
      tax: newTax,
      total: newTotal,
    );
  }

  /// Check if cart is empty
  bool get isEmpty => items.isEmpty;

  /// Get total number of items in cart
  int get itemCount => items.fold<int>(0, (sum, item) => sum + item.quantity);

  @override
  List<Object?> get props => [items, subtotal, tax, total];

  @override
  String toString() => 'Cart(items: ${items.length}, subtotal: $subtotal, tax: $tax, total: $total)';
}
