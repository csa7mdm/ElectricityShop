import 'package:json_annotation/json_annotation.dart';
import '../../domain/entities/payment_method.dart';

part 'payment_method_model.g.dart';

/// Data model for a payment method
@JsonSerializable()
class PaymentMethodModel extends PaymentMethod {
  const PaymentMethodModel({
    required super.id,
    required super.type,
    required super.name,
    super.cardNumber,
    super.cardHolderName,
    super.expiryDate,
    super.isDefault,
  });

  /// Create model from JSON
  factory PaymentMethodModel.fromJson(Map<String, dynamic> json) => _$PaymentMethodModelFromJson(json);

  /// Convert model to JSON
  Map<String, dynamic> toJson() => _$PaymentMethodModelToJson(this);

  /// Create model from entity
  factory PaymentMethodModel.fromEntity(PaymentMethod entity) {
    return PaymentMethodModel(
      id: entity.id,
      type: entity.type,
      name: entity.name,
      cardNumber: entity.cardNumber,
      cardHolderName: entity.cardHolderName,
      expiryDate: entity.expiryDate,
      isDefault: entity.isDefault,
    );
  }
}
