import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/user.dart';

/// Interface for authentication repository
abstract class AuthRepository {
  /// Register a new user
  Future<Either<Failure, User>> register({
    required String email,
    required String password,
    required String firstName,
    required String lastName,
  });

  /// Login a user
  Future<Either<Failure, User>> login({
    required String email,
    required String password,
  });

  /// Logout the currently authenticated user
  Future<Either<Failure, bool>> logout();

  /// Refresh the authentication token
  Future<Either<Failure, bool>> refreshToken();

  /// Get the currently authenticated user
  Future<Either<Failure, User?>> getCurrentUser();

  /// Check if a user is authenticated
  Future<bool> isAuthenticated();
}
