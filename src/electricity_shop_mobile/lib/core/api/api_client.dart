import 'package:dio/dio.dart';
import '../config/api_config.dart';
import '../config/app_config.dart';
import '../storage/secure_storage.dart';
import 'interceptors/auth_interceptor.dart';
import '../error/exceptions.dart';
import 'package:logger/logger.dart';

/// API client for making HTTP requests to the backend
class ApiClient {
  late final Dio _dio;
  final Logger _logger = Logger();
  final SecureStorageService _secureStorage;
  
  ApiClient(this._secureStorage) {
    _dio = Dio(
      BaseOptions(
        baseUrl: ApiConfig.baseUrl,
        connectTimeout: Duration(seconds: AppConfig.connectionTimeout),
        receiveTimeout: Duration(seconds: AppConfig.receiveTimeout),
        contentType: 'application/json',
      ),
    );
    
    // Add logging interceptor
    _dio.interceptors.add(
      LogInterceptor(
        logPrint: (object) => _logger.d(object.toString()),
        requestHeader: true,
        requestBody: true,
        responseHeader: true,
        responseBody: true,
        error: true,
      ),
    );
    
    // Add auth interceptor
    _dio.interceptors.add(AuthInterceptor(_secureStorage, _dio));
  }
  
  /// Perform a GET request
  Future<dynamic> get(
    String path, {
    Map<String, dynamic>? queryParameters,
    Options? options,
  }) async {
    try {
      final response = await _dio.get(
        path,
        queryParameters: queryParameters,
        options: options,
      );
      return response.data;
    } on DioException catch (e) {
      _handleDioError(e);
    } catch (e) {
      throw ServerException(message: 'Unexpected error occurred: $e');
    }
  }
  
  /// Perform a POST request
  Future<dynamic> post(
    String path, {
    dynamic data,
    Map<String, dynamic>? queryParameters,
    Options? options,
  }) async {
    try {
      final response = await _dio.post(
        path,
        data: data,
        queryParameters: queryParameters,
        options: options,
      );
      return response.data;
    } on DioException catch (e) {
      _handleDioError(e);
    } catch (e) {
      throw ServerException(message: 'Unexpected error occurred: $e');
    }
  }
  
  /// Perform a PUT request
  Future<dynamic> put(
    String path, {
    dynamic data,
    Map<String, dynamic>? queryParameters,
    Options? options,
  }) async {
    try {
      final response = await _dio.put(
        path,
        data: data,
        queryParameters: queryParameters,
        options: options,
      );
      return response.data;
    } on DioException catch (e) {
      _handleDioError(e);
    } catch (e) {
      throw ServerException(message: 'Unexpected error occurred: $e');
    }
  }
  
  /// Perform a DELETE request
  Future<dynamic> delete(
    String path, {
    dynamic data,
    Map<String, dynamic>? queryParameters,
    Options? options,
  }) async {
    try {
      final response = await _dio.delete(
        path,
        data: data,
        queryParameters: queryParameters,
        options: options,
      );
      return response.data;
    } on DioException catch (e) {
      _handleDioError(e);
    } catch (e) {
      throw ServerException(message: 'Unexpected error occurred: $e');
    }
  }
  
  /// Handle Dio errors by converting them to our custom exceptions
  void _handleDioError(DioException error) {
    if (error.response != null) {
      final statusCode = error.response?.statusCode;
      final data = error.response?.data;
      
      switch (statusCode) {
        case 400:
          throw BadRequestException(
            message: 'Bad request',
            data: data,
          );
        case 401:
          throw UnauthorizedException(
            message: 'Unauthorized',
            data: data,
          );
        case 403:
          throw ForbiddenException(
            message: 'Forbidden',
            data: data,
          );
        case 404:
          throw NotFoundException(
            message: 'Resource not found',
            data: data,
          );
        case 500:
          throw ServerException(
            message: 'Server error',
            data: data,
          );
        default:
          throw ServerException(
            message: 'Error occurred: ${error.message}',
            data: data,
          );
      }
    } else {
      // No response from server - likely a connection issue
      throw NetworkException(
        message: 'Network error: ${error.message}',
      );
    }
  }
}
