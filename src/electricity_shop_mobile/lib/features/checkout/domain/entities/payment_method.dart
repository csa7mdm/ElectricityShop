import 'package:equatable/equatable.dart';

/// Payment method types
enum PaymentMethodType {
  creditCard,
  paypal,
  applePay,
  googlePay,
  cashOnDelivery,
}

/// Entity class for a payment method
class PaymentMethod extends Equatable {
  final String id;
  final PaymentMethodType type;
  final String name;
  final String? cardNumber;
  final String? cardHolderName;
  final String? expiryDate;
  final bool isDefault;

  const PaymentMethod({
    required this.id,
    required this.type,
    required this.name,
    this.cardNumber,
    this.cardHolderName,
    this.expiryDate,
    this.isDefault = false,
  });

  /// Create a copy of this PaymentMethod with provided parameters
  PaymentMethod copyWith({
    String? id,
    PaymentMethodType? type,
    String? name,
    String? cardNumber,
    String? cardHolderName,
    String? expiryDate,
    bool? isDefault,
  }) {
    return PaymentMethod(
      id: id ?? this.id,
      type: type ?? this.type,
      name: name ?? this.name,
      cardNumber: cardNumber ?? this.cardNumber,
      cardHolderName: cardHolderName ?? this.cardHolderName,
      expiryDate: expiryDate ?? this.expiryDate,
      isDefault: isDefault ?? this.isDefault,
    );
  }

  /// Get masked card number for display (e.g., **** **** **** 1234)
  String get maskedCardNumber {
    if (cardNumber == null || cardNumber!.isEmpty) {
      return '';
    }
    
    if (cardNumber!.length <= 4) {
      return cardNumber!;
    }
    
    final lastFour = cardNumber!.substring(cardNumber!.length - 4);
    return '**** **** **** $lastFour';
  }

  @override
  List<Object?> get props => [
    id, 
    type, 
    name, 
    cardNumber, 
    cardHolderName, 
    expiryDate,
    isDefault,
  ];
}
