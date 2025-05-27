import 'package:flutter/material.dart';
import '../../../../core/utils/formatters.dart';
import '../../domain/entities/product.dart';

class ProductDetailBottomBar extends StatelessWidget {
  final Product product;
  final int quantity;
  final VoidCallback onAddToCart;

  const ProductDetailBottomBar({
    super.key,
    required this.product,
    required this.quantity,
    required this.onAddToCart,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.1),
            spreadRadius: 1,
            blurRadius: 5,
            offset: const Offset(0, -2),
          ),
        ],
      ),
      child: Row(
        children: [
          // Price display
          Expanded(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'Total Price',
                  style: TextStyle(
                    fontSize: 12,
                    color: Colors.grey,
                  ),
                ),
                Text(
                  AppFormatters.formatCurrency(
                    product.onSale
                        ? product.salePrice * quantity
                        : product.price * quantity,
                  ),
                  style: const TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ],
            ),
          ),
          
          // Add to cart button
          Expanded(
            child: ElevatedButton.icon(
              onPressed: product.isInStock ? onAddToCart : null,
              icon: const Icon(Icons.shopping_cart),
              label: const Text('Add to Cart'),
              style: ElevatedButton.styleFrom(
                padding: const EdgeInsets.symmetric(vertical: 12),
                backgroundColor: product.isInStock 
                    ? Theme.of(context).primaryColor 
                    : Colors.grey,
              ),
            ),
          ),
        ],
      ),
    );
  }
}
