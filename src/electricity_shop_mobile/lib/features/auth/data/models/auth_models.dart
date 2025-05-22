import 'package:json_annotation/json_annotation.dart';
import 'user_model.dart';

part 'auth_models.g.dart';

/// Request model for user registration
@JsonSerializable()
class RegisterRequest {
  final String email;
  final String password;
  final String firstName;
  final String lastName;

  RegisterRequest({
    required this.email,
    required this.password,
    required this.firstName,
    required this.lastName,
  });

  /// Creates a [RegisterRequest] from JSON map
  factory RegisterRequest.fromJson(Map<String, dynamic> json) => 
      _$RegisterRequestFromJson(json);

  /// Converts [RegisterRequest] to JSON map
  Map<String, dynamic> toJson() => _$RegisterRequestToJson(this);
}

/// Request model for user login
@JsonSerializable()
class LoginRequest {
  final String email;
  final String password;

  LoginRequest({
    required this.email,
    required this.password,
  });

  /// Creates a [LoginRequest] from JSON map
  factory LoginRequest.fromJson(Map<String, dynamic> json) => 
      _$LoginRequestFromJson(json);

  /// Converts [LoginRequest] to JSON map
  Map<String, dynamic> toJson() => _$LoginRequestToJson(this);
}

/// Request model for refreshing tokens
@JsonSerializable()
class RefreshTokenRequest {
  final String token;
  final String refreshToken;

  RefreshTokenRequest({
    required this.token,
    required this.refreshToken,
  });

  /// Creates a [RefreshTokenRequest] from JSON map
  factory RefreshTokenRequest.fromJson(Map<String, dynamic> json) => 
      _$RefreshTokenRequestFromJson(json);

  /// Converts [RefreshTokenRequest] to JSON map
  Map<String, dynamic> toJson() => _$RefreshTokenRequestToJson(this);
}

/// Response model for authentication operations
@JsonSerializable()
class AuthResponse {
  final bool success;
  final String? token;
  final String? refreshToken;
  final UserModel? user;
  final List<String>? errors;

  AuthResponse({
    required this.success,
    this.token,
    this.refreshToken,
    this.user,
    this.errors,
  });

  /// Creates an [AuthResponse] from JSON map
  factory AuthResponse.fromJson(Map<String, dynamic> json) => 
      _$AuthResponseFromJson(json);

  /// Converts [AuthResponse] to JSON map
  Map<String, dynamic> toJson() => _$AuthResponseToJson(this);
}
