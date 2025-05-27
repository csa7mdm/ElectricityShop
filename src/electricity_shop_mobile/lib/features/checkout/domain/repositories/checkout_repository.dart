import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../cart/domain/entities/cart.dart';
import '../entities/address.dart';
import '../entities/order.dart';
import '../entities/payment_method.dart';

/// Repository interface for checkout operations
abstract class CheckoutRepository {
  /// Get saved addresses for the user
  Future<Either<Failure, List<Address>>> getAddresses();
  
  /// Add a new address
  Future<Either<Failure, Address>> addAddress({
    required String fullName,
    required String phone,
    required String addressLine1,
    String? addressLine2,
    required String city,
    required String state,
    required String postalCode,
    required String country,
    bool isDefault = false,
  });
  
  /// Update an existing address
  Future<Either<Failure, Address>> updateAddress({
    required String id,
    required String fullName,
    required String phone,
    required String addressLine1,
    String? addressLine2,
    required String city,
    required String state,
    required String postalCode,
    required String country,
    bool isDefault = false,
  });
  
  /// Delete an address
  Future<Either<Failure, bool>> deleteAddress({
    required String id,
  });
  
  /// Get saved payment methods for the user
  Future<Either<Failure, List<PaymentMethod>>> getPaymentMethods();
  
  /// Add a new payment method
  Future<Either<Failure, PaymentMethod>> addPaymentMethod({
    required PaymentMethodType type,
    required String name,
    String? cardNumber,
    String? cardHolderName,
    String? expiryDate,
    bool isDefault = false,
  });
  
  /// Delete a payment method
  Future<Either<Failure, bool>> deletePaymentMethod({
    required String id,
  });
  
  /// Place an order
  Future<Either<Failure, Order>> placeOrder({
    required Cart cart,
    required Address shippingAddress,
    Address? billingAddress,
    required PaymentMethod paymentMethod,
    required double shippingCost,
  });
  
  /// Calculate shipping cost based on items and shipping address
  Future<Either<Failure, double>> calculateShippingCost({
    required Cart cart,
    required Address shippingAddress,
  });
}
