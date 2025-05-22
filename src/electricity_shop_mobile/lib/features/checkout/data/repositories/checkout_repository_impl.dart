import 'dart:convert';
import 'package:dartz/dartz.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../../../core/api/api_client.dart';
import '../../../../core/error/failures.dart';
import '../../../cart/domain/entities/cart.dart';
import '../../domain/entities/address.dart';
import '../../domain/entities/order.dart';
import '../../domain/entities/payment_method.dart';
import '../../domain/repositories/checkout_repository.dart';
import '../models/address_model.dart';
import '../models/payment_method_model.dart';
import '../models/order_model.dart';

/// Implementation of CheckoutRepository
class CheckoutRepositoryImpl implements CheckoutRepository {
  final ApiClient _apiClient;
  final SharedPreferences _sharedPreferences;
  final FlutterSecureStorage _secureStorage;

  static const String _addressesKey = 'user_addresses';
  static const String _paymentMethodsKey = 'user_payment_methods';

  CheckoutRepositoryImpl(
    this._apiClient,
    this._sharedPreferences,
    this._secureStorage,
  );

  @override
  Future<Either<Failure, List<Address>>> getAddresses() async {
    try {
      // In a real app, we would fetch addresses from the server
      // For now, we'll use local storage
      final addressesJson = _sharedPreferences.getString(_addressesKey);
      
      if (addressesJson == null) {
        return const Right([]);
      }
      
      final addressesList = json.decode(addressesJson) as List;
      final addresses = addressesList
          .map((json) => AddressModel.fromJson(json))
          .toList();
      
      return Right(addresses);
    } catch (e) {
      return Left(CacheFailure(
        message: 'Failed to fetch addresses',
        data: e.toString(),
      ));
    }
  }

