import 'package:equatable/equatable.dart';
import '../../domain/entities/user.dart';

/// Base class for authentication states
abstract class AuthState extends Equatable {
  const AuthState();

  @override
  List<Object?> get props => [];
}

/// Initial state
class AuthInitial extends AuthState {}

/// Loading state for authentication operations
class AuthLoading extends AuthState {}

/// State for an authenticated user
class Authenticated extends AuthState {
  final User user;

  const Authenticated(this.user);

  @override
  List<Object?> get props => [user];
}

/// State for an unauthenticated user
class Unauthenticated extends AuthState {}

/// State for authentication errors
class AuthError extends AuthState {
  final String message;
  final dynamic data;

  const AuthError({
    required this.message,
    this.data,
  });

  @override
  List<Object?> get props => [message, data];
}

/// State for successful registration
class RegisterSuccess extends AuthState {
  final User user;

  const RegisterSuccess(this.user);

  @override
  List<Object?> get props => [user];
}

/// State for successful login
class LoginSuccess extends AuthState {
  final User user;

  const LoginSuccess(this.user);

  @override
  List<Object?> get props => [user];
}

/// State for successful logout
class LogoutSuccess extends AuthState {}
