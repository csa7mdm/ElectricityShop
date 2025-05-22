import 'package:flutter_bloc/flutter_bloc.dart';
import '../../domain/repositories/cart_repository.dart';
import 'cart_event.dart';
import 'cart_state.dart';

/// BLoC for handling cart operations
class CartBloc extends Bloc<CartEvent, CartState> {
  final CartRepository _cartRepository;

  CartBloc(this._cartRepository) : super(const CartInitial()) {
    on<LoadCartEvent>(_onLoadCart);
    on<AddToCartEvent>(_onAddToCart);
    on<UpdateCartItemEvent>(_onUpdateCartItem);
    on<RemoveFromCartEvent>(_onRemoveFromCart);
    on<ClearCartEvent>(_onClearCart);
  }

  /// Handle loading the cart
  Future<void> _onLoadCart(
    LoadCartEvent event,
    Emitter<CartState> emit,
  ) async {
    emit(const CartLoading());
    
    final result = await _cartRepository.getCart();
    
    result.fold(
      (failure) => emit(CartError(message: failure.message, data: failure.data)),
      (cart) => emit(CartLoaded(cart: cart)),
    );
  }

  /// Handle adding an item to the cart
  Future<void> _onAddToCart(
    AddToCartEvent event,
    Emitter<CartState> emit,
  ) async {
    emit(const CartLoading());
    
    final result = await _cartRepository.addToCart(
      productId: event.productId,
      name: event.name,
      imageUrl: event.imageUrl,
      price: event.price,
      quantity: event.quantity,
    );
    
    result.fold(
      (failure) => emit(CartError(message: failure.message, data: failure.data)),
      (cart) => emit(CartLoaded(cart: cart, isItemAdded: true)),
    );
  }

  /// Handle updating an item in the cart
  Future<void> _onUpdateCartItem(
    UpdateCartItemEvent event,
    Emitter<CartState> emit,
  ) async {
    emit(const CartLoading());
    
    final result = await _cartRepository.updateCartItemQuantity(
      itemId: event.itemId,
      quantity: event.quantity,
    );
    
    result.fold(
      (failure) => emit(CartError(message: failure.message, data: failure.data)),
      (cart) => emit(CartLoaded(cart: cart, isItemUpdated: true)),
    );
  }

  /// Handle removing an item from the cart
  Future<void> _onRemoveFromCart(
    RemoveFromCartEvent event,
    Emitter<CartState> emit,
  ) async {
    emit(const CartLoading());
    
    final result = await _cartRepository.removeFromCart(
      itemId: event.itemId,
    );
    
    result.fold(
      (failure) => emit(CartError(message: failure.message, data: failure.data)),
      (cart) => emit(CartLoaded(cart: cart, isItemRemoved: true)),
    );
  }

  /// Handle clearing the cart
  Future<void> _onClearCart(
    ClearCartEvent event,
    Emitter<CartState> emit,
  ) async {
    emit(const CartLoading());
    
    final result = await _cartRepository.clearCart();
    
    result.fold(
      (failure) => emit(CartError(message: failure.message, data: failure.data)),
      (cart) => emit(CartLoaded(cart: cart, isCartCleared: true)),
    );
  }
}
