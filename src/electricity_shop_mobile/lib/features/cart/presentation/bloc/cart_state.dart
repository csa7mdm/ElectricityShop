import 'package:equatable/equatable.dart';
import '../../domain/entities/cart.dart';

/// Base class for cart states
abstract class CartState extends Equatable {
  const CartState();

  @override
  List<Object?> get props => [];
}

/// Initial cart state
class CartInitial extends CartState {
  const CartInitial();
}

/// Loading cart state
class CartLoading extends CartState {
  const CartLoading();
}

/// Loaded cart state
class CartLoaded extends CartState {
  final Cart cart;
  final bool isItemAdded;
  final bool isItemRemoved;
  final bool isItemUpdated;
  final bool isCartCleared;

  const CartLoaded({
    required this.cart,
    this.isItemAdded = false,
    this.isItemRemoved = false,
    this.isItemUpdated = false,
    this.isCartCleared = false,
  });

  @override
  List<Object?> get props => [cart, isItemAdded, isItemRemoved, isItemUpdated, isCartCleared];

  /// Create a copy with updated flags
  CartLoaded copyWith({
    Cart? cart,
    bool? isItemAdded,
    bool? isItemRemoved,
    bool? isItemUpdated,
    bool? isCartCleared,
  }) {
    return CartLoaded(
      cart: cart ?? this.cart,
      isItemAdded: isItemAdded ?? false,
      isItemRemoved: isItemRemoved ?? false,
      isItemUpdated: isItemUpdated ?? false,
      isCartCleared: isCartCleared ?? false,
    );
  }
}

/// Error state
class CartError extends CartState {
  final String message;
  final dynamic data;

  const CartError({
    required this.message,
    this.data,
  });

  @override
  List<Object?> get props => [message, data];
}
