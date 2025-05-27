import 'package:equatable/equatable.dart';
import '../../domain/entities/address.dart';
import '../../domain/entities/payment_method.dart';
import '../../../cart/domain/entities/cart.dart';

/// Base class for checkout events
abstract class CheckoutEvent extends Equatable {
  const CheckoutEvent();

  @override
  List<Object?> get props => [];
}

/// Event to load saved addresses
class LoadAddressesEvent extends CheckoutEvent {
  const LoadAddressesEvent();
}

/// Event to add a new address
class AddAddressEvent extends CheckoutEvent {
  final String fullName;
  final String phone;
  final String addressLine1;
  final String? addressLine2;
  final String city;
  final String state;
  final String postalCode;
  final String country;
  final bool isDefault;

  const AddAddressEvent({
    required this.fullName,
    required this.phone,
    required this.addressLine1,
    this.addressLine2,
    required this.city,
    required this.state,
    required this.postalCode,
    required this.country,
    this.isDefault = false,
  });

  @override
  List<Object?> get props => [
        fullName,
        phone,
        addressLine1,
        addressLine2,
        city,
        state,
        postalCode,
        country,
        isDefault,
      ];
}

/// Event to update an existing address
class UpdateAddressEvent extends CheckoutEvent {
  final String id;
  final String fullName;
  final String phone;
  final String addressLine1;
  final String? addressLine2;
  final String city;
  final String state;
  final String postalCode;
  final String country;
  final bool isDefault;

  const UpdateAddressEvent({
    required this.id,
    required this.fullName,
    required this.phone,
    required this.addressLine1,
    this.addressLine2,
    required this.city,
    required this.state,
    required this.postalCode,
    required this.country,
    this.isDefault = false,
  });

  @override
  List<Object?> get props => [
        id,
        fullName,
        phone,
        addressLine1,
        addressLine2,
        city,
        state,
        postalCode,
        country,
        isDefault,
      ];
}

/// Event to delete an address
class DeleteAddressEvent extends CheckoutEvent {
  final String id;

  const DeleteAddressEvent({
    required this.id,
  });

  @override
  List<Object?> get props => [id];
}

/// Event to load saved payment methods
class LoadPaymentMethodsEvent extends CheckoutEvent {
  const LoadPaymentMethodsEvent();
}

/// Event to add a new payment method
class AddPaymentMethodEvent extends CheckoutEvent {
  final PaymentMethodType type;
  final String name;
  final String? cardNumber;
  final String? cardHolderName;
  final String? expiryDate;
  final bool isDefault;

  const AddPaymentMethodEvent({
    required this.type,
    required this.name,
    this.cardNumber,
    this.cardHolderName,
    this.expiryDate,
    this.isDefault = false,
  });

  @override
  List<Object?> get props => [
        type,
        name,
        cardNumber,
        cardHolderName,
        expiryDate,
        isDefault,
      ];
}

/// Event to delete a payment method
class DeletePaymentMethodEvent extends CheckoutEvent {
  final String id;

  const DeletePaymentMethodEvent({
    required this.id,
  });

  @override
  List<Object?> get props => [id];
}

/// Event to select a shipping address
class SelectShippingAddressEvent extends CheckoutEvent {
  final Address address;

  const SelectShippingAddressEvent({
    required this.address,
  });

  @override
  List<Object?> get props => [address];
}

/// Event to select a payment method
class SelectPaymentMethodEvent extends CheckoutEvent {
  final PaymentMethod paymentMethod;

  const SelectPaymentMethodEvent({
    required this.paymentMethod,
  });

  @override
  List<Object?> get props => [paymentMethod];
}

/// Event to calculate shipping cost
class CalculateShippingCostEvent extends CheckoutEvent {
  final Cart cart;
  final Address shippingAddress;

  const CalculateShippingCostEvent({
    required this.cart,
    required this.shippingAddress,
  });

  @override
  List<Object?> get props => [cart, shippingAddress];
}

/// Event to place an order
class PlaceOrderEvent extends CheckoutEvent {
  final Cart cart;
  final Address shippingAddress;
  final Address? billingAddress;
  final PaymentMethod paymentMethod;
  final double shippingCost;

  const PlaceOrderEvent({
    required this.cart,
    required this.shippingAddress,
    this.billingAddress,
    required this.paymentMethod,
    required this.shippingCost,
  });

  @override
  List<Object?> get props => [
        cart,
        shippingAddress,
        billingAddress,
        paymentMethod,
        shippingCost,
      ];
}