  @override
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
  }) async {
    try {
      // Get existing addresses
      final addressesResult = await getAddresses();
      
      return addressesResult.fold(
        (failure) => Left(failure),
        (addresses) async {
          // Create new address
          final newAddress = AddressModel(
            id: DateTime.now().millisecondsSinceEpoch.toString(),
            fullName: fullName,
            phone: phone,
            addressLine1: addressLine1,
            addressLine2: addressLine2,
            city: city,
            state: state,
            postalCode: postalCode,
            country: country,
            isDefault: isDefault,
          );
          
          final List<AddressModel> updatedAddresses = [];
          
          // If this is set as default, unset default for other addresses
          for (final address in addresses) {
            if (isDefault && address.isDefault) {
              updatedAddresses.add(
                AddressModel.fromEntity(address.copyWith(isDefault: false)),
              );
            } else {
              updatedAddresses.add(AddressModel.fromEntity(address));
            }
          }
          
          // Add the new address
          updatedAddresses.add(newAddress);
          
          // Save to storage
          final addressesJson = json.encode(
            updatedAddresses.map((a) => (a as AddressModel).toJson()).toList(),
          );
          await _sharedPreferences.setString(_addressesKey, addressesJson);
          
          return Right(newAddress);
        },
      );
    } catch (e) {
      return Left(CacheFailure(
        message: 'Failed to add address',
        data: e.toString(),
      ));
    }
  }

  @override
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
  }) async {
    try {
      // Get existing addresses
      final addressesResult = await getAddresses();
      
      return addressesResult.fold(
        (failure) => Left(failure),
        (addresses) async {
          // Find the address to update
          final addressIndex = addresses.indexWhere((a) => a.id == id);
          
          if (addressIndex < 0) {
            return Left(NotFoundFailure(
              message: 'Address not found',
              data: {'addressId': id},
            ));
          }
          
          // Create updated address
          final updatedAddress = AddressModel(
            id: id,
            fullName: fullName,
            phone: phone,
            addressLine1: addressLine1,
            addressLine2: addressLine2,
            city: city,
            state: state,
            postalCode: postalCode,
            country: country,
            isDefault: isDefault,
          );
          
          final List<AddressModel> updatedAddresses = [];
          
          // Update the list of addresses
          for (int i = 0; i < addresses.length; i++) {
            if (i == addressIndex) {
              updatedAddresses.add(updatedAddress);
            } else if (isDefault && addresses[i].isDefault) {
              // Unset default for other addresses if this one is default
              updatedAddresses.add(
                AddressModel.fromEntity(addresses[i].copyWith(isDefault: false)),
              );
            } else {
              updatedAddresses.add(AddressModel.fromEntity(addresses[i]));
            }
          }
          
          // Save to storage
          final addressesJson = json.encode(
            updatedAddresses.map((a) => (a as AddressModel).toJson()).toList(),
          );
          await _sharedPreferences.setString(_addressesKey, addressesJson);
          
          return Right(updatedAddress);
        },
      );
    } catch (e) {
      return Left(CacheFailure(
        message: 'Failed to update address',
        data: e.toString(),
      ));
    }
  }

  @override
  Future<Either<Failure, bool>> deleteAddress({
    required String id,
  }) async {
    try {
      // Get existing addresses
      final addressesResult = await getAddresses();
      
      return addressesResult.fold(
        (failure) => Left(failure),
        (addresses) async {
          // Remove the address
          final updatedAddresses = addresses
              .where((a) => a.id != id)
              .map((a) => AddressModel.fromEntity(a))
              .toList();
          
          // Check if we found and removed the address
          if (updatedAddresses.length == addresses.length) {
            return Left(NotFoundFailure(
              message: 'Address not found',
              data: {'addressId': id},
            ));
          }
          
          // Save to storage
          final addressesJson = json.encode(
            updatedAddresses.map((a) => (a as AddressModel).toJson()).toList(),
          );
          await _sharedPreferences.setString(_addressesKey, addressesJson);
          
          return const Right(true);
        },
      );
    } catch (e) {
      return Left(CacheFailure(
        message: 'Failed to delete address',
        data: e.toString(),
      ));
    }
  }

  @override
  Future<Either<Failure, List<PaymentMethod>>> getPaymentMethods() async {
    try {
      // In a real app, payment methods (especially card details) should be handled securely
      // For demo purposes, we'll store minimal info in secure storage
      final methodsJson = await _secureStorage.read(key: _paymentMethodsKey);
      
      if (methodsJson == null) {
        return const Right([]);
      }
      
      final methodsList = json.decode(methodsJson) as List;
      final methods = methodsList
          .map((json) => PaymentMethodModel.fromJson(json))
          .toList();
      
      return Right(methods);
    } catch (e) {
      return Left(CacheFailure(
        message: 'Failed to fetch payment methods',
        data: e.toString(),
      ));
    }
  }

  @override
  Future<Either<Failure, PaymentMethod>> addPaymentMethod({
    required PaymentMethodType type,
    required String name,
    String? cardNumber,
    String? cardHolderName,
    String? expiryDate,
    bool isDefault = false,
  }) async {
    try {
      // Get existing payment methods
      final methodsResult = await getPaymentMethods();
      
      return methodsResult.fold(
        (failure) => Left(failure),
        (methods) async {
          // Create new payment method
          final newMethod = PaymentMethodModel(
            id: DateTime.now().millisecondsSinceEpoch.toString(),
            type: type,
            name: name,
            cardNumber: cardNumber,
            cardHolderName: cardHolderName,
            expiryDate: expiryDate,
            isDefault: isDefault,
          );
          
          final List<PaymentMethodModel> updatedMethods = [];
          
          // If this is set as default, unset default for other methods
          for (final method in methods) {
            if (isDefault && method.isDefault) {
              updatedMethods.add(
                PaymentMethodModel.fromEntity(method.copyWith(isDefault: false)),
              );
            } else {
              updatedMethods.add(PaymentMethodModel.fromEntity(method));
            }
          }
          
          // Add the new method
          updatedMethods.add(newMethod);
          
          // Save to secure storage
          final methodsJson = json.encode(
            updatedMethods.map((m) => (m as PaymentMethodModel).toJson()).toList(),
          );
          await _secureStorage.write(key: _paymentMethodsKey, value: methodsJson);
          
          return Right(newMethod);
        },
      );
    } catch (e) {
      return Left(CacheFailure(
        message: 'Failed to add payment method',
        data: e.toString(),
      ));
    }
  }

  @override
  Future<Either<Failure, bool>> deletePaymentMethod({
    required String id,
  }) async {
    try {
      // Get existing payment methods
      final methodsResult = await getPaymentMethods();
      
      return methodsResult.fold(
        (failure) => Left(failure),
        (methods) async {
          // Remove the payment method
          final updatedMethods = methods
              .where((m) => m.id != id)
              .map((m) => PaymentMethodModel.fromEntity(m))
              .toList();
          
          // Check if we found and removed the method
          if (updatedMethods.length == methods.length) {
            return Left(NotFoundFailure(
              message: 'Payment method not found',
              data: {'methodId': id},
            ));
          }
          
          // Save to secure storage
          final methodsJson = json.encode(
            updatedMethods.map((m) => (m as PaymentMethodModel).toJson()).toList(),
          );
          await _secureStorage.write(key: _paymentMethodsKey, value: methodsJson);
          
          return const Right(true);
        },
      );
    } catch (e) {
      return Left(CacheFailure(
        message: 'Failed to delete payment method',
        data: e.toString(),
      ));
    }
  }

  @override
  Future<Either<Failure, Order>> placeOrder({
    required Cart cart,
    required Address shippingAddress,
    Address? billingAddress,
    required PaymentMethod paymentMethod,
    required double shippingCost,
  }) async {
    try {
      // In a real app, we would send the order to the server
      // and receive the created order with an ID
      
      // For this demo, create a local order
      final order = OrderModel(
        id: 'ORD-${DateTime.now().millisecondsSinceEpoch}',
        userId: 'current-user-id', // Would come from auth service
        items: cart.items.map((item) => item).toList(),
        shippingAddress: AddressModel.fromEntity(shippingAddress),
        billingAddress: billingAddress != null 
            ? AddressModel.fromEntity(billingAddress)
            : null,
        paymentMethod: PaymentMethodModel.fromEntity(paymentMethod),
        subtotal: cart.subtotal,
        tax: cart.tax,
        shippingCost: shippingCost,
        total: cart.total + shippingCost,
        status: OrderStatus.pending,
        createdAt: DateTime.now(),
        updatedAt: null,
      );
      
      // In a real app, we would also clear the cart after placing the order
      
      return Right(order);
    } catch (e) {
      return Left(ServerFailure(
        message: 'Failed to place order',
        data: e.toString(),
      ));
    }
  }

  @override
  Future<Either<Failure, double>> calculateShippingCost({
    required Cart cart,
    required Address shippingAddress,
  }) async {
    try {
      // In a real app, shipping cost would be calculated based on
      // items, shipping address, shipping method, etc.
      
      // For this demo, use a simple calculation
      double shippingCost = 0;
      
      // Base shipping cost
      shippingCost += 5.0;
      
      // Add $1 for each item
      shippingCost += cart.itemCount * 1.0;
      
      // Maximum shipping cost is $15
      if (shippingCost > 15.0) {
        shippingCost = 15.0;
      }
      
      // Free shipping for orders over $50
      if (cart.subtotal > 50.0) {
        shippingCost = 0;
      }
      
      return Right(shippingCost);
    } catch (e) {
      return Left(ServerFailure(
        message: 'Failed to calculate shipping cost',
        data: e.toString(),
      ));
    }
  }
}
