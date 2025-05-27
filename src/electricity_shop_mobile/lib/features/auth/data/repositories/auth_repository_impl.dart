import 'package:dartz/dartz.dart';
import 'package:jwt_decoder/jwt_decoder.dart';
import '../../../../core/api/api_client.dart';
import '../../../../core/config/api_config.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/storage/secure_storage.dart';
import '../../domain/entities/user.dart';
import '../../domain/repositories/auth_repository.dart';
import '../models/auth_models.dart';
import '../models/user_model.dart';
import 'dart:convert';

/// Implementation of [AuthRepository]
class AuthRepositoryImpl implements AuthRepository {
  final ApiClient _apiClient;
  final SecureStorageService _secureStorage;

  AuthRepositoryImpl(this._apiClient, this._secureStorage);

  @override
  Future<Either<Failure, User>> register({
    required String email,
    required String password,
    required String firstName,
    required String lastName,
  }) async {
    try {
      final request = RegisterRequest(
        email: email,
        password: password,
        firstName: firstName,
        lastName: lastName,
      );

      final response = await _apiClient.post(
        ApiConfig.register,
        data: request.toJson(),
      );

      final authResponse = AuthResponse.fromJson(response);

      if (!authResponse.success) {
        return Left(AuthFailure(
          message: 'Registration failed',
          data: authResponse.errors,
        ));
      }

      // Store tokens
      if (authResponse.token != null) {
        await _secureStorage.setToken(authResponse.token!);
      }
      if (authResponse.refreshToken != null) {
        await _secureStorage.setRefreshToken(authResponse.refreshToken!);
      }

      // Store user data
      if (authResponse.user != null) {
        await _storeUserData(authResponse.user!);
        return Right(authResponse.user!);
      } else {
        return const Left(AuthFailure(
          message: 'User data not received after registration',
        ));
      }
    } on ApiException catch (e) {
      return Left(_mapExceptionToFailure(e));
    } catch (e) {
      return Left(UnexpectedFailure(
        message: 'An unexpected error occurred during registration: $e',
      ));
    }
  }

  @override
  Future<Either<Failure, User>> login({
    required String email,
    required String password,
  }) async {
    try {
      final request = LoginRequest(
        email: email,
        password: password,
      );

      final response = await _apiClient.post(
        ApiConfig.login,
        data: request.toJson(),
      );

      final authResponse = AuthResponse.fromJson(response);

      if (!authResponse.success) {
        return Left(AuthFailure(
          message: 'Login failed',
          data: authResponse.errors,
        ));
      }

      // Store tokens
      if (authResponse.token != null) {
        await _secureStorage.setToken(authResponse.token!);
      }
      if (authResponse.refreshToken != null) {
        await _secureStorage.setRefreshToken(authResponse.refreshToken!);
      }

      // Store user data
      if (authResponse.user != null) {
        await _storeUserData(authResponse.user!);
        return Right(authResponse.user!);
      } else {
        return const Left(AuthFailure(
          message: 'User data not received after login',
        ));
      }
    } on ApiException catch (e) {
      return Left(_mapExceptionToFailure(e));
    } catch (e) {
      return Left(UnexpectedFailure(
        message: 'An unexpected error occurred during login: $e',
      ));
    }
  }

  @override
  Future<Either<Failure, bool>> logout() async {
    try {
      // Clear stored tokens and user data
      await _secureStorage.clearAll();
      return const Right(true);
    } catch (e) {
      return Left(UnexpectedFailure(
        message: 'An error occurred during logout: $e',
      ));
    }
  }

