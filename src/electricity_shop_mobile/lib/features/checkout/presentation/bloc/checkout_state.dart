import 'package:equatable/equatable.dart';
import '../../domain/entities/address.dart';
import '../../domain/entities/order.dart';
import '../../domain/entities/payment_method.dart';

/// Base class for checkout states
abstract class CheckoutState extends Equatable {
  const CheckoutState();

  @override
  List<Object?> get props => [];
}

/// Initial checkout state
class CheckoutInitial extends CheckoutState {
  const CheckoutInitial();
}

/// Loading state
class CheckoutLoading extends CheckoutState {
  const CheckoutLoading();
}

/// Addresses loaded state
class AddressesLoaded extends CheckoutState {
  final List<Address> addresses;
  final Address? selectedAddress;

  const AddressesLoaded({
    required this.addresses,
    this.selectedAddress,
  });

  @override
  List<Object?> get props => [addresses, selectedAddress];

  /// Create a copy with updated values
  AddressesLoaded copyWith({
    List<Address>? addresses,
    Address? selectedAddress,
    bool clearSelectedAddress = false,
  }) {
    return AddressesLoaded(
      addresses: addresses ?? this.addresses,
      selectedAddress: clearSelectedAddress ? null : selectedAddress ?? this.selectedAddress,
    );
  }
}

/// Address added state
class AddressAdded extends CheckoutState {
  final Address address;

  const AddressAdded({
    required this.address,
  });

  @override
  List<Object?> get props => [address];
}

/// Address updated state
class AddressUpdated extends CheckoutState {
  final Address address;

  const AddressUpdated({
    required this.address,
  });

  @override
  List<Object?> get props => [address];
}

/// Address deleted state
class AddressDeleted extends CheckoutState {
  final String id;

  const AddressDeleted({
    required this.id,
  });

  @override
  List<Object?> get props => [id];
}

/// Payment methods loaded state
class PaymentMethodsLoaded extends CheckoutState {
  final List<PaymentMethod> paymentMethods;
  final PaymentMethod? selectedMethod;

  const PaymentMethodsLoaded({
    required this.paymentMethods,
    this.selectedMethod,
  });

  @override
  List<Object?> get props => [paymentMethods, selectedMethod];

  /// Create a copy with updated values
  PaymentMethodsLoaded copyWith({
    List<PaymentMethod>? paymentMethods,
    PaymentMethod? selectedMethod,
    bool clearSelectedMethod = false,
  }) {
    return PaymentMethodsLoaded(
      paymentMethods: paymentMethods ?? this.paymentMethods,
      selectedMethod: clearSelectedMethod ? null : selectedMethod ?? this.selectedMethod,
    );
  }
}

/// Payment method added state
class PaymentMethodAdded extends CheckoutState {
  final PaymentMethod paymentMethod;

  const PaymentMethodAdded({
    required this.paymentMethod,
  });

  @override
  List<Object?> get props => [paymentMethod];
}

/// Payment method deleted state
class PaymentMethodDeleted extends CheckoutState {
  final String id;

  const PaymentMethodDeleted({
    required this.id,
  });

  @override
  List<Object?> get props => [id];
}

/// Shipping cost calculated state
class ShippingCostCalculated extends CheckoutState {
  final double shippingCost;

  const ShippingCostCalculated({
    required this.shippingCost,
  });

  @override
  List<Object?> get props => [shippingCost];
}

/// Order placed state
class OrderPlaced extends CheckoutState {
  final Order order;

  const OrderPlaced({
    required this.order,
  });

  @override
  List<Object?> get props => [order];
}

/// Error state
class CheckoutError extends CheckoutState {
  final String message;
  final dynamic data;

  const CheckoutError({
    required this.message,
    this.data,
  });

  @override
  List<Object?> get props => [message, data];
}
