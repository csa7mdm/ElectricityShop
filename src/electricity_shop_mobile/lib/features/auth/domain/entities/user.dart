import 'package:equatable/equatable.dart';

/// User entity representing a user in the system
class User extends Equatable {
  /// Unique identifier
  final String id;
  
  /// Email address
  final String email;
  
  /// First name
  final String firstName;
  
  /// Last name
  final String lastName;
  
  /// User roles (e.g., 'User', 'Admin')
  final List<String> roles;

  /// Creates a new [User] instance
  const User({
    required this.id,
    required this.email,
    required this.firstName,
    required this.lastName,
    required this.roles,
  });

  /// Full name of the user
  String get fullName => '$firstName $lastName';

  /// Checks if user has admin role
  bool get isAdmin => roles.contains('Admin');

  @override
  List<Object?> get props => [id, email, firstName, lastName, roles];
}
