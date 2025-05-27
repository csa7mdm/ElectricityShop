import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../bloc/cart_bloc.dart';
import '../bloc/cart_event.dart';
import '../bloc/cart_state.dart';
import '../widgets/cart_item_card.dart';
import '../widgets/cart_summary.dart';
import '../widgets/empty_cart.dart';
import '../../../../app/routes.dart';

class CartPage extends StatefulWidget {
  static const String routeName = AppRoutes.cart;

  const CartPage({super.key});

  @override
  State<CartPage> createState() => _CartPageState();
}

class _CartPageState extends State<CartPage> {
  @override
  void initState() {
    super.initState();
    context.read<CartBloc>().add(const LoadCartEvent());
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Shopping Cart'),
        actions: [
          BlocBuilder<CartBloc, CartState>(
            builder: (context, state) {
              if (state is CartLoaded && state.cart.items.isNotEmpty) {
                return IconButton(
                  icon: const Icon(Icons.delete_sweep),
                  tooltip: 'Clear cart',
                  onPressed: () {
                    _showClearCartConfirmation(context);
                  },
                );
              }
              return const SizedBox.shrink();
            },
          ),
        ],
      ),
      body: BlocConsumer<CartBloc, CartState>(
        listener: (context, state) {
          if (state is CartLoaded) {
            // Show feedback based on cart actions
            if (state.isItemAdded) {
              _showSnackBar(context, 'Item added to cart');
            } else if (state.isItemRemoved) {
              _showSnackBar(context, 'Item removed from cart');
            } else if (state.isCartCleared) {
              _showSnackBar(context, 'Cart has been cleared');
            }
          } else if (state is CartError) {
            _showSnackBar(context, 'Error: ${state.message}', isError: true);
          }
        },
        builder: (context, state) {
          if (state is CartLoading && state is! CartLoaded) {
            return const Center(
              child: CircularProgressIndicator(),
            );
          } else if (state is CartLoaded) {
            if (state.cart.items.isEmpty) {
              return EmptyCart(
                onContinueShopping: () {
                  Navigator.pushReplacementNamed(context, AppRoutes.products);
                },
              );
            }

            return Column(
              children: [
                Expanded(
                  child: ListView.builder(
                    itemCount: state.cart.items.length,
                    padding: const EdgeInsets.only(bottom: 16, top: 8),
                    itemBuilder: (context, index) {
                      final item = state.cart.items[index];
                      return CartItemCard(
                        item: item,
                        onRemove: () {
                          context.read<CartBloc>().add(
                                RemoveFromCartEvent(itemId: item.id),
                              );
                        },
                        onUpdateQuantity: (quantity) {
                          context.read<CartBloc>().add(
                                UpdateCartItemEvent(
                                  itemId: item.id,
                                  quantity: quantity,
                                ),
                              );
                        },
                      );
                    },
                  ),
                ),
                CartSummary(
                  cart: state.cart,
                  onCheckout: () {
                    _proceedToCheckout(context);
                  },
                ),
              ],
            );
          } else if (state is CartError) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Icon(
                    Icons.error_outline,
                    size: 48,
                    color: Colors.red,
                  ),
                  const SizedBox(height: 16),
                  Text(
                    'Error: ${state.message}',
                    style: const TextStyle(fontSize: 16),
                    textAlign: TextAlign.center,
                  ),
                  const SizedBox(height: 24),
                  ElevatedButton(
                    onPressed: () {
                      context.read<CartBloc>().add(const LoadCartEvent());
                    },
                    child: const Text('Retry'),
                  ),
                ],
              ),
            );
          }
          
          return const SizedBox.shrink();
        },
      ),
      bottomNavigationBar: BlocBuilder<CartBloc, CartState>(
        builder: (context, state) {
          if (state is CartLoaded && state.cart.items.isNotEmpty) {
            return BottomAppBar(
              child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                child: Row(
                  children: [
                    Column(
                      mainAxisSize: MainAxisSize.min,
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text(
                          'Total Amount:',
                          style: TextStyle(
                            fontSize: 14,
                          ),
                        ),
                        Text(
                          '\$${state.cart.total.toStringAsFixed(2)}',
                          style: TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.bold,
                            color: Colors.blue[800],
                          ),
                        ),
                      ],
                    ),
                    const Spacer(),
                    ElevatedButton(
                      onPressed: () {
                        _proceedToCheckout(context);
                      },
                      style: ElevatedButton.styleFrom(
                        padding: const EdgeInsets.symmetric(
                          horizontal: 24,
                          vertical: 12,
                        ),
                        backgroundColor: Colors.blue[700],
                        foregroundColor: Colors.white,
                      ),
                      child: const Text('CHECKOUT'),
                    ),
                  ],
                ),
              ),
            );
          }
          return const SizedBox.shrink();
        },
      ),
    );
  }

  void _showSnackBar(BuildContext context, String message, {bool isError = false}) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: isError ? Colors.red : Colors.blue[700],
        duration: const Duration(seconds: 2),
      ),
    );
  }

  void _showClearCartConfirmation(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Clear Cart'),
        content: const Text('Are you sure you want to remove all items from your cart?'),
        actions: [
          TextButton(
            onPressed: () {
              Navigator.of(context).pop();
            },
            child: const Text('CANCEL'),
          ),
          TextButton(
            onPressed: () {
              Navigator.of(context).pop();
              context.read<CartBloc>().add(const ClearCartEvent());
            },
            style: TextButton.styleFrom(
              foregroundColor: Colors.red,
            ),
            child: const Text('CLEAR'),
          ),
        ],
      ),
    );
  }

  void _proceedToCheckout(BuildContext context) {
    // Navigate to checkout
    Navigator.of(context).pushNamed(AppRoutes.checkout);
  }
}
