import 'package:json_annotation/json_annotation.dart';
import '../../domain/entities/user.dart';

part 'user_model.g.dart';

/// User model for data transfer and serialization
@JsonSerializable()
class UserModel extends User {
  const UserModel({
    required String id,
    required String email,
    required String firstName,
    required String lastName,
    required List<String> roles,
  }) : super(
          id: id,
          email: email,
          firstName: firstName,
          lastName: lastName,
          roles: roles,
        );

  /// Creates a [UserModel] from JSON map
  factory UserModel.fromJson(Map<String, dynamic> json) => _$UserModelFromJson(json);

  /// Converts [UserModel] to JSON map
  Map<String, dynamic> toJson() => _$UserModelToJson(this);

  /// Creates a copy of [UserModel] with specified fields replaced
  UserModel copyWith({
    String? id,
    String? email,
    String? firstName,
    String? lastName,
    List<String>? roles,
  }) {
    return UserModel(
      id: id ?? this.id,
      email: email ?? this.email,
      firstName: firstName ?? this.firstName,
      lastName: lastName ?? this.lastName,
      roles: roles ?? this.roles,
    );
  }
}
