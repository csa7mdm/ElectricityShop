/// Configuration for API endpoints
class ApiConfig {
  ApiConfig._();

  /// Base URL of the API
  // Base URL should be changed according to your environment (development, staging, production)
  static const String baseUrl = 'http://10.0.2.2:5000'; // Default for Android emulator pointing to localhost

  /// Authentication endpoints
  static const String register = '/api/auth/register';
  static const String login = '/api/auth/login';
  static const String refresh = '/api/auth/refresh';

  /// Product endpoints
  static const String products = '/api/products';
  static const String productById = '/api/products/'; // Append product ID

  /// Cart endpoints
  static const String cart = '/api/cart';
  static const String cartItems = '/api/cart/items';
  static const String cartItemById = '/api/cart/items/'; // Append item ID

  /// Order endpoints
  static const String orders = '/api/orders';
  static const String orderById = '/api/orders/'; // Append order ID
  static const String cancelOrder = '/api/orders/'; // Append order ID + /cancel
  static const String payOrder = '/api/orders/'; // Append order ID + /pay
}
