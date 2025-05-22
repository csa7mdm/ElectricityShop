import 'package:json_annotation/json_annotation.dart';
import '../../domain/entities/address.dart';

part 'address_model.g.dart';

/// Data model for an address
@JsonSerializable()
class AddressModel extends Address {
  const AddressModel({
    required super.id,
    required super.fullName,
    required super.phone,
    required super.addressLine1,
    super.addressLine2,
    required super.city,
    required super.state,
    required super.postalCode,
    required super.country,
    super.isDefault,
  });

  /// Create model from JSON
  factory AddressModel.fromJson(Map<String, dynamic> json) => _$AddressModelFromJson(json);

  /// Convert model to JSON
  Map<String, dynamic> toJson() => _$AddressModelToJson(this);

  /// Create model from entity
  factory AddressModel.fromEntity(Address entity) {
    return AddressModel(
      id: entity.id,
      fullName: entity.fullName,
      phone: entity.phone,
      addressLine1: entity.addressLine1,
      addressLine2: entity.addressLine2,
      city: entity.city,
      state: entity.state,
      postalCode: entity.postalCode,
      country: entity.country,
      isDefault: entity.isDefault,
    );
  }
}