  @override
  Future<Either<Failure, bool>> refreshToken() async {
    try {
      final token = await _secureStorage.getToken();
      final refreshToken = await _secureStorage.getRefreshToken();

      if (token == null || refreshToken == null) {
        return const Left(AuthFailure(
          message: 'No token available for refresh',
        ));
      }

      final request = RefreshTokenRequest(
        token: token,
        refreshToken: refreshToken,
      );

      final response = await _apiClient.post(
        ApiConfig.refresh,
        data: request.toJson(),
      );

      final authResponse = AuthResponse.fromJson(response);

      if (!authResponse.success) {
        return Left(AuthFailure(
          message: 'Token refresh failed',
          data: authResponse.errors,
        ));
      }

      // Update tokens
      if (authResponse.token != null) {
        await _secureStorage.setToken(authResponse.token!);
      }
      if (authResponse.refreshToken != null) {
        await _secureStorage.setRefreshToken(authResponse.refreshToken!);
      }

      return const Right(true);
    } on ApiException catch (e) {
      return Left(_mapExceptionToFailure(e));
    } catch (e) {
      return Left(UnexpectedFailure(
        message: 'An unexpected error occurred during token refresh: $e',
      ));
    }
  }

  @override
  Future<Either<Failure, User?>> getCurrentUser() async {
    try {
      if (!await isAuthenticated()) {
        return const Right(null);
      }

      // Try to get user from secure storage
      final userData = await _getUserData();
      if (userData != null) {
        return Right(userData);
      }

      // If not found, try to decode from JWT token
      final token = await _secureStorage.getToken();
      if (token != null) {
        try {
          final Map<String, dynamic> decodedToken = JwtDecoder.decode(token);
          // Extract user data from token claims
          final String userId = decodedToken['sub'] ?? '';
          final String email = decodedToken['email'] ?? '';
          final String firstName = decodedToken['firstName'] ?? '';
          final String lastName = decodedToken['lastName'] ?? '';
          final List<String> roles = (decodedToken['roles'] as List<dynamic>?)
                  ?.map((role) => role.toString())
                  .toList() ??
              [];

          final user = UserModel(
            id: userId,
            email: email,
            firstName: firstName,
            lastName: lastName,
            roles: roles,
          );

          // Store user data for future use
          await _storeUserData(user);
          return Right(user);
        } catch (e) {
          return Left(AuthFailure(
            message: 'Failed to decode user data from token',
          ));
        }
      }

      return const Right(null);
    } catch (e) {
      return Left(UnexpectedFailure(
        message: 'An unexpected error occurred: $e',
      ));
    }
  }

  @override
  Future<bool> isAuthenticated() async {
    try {
      final token = await _secureStorage.getToken();
      if (token == null) {
        return false;
      }

      // Check if token is expired
      bool isExpired = JwtDecoder.isExpired(token);
      if (isExpired) {
        // Try to refresh the token
        final refreshResult = await refreshToken();
        return refreshResult.isRight();
      }

      return true;
    } catch (e) {
      return false;
    }
  }

  // Helper method to store user data
  Future<void> _storeUserData(UserModel user) async {
    await _secureStorage.setUsername(user.email);
    
    // Convert the user model to JSON string and store it
    final userJson = jsonEncode(user.toJson());
    await _secureStorage.setUserData(userJson);
  }

  // Helper method to retrieve user data
  Future<UserModel?> _getUserData() async {
    final userJson = await _secureStorage.getUserData();
    if (userJson != null) {
      try {
        final userMap = jsonDecode(userJson) as Map<String, dynamic>;
        return UserModel.fromJson(userMap);
      } catch (e) {
        return null;
      }
    }
    return null;
  }

  // Helper method to map API exceptions to domain failures
  Failure _mapExceptionToFailure(ApiException exception) {
    if (exception is UnauthorizedException) {
      return AuthFailure(
        message: exception.message,
        data: exception.data,
      );
    } else if (exception is BadRequestException) {
      return ValidationFailure(
        message: exception.message,
        data: exception.data,
      );
    } else if (exception is NotFoundException) {
      return NotFoundFailure(
        message: exception.message,
        data: exception.data,
      );
    } else if (exception is ForbiddenException) {
      return PermissionFailure(
        message: exception.message,
        data: exception.data,
      );
    } else if (exception is NetworkException) {
      return NetworkFailure(
        message: exception.message,
        data: exception.data,
      );
    } else if (exception is ServerException) {
      return ServerFailure(
        message: exception.message,
        data: exception.data,
      );
    } else {
      return UnexpectedFailure(
        message: exception.message,
        data: exception.data,
      );
    }
  }
}
