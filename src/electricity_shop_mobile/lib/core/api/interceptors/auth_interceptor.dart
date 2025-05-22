import 'package:dio/dio.dart';
import '../../storage/secure_storage.dart';
import '../../config/api_config.dart';

/// Interceptor for handling authentication in API requests
class AuthInterceptor extends Interceptor {
  final SecureStorageService _secureStorage;
  final Dio _dio;
  
  AuthInterceptor(this._secureStorage, this._dio);
  
  @override
  void onRequest(
    RequestOptions options, 
    RequestInterceptorHandler handler
  ) async {
    final token = await _secureStorage.getToken();
    if (token != null) {
      options.headers['Authorization'] = 'Bearer $token';
    }
    handler.next(options);
  }
  
  @override
  void onError(DioException err, ErrorInterceptorHandler handler) async {
    // Check if error is due to token expiration (401 Unauthorized)
    if (err.response?.statusCode == 401) {
      // Token expired, try to refresh
      if (await _refreshToken()) {
        // If token refresh was successful, retry the failed request
        return handler.resolve(await _retryRequest(err.requestOptions));
      }
    }
    // If token refresh failed or another error occurred, continue with the error
    return handler.next(err);
  }
  
  /// Attempt to refresh the authentication token
  Future<bool> _refreshToken() async {
    final refreshToken = await _secureStorage.getRefreshToken();
    
    if (refreshToken == null) {
      return false;
    }
    
    try {
      final response = await _dio.post(
        ApiConfig.refresh,
        data: {
          'refreshToken': refreshToken,
          'token': await _secureStorage.getToken()
        },
        options: Options(
          headers: {'Content-Type': 'application/json'},
        ),
      );
      
      if (response.statusCode == 200) {
        // Store the new tokens
        final String newToken = response.data['token'];
        final String newRefreshToken = response.data['refreshToken'];
        
        await _secureStorage.setToken(newToken);
        await _secureStorage.setRefreshToken(newRefreshToken);
        
        return true;
      }
      return false;
    } catch (e) {
      return false;
    }
  }
  
  /// Retry the original request with the new token
  Future<Response<dynamic>> _retryRequest(RequestOptions requestOptions) async {
    final token = await _secureStorage.getToken();
    
    // Update the authorization header with the new token
    requestOptions.headers['Authorization'] = 'Bearer $token';
    
    // Create a new request with the original parameters
    final options = Options(
      method: requestOptions.method,
      headers: requestOptions.headers,
    );
    
    return _dio.request<dynamic>(
      requestOptions.path,
      data: requestOptions.data,
      queryParameters: requestOptions.queryParameters,
      options: options,
    );
  }
}
