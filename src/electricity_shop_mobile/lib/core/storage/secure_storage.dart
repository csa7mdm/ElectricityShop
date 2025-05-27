import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../config/app_config.dart';

/// Service for securely storing sensitive information like auth tokens
class SecureStorageService {
  final FlutterSecureStorage _storage = const FlutterSecureStorage();
  
  // Storage keys
  static const String _userDataKey = 'user_data';
  static const String _usernameKey = 'username';
  
  // Token storage operations
  
  /// Store JWT token
  Future<void> setToken(String token) async {
    await _storage.write(key: AppConfig.tokenKey, value: token);
  }
  
  /// Get JWT token
  Future<String?> getToken() async {
    return await _storage.read(key: AppConfig.tokenKey);
  }
  
  /// Remove JWT token
  Future<void> deleteToken() async {
    await _storage.delete(key: AppConfig.tokenKey);
  }
  
  // Refresh token operations
  
  /// Store refresh token
  Future<void> setRefreshToken(String refreshToken) async {
    await _storage.write(key: AppConfig.refreshTokenKey, value: refreshToken);
  }
  
  /// Get refresh token
  Future<String?> getRefreshToken() async {
    return await _storage.read(key: AppConfig.refreshTokenKey);
  }
  
  /// Remove refresh token
  Future<void> deleteRefreshToken() async {
    await _storage.delete(key: AppConfig.refreshTokenKey);
  }
  
  // User data operations
  
  /// Store user data as JSON string
  Future<void> setUserData(String userData) async {
    await _storage.write(key: _userDataKey, value: userData);
  }
  
  /// Get user data as JSON string
  Future<String?> getUserData() async {
    return await _storage.read(key: _userDataKey);
  }
  
  /// Remove user data
  Future<void> deleteUserData() async {
    await _storage.delete(key: _userDataKey);
  }
  
  /// Store username (email)
  Future<void> setUsername(String username) async {
    await _storage.write(key: _usernameKey, value: username);
  }
  
  /// Get username (email)
  Future<String?> getUsername() async {
    return await _storage.read(key: _usernameKey);
  }
  
  /// Clear all secure storage (for logout)
  Future<void> clearAll() async {
    await _storage.deleteAll();
  }
}
