import 'package:equatable/equatable.dart';

/// Base class for cart events
abstract class CartEvent extends Equatable {
  const CartEvent();

  @override
  List<Object?> get props => [];
}

/// Event to load the cart
class LoadCartEvent extends CartEvent {
  const LoadCartEvent();
}

/// Event to add an item to the cart
class AddToCartEvent extends CartEvent {
  final String productId;
  final String name;
  final String imageUrl;
  final double price;
  final int quantity;

  const AddToCartEvent({
    required this.productId,
    required this.name,
    required this.imageUrl,
    required this.price,
    required this.quantity,
  });

  @override
  List<Object?> get props => [productId, quantity];
}

/// Event to update the quantity of an item in the cart
class UpdateCartItemEvent extends CartEvent {
  final String itemId;
  final int quantity;

  const UpdateCartItemEvent({
    required this.itemId,
    required this.quantity,
  });

  @override
  List<Object?> get props => [itemId, quantity];
}

/// Event to remove an item from the cart
class RemoveFromCartEvent extends CartEvent {
  final String itemId;

  const RemoveFromCartEvent({
    required this.itemId,
  });

  @override
  List<Object?> get props => [itemId];
}

/// Event to clear the entire cart
class ClearCartEvent extends CartEvent {
  const ClearCartEvent();
}
