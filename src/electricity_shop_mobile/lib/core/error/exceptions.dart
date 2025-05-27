/// Base exception class for all API related exceptions
class ApiException implements Exception {
  final String message;
  final dynamic data;

  ApiException({
    required this.message,
    this.data,
  });

  @override
  String toString() => message;
}

/// Exception for 400 Bad Request responses
class BadRequestException extends ApiException {
  BadRequestException({
    required String message,
    dynamic data,
  }) : super(message: message, data: data);
}

/// Exception for 401 Unauthorized responses
class UnauthorizedException extends ApiException {
  UnauthorizedException({
    required String message,
    dynamic data,
  }) : super(message: message, data: data);
}

/// Exception for 403 Forbidden responses
class ForbiddenException extends ApiException {
  ForbiddenException({
    required String message,
    dynamic data,
  }) : super(message: message, data: data);
}

/// Exception for 404 Not Found responses
class NotFoundException extends ApiException {
  NotFoundException({
    required String message,
    dynamic data,
  }) : super(message: message, data: data);
}

/// Exception for 500 Server Error responses
class ServerException extends ApiException {
  ServerException({
    required String message,
    dynamic data,
  }) : super(message: message, data: data);
}

/// Exception for network connectivity issues
class NetworkException extends ApiException {
  NetworkException({
    required String message,
    dynamic data,
  }) : super(message: message, data: data);
}

/// Exception for local data caching errors
class CacheException implements Exception {
  final String message;

  CacheException({required this.message});

  @override
  String toString() => message;
}
