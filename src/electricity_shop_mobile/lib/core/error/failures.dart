import 'package:equatable/equatable.dart';

/// Base failure class for all domain-level failures
abstract class Failure extends Equatable {
  final String message;
  final dynamic data;

  const Failure({
    required this.message,
    this.data,
  });

  @override
  List<Object?> get props => [message, data];
}

/// Failure for server-related errors
class ServerFailure extends Failure {
  const ServerFailure({
    required String message,
    dynamic data,
  }) : super(message: message, data: data);
}

/// Failure for network connectivity issues
class NetworkFailure extends Failure {
  const NetworkFailure({
    required String message,
    dynamic data,
  }) : super(message: message, data: data);
}

/// Failure for authentication errors
class AuthFailure extends Failure {
  const AuthFailure({
    required String message,
    dynamic data,
  }) : super(message: message, data: data);
}

/// Failure for input validation errors
class ValidationFailure extends Failure {
  const ValidationFailure({
    required String message,
    dynamic data,
  }) : super(message: message, data: data);
}

/// Failure for resource not found errors
class NotFoundFailure extends Failure {
  const NotFoundFailure({
    required String message,
    dynamic data,
  }) : super(message: message, data: data);
}

/// Failure for cache-related errors
class CacheFailure extends Failure {
  const CacheFailure({
    required String message,
    dynamic data,
  }) : super(message: message, data: data);
}

/// Failure for permission errors
class PermissionFailure extends Failure {
  const PermissionFailure({
    required String message,
    dynamic data,
  }) : super(message: message, data: data);
}

/// Failure for unexpected errors
class UnexpectedFailure extends Failure {
  const UnexpectedFailure({
    required String message,
    dynamic data,
  }) : super(message: message, data: data);
}
