import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../shared/widgets/error_view.dart';
import '../../domain/entities/product.dart';
import '../bloc/product_bloc.dart';
import '../bloc/product_event.dart';
import '../bloc/product_state.dart';
import '../../../../features/cart/presentation/bloc/cart_bloc.dart';
import '../../../../features/cart/presentation/bloc/cart_event.dart';
import '../../../../features/cart/presentation/pages/cart_page.dart';
import '../widgets/product_detail_loading.dart';
import '../widgets/product_image_carousel.dart';
import '../widgets/quantity_selector.dart';
import 'product_detail_page_bottom.dart';

/// Page for displaying product details
class ProductDetailPage extends StatefulWidget {
  static const String routeName = '/product-detail';
  
  final String productId;
  
  const ProductDetailPage({
    super.key,
    required this.productId,
  });

  @override
  State<ProductDetailPage> createState() => _ProductDetailPageState();
}

class _ProductDetailPageState extends State<ProductDetailPage> {
  int _quantity = 1;
  
  @override
  void initState() {
    super.initState();
    // Load product details
    context.read<ProductBloc>().add(
      LoadProductDetailEvent(id: widget.productId),
    );
  }
  
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Product Details'),
        actions: [
          IconButton(
            icon: const Icon(Icons.share),
            onPressed: () {
              // Implement share functionality
            },
          ),
          IconButton(
            icon: const Icon(Icons.favorite_border),
            onPressed: () {
              // Implement add to favorites
            },
          ),
          IconButton(
            icon: const Icon(Icons.shopping_cart),
            onPressed: () {
              // Navigate to cart page
              Navigator.of(context).pushNamed(CartPage.routeName);
            },
          ),
        ],
      ),
      body: BlocBuilder<ProductBloc, ProductState>(
        builder: (context, state) {
          if (state is ProductDetailLoading) {
            return const ProductDetailLoading();
          } else if (state is ProductDetailLoaded) {
            return _buildProductDetailView(state.product);
          } else if (state is ProductError) {
            return ErrorView(
              message: state.message,
              onRetry: () => context.read<ProductBloc>().add(
                LoadProductDetailEvent(id: widget.productId),
              ),
            );
          } else {
            return const SizedBox.shrink();
          }
        },
      ),
      bottomNavigationBar: BlocBuilder<ProductBloc, ProductState>(
        builder: (context, state) {
          if (state is ProductDetailLoaded) {
            return ProductDetailBottomBar(
              product: state.product,
              quantity: _quantity,
              onAddToCart: () => _addToCart(state.product),
            );
          } else {
            return const SizedBox.shrink();
          }
        },
      ),
    );
  }
  
  Widget _buildProductDetailView(Product product) {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Image carousel
          ProductImageCarousel(imageUrls: product.imageUrls),
          const SizedBox(height: 24),
          
          // Title and rating
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: Text(
                  product.name,
                  style: const TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
              if (product.averageRating != null)
                Row(
                  children: [
                    const Icon(
                      Icons.star,
                      color: Colors.amber,
                    ),
                    const SizedBox(width: 4),
                    Text(
                      product.averageRating!.toStringAsFixed(1),
                      style: const TextStyle(
                        fontSize: 18,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    Text(
                      ' (${product.reviewCount})',
                      style: TextStyle(
                        color: Colors.grey[600],
                      ),
                    ),
                  ],
                ),
            ],
          ),
          const SizedBox(height: 8),
          
          // Category
          Text(
            product.categoryName,
            style: TextStyle(
              color: Colors.grey[600],
              fontSize: 16,
            ),
          ),
          const SizedBox(height: 16),
          
          // Price information
          if (product.onSale)
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Text(
                      '\$${product.price.toStringAsFixed(2)}',
                      style: const TextStyle(
                        decoration: TextDecoration.lineThrough,
                        color: Colors.grey,
                        fontSize: 16,
                      ),
                    ),
                    const SizedBox(width: 8),
                    Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 8,
                        vertical: 4,
                      ),
                      decoration: BoxDecoration(
                        color: Colors.red,
                        borderRadius: BorderRadius.circular(4),
                      ),
                      child: Text(
                        '${product.discountPercentage?.toInt() ?? 0}% OFF',
                        style: const TextStyle(
                          color: Colors.white,
                          fontWeight: FontWeight.bold,
                          fontSize: 12,
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 4),
                Text(
                  '\$${product.salePrice.toStringAsFixed(2)}',
                  style: const TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 24,
                    color: Colors.red,
                  ),
                ),
              ],
            )
          else
            Text(
              '\$${product.price.toStringAsFixed(2)}',
              style: const TextStyle(
                fontWeight: FontWeight.bold,
                fontSize: 24,
              ),
            ),
          
          const SizedBox(height: 8),
          
          // Stock status
          Container(
            padding: const EdgeInsets.symmetric(
              horizontal: 8,
              vertical: 4,
            ),
            decoration: BoxDecoration(
              color: product.isInStock ? Colors.green : Colors.red,
              borderRadius: BorderRadius.circular(4),
            ),
            child: Text(
              product.isInStock ? 'In Stock' : 'Out of Stock',
              style: const TextStyle(
                color: Colors.white,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          
          const SizedBox(height: 24),
          
          // Description
          const Text(
            'Description',
            style: TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 8),
          Text(
            product.description,
            style: const TextStyle(
              fontSize: 16,
              height: 1.5,
            ),
          ),
          
          const SizedBox(height: 24),
          
          // Quantity selector (if in stock)
          if (product.isInStock) 
            QuantitySelector(
              quantity: _quantity,
              onChanged: (value) {
                setState(() {
                  _quantity = value;
                });
              },
            ),
          
          const SizedBox(height: 16),
        ],
      ),
    );
  }
  
  void _addToCart(Product product) {
    // Add to cart using CartBloc
    final price = product.onSale ? product.salePrice : product.price;
    
    context.read<CartBloc>().add(
      AddToCartEvent(
        productId: product.id,
        name: product.name,
        imageUrl: product.imageUrls.isNotEmpty ? product.imageUrls[0] : '',
        price: price,
        quantity: _quantity,
      ),
    );
    
    // Show success message
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('${product.name} added to cart'),
        backgroundColor: Colors.green,
        duration: const Duration(seconds: 2),
        action: SnackBarAction(
          label: 'VIEW CART',
          textColor: Colors.white,
          onPressed: () {
            // Navigate to cart page
            Navigator.of(context).pushNamed(CartPage.routeName);
          },
        ),
      ),
    );
  }
}
