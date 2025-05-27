/// Application configuration
class AppConfig {
  AppConfig._();

  /// App name
  static const String appName = 'ElectricityShop';

  /// App version
  static const String appVersion = '1.0.0';

  /// Timeout for API requests in seconds
  static const int connectionTimeout = 30;
  static const int receiveTimeout = 30;

  /// Pagination defaults
  static const int defaultPageSize = 10;
  static const int initialPageKey = 1;

  /// Local storage keys
  static const String tokenKey = 'auth_token';
  static const String refreshTokenKey = 'refresh_token';
  static const String userDataKey = 'user_data';
  static const String isDarkModeKey = 'is_dark_mode';
}
