import 'package:flutter_bloc/flutter_bloc.dart';
import '../../domain/repositories/checkout_repository.dart';
import 'checkout_event.dart';
import 'checkout_state.dart';

/// BLoC for handling checkout operations
class CheckoutBloc extends Bloc<CheckoutEvent, CheckoutState> {
  final CheckoutRepository _checkoutRepository;

  CheckoutBloc(this._checkoutRepository) : super(const CheckoutInitial()) {
    on<LoadAddressesEvent>(_onLoadAddresses);
    on<AddAddressEvent>(_onAddAddress);
    on<UpdateAddressEvent>(_onUpdateAddress);
    on<DeleteAddressEvent>(_onDeleteAddress);
    on<LoadPaymentMethodsEvent>(_onLoadPaymentMethods);
    on<AddPaymentMethodEvent>(_onAddPaymentMethod);
    on<DeletePaymentMethodEvent>(_onDeletePaymentMethod);
    on<SelectShippingAddressEvent>(_onSelectShippingAddress);
    on<SelectPaymentMethodEvent>(_onSelectPaymentMethod);
    on<CalculateShippingCostEvent>(_onCalculateShippingCost);
    on<PlaceOrderEvent>(_onPlaceOrder);
  }

  /// Handle loading addresses
  Future<void> _onLoadAddresses(
    LoadAddressesEvent event,
    Emitter<CheckoutState> emit,
  ) async {
    emit(const CheckoutLoading());
    
    final result = await _checkoutRepository.getAddresses();
    
    result.fold(
      (failure) => emit(CheckoutError(message: failure.message, data: failure.data)),
      (addresses) {
        // Select default address if available
        final defaultAddress = addresses.isNotEmpty
            ? addresses.firstWhere(
                (address) => address.isDefault,
                orElse: () => addresses.first,
              )
            : null;
        
        emit(AddressesLoaded(
          addresses: addresses,
          selectedAddress: defaultAddress,
        ));
      },
    );
  }

  /// Handle adding a new address
  Future<void> _onAddAddress(
    AddAddressEvent event,
    Emitter<CheckoutState> emit,
  ) async {
    emit(const CheckoutLoading());
    
    final result = await _checkoutRepository.addAddress(
      fullName: event.fullName,
      phone: event.phone,
      addressLine1: event.addressLine1,
      addressLine2: event.addressLine2,
      city: event.city,
      state: event.state,
      postalCode: event.postalCode,
      country: event.country,
      isDefault: event.isDefault,
    );
    
    result.fold(
      (failure) => emit(CheckoutError(message: failure.message, data: failure.data)),
      (address) {
        emit(AddressAdded(address: address));
        
        // Refresh addresses list
        add(const LoadAddressesEvent());
      },
    );
  }

  /// Handle updating an address
  Future<void> _onUpdateAddress(
    UpdateAddressEvent event,
    Emitter<CheckoutState> emit,
  ) async {
    emit(const CheckoutLoading());
    
    final result = await _checkoutRepository.updateAddress(
      id: event.id,
      fullName: event.fullName,
      phone: event.phone,
      addressLine1: event.addressLine1,
      addressLine2: event.addressLine2,
      city: event.city,
      state: event.state,
      postalCode: event.postalCode,
      country: event.country,
      isDefault: event.isDefault,
    );
    
    result.fold(
      (failure) => emit(CheckoutError(message: failure.message, data: failure.data)),
      (address) {
        emit(AddressUpdated(address: address));
        
        // Refresh addresses list
        add(const LoadAddressesEvent());
      },
    );
  }

  /// Handle deleting an address
  Future<void> _onDeleteAddress(
    DeleteAddressEvent event,
    Emitter<CheckoutState> emit,
  ) async {
    emit(const CheckoutLoading());
    
    final result = await _checkoutRepository.deleteAddress(
      id: event.id,
    );
    
    result.fold(
      (failure) => emit(CheckoutError(message: failure.message, data: failure.data)),
      (success) {
        emit(AddressDeleted(id: event.id));
        
        // Refresh addresses list
        add(const LoadAddressesEvent());
      },
    );
  }

  /// Handle loading payment methods
  Future<void> _onLoadPaymentMethods(
    LoadPaymentMethodsEvent event,
    Emitter<CheckoutState> emit,
  ) async {
    emit(const CheckoutLoading());
    
    final result = await _checkoutRepository.getPaymentMethods();
    
    result.fold(
      (failure) => emit(CheckoutError(message: failure.message, data: failure.data)),
      (paymentMethods) {
        // Select default payment method if available
        final defaultMethod = paymentMethods.isNotEmpty
            ? paymentMethods.firstWhere(
                (method) => method.isDefault,
                orElse: () => paymentMethods.first,
              )
            : null;
        
        emit(PaymentMethodsLoaded(
          paymentMethods: paymentMethods,
          selectedMethod: defaultMethod,
        ));
      },
    );
  }

  /// Handle adding a new payment method
  Future<void> _onAddPaymentMethod(
    AddPaymentMethodEvent event,
    Emitter<CheckoutState> emit,
  ) async {
    emit(const CheckoutLoading());
    
    final result = await _checkoutRepository.addPaymentMethod(
      type: event.type,
      name: event.name,
      cardNumber: event.cardNumber,
      cardHolderName: event.cardHolderName,
      expiryDate: event.expiryDate,
      isDefault: event.isDefault,
    );
    
    result.fold(
      (failure) => emit(CheckoutError(message: failure.message, data: failure.data)),
      (paymentMethod) {
        emit(PaymentMethodAdded(paymentMethod: paymentMethod));
        
        // Refresh payment methods list
        add(const LoadPaymentMethodsEvent());
      },
    );
  }

  /// Handle deleting a payment method
  Future<void> _onDeletePaymentMethod(
    DeletePaymentMethodEvent event,
    Emitter<CheckoutState> emit,
  ) async {
    emit(const CheckoutLoading());
    
    final result = await _checkoutRepository.deletePaymentMethod(
      id: event.id,
    );
    
    result.fold(
      (failure) => emit(CheckoutError(message: failure.message, data: failure.data)),
      (success) {
        emit(PaymentMethodDeleted(id: event.id));
        
        // Refresh payment methods list
        add(const LoadPaymentMethodsEvent());
      },
    );
  }

  /// Handle selecting a shipping address
  void _onSelectShippingAddress(
    SelectShippingAddressEvent event,
    Emitter<CheckoutState> emit,
  ) {
    if (state is AddressesLoaded) {
      final currentState = state as AddressesLoaded;
      emit(currentState.copyWith(
        selectedAddress: event.address,
      ));
    }
  }

  /// Handle selecting a payment method
  void _onSelectPaymentMethod(
    SelectPaymentMethodEvent event,
    Emitter<CheckoutState> emit,
  ) {
    if (state is PaymentMethodsLoaded) {
      final currentState = state as PaymentMethodsLoaded;
      emit(currentState.copyWith(
        selectedMethod: event.paymentMethod,
      ));
    }
  }

  /// Handle calculating shipping cost
  Future<void> _onCalculateShippingCost(
    CalculateShippingCostEvent event,
    Emitter<CheckoutState> emit,
  ) async {
    emit(const CheckoutLoading());
    
    final result = await _checkoutRepository.calculateShippingCost(
      cart: event.cart,
      shippingAddress: event.shippingAddress,
    );
    
    result.fold(
      (failure) => emit(CheckoutError(message: failure.message, data: failure.data)),
      (shippingCost) => emit(ShippingCostCalculated(shippingCost: shippingCost)),
    );
  }

  /// Handle placing an order
  Future<void> _onPlaceOrder(
    PlaceOrderEvent event,
    Emitter<CheckoutState> emit,
  ) async {
    emit(const CheckoutLoading());
    
    final result = await _checkoutRepository.placeOrder(
      cart: event.cart,
      shippingAddress: event.shippingAddress,
      billingAddress: event.billingAddress,
      paymentMethod: event.paymentMethod,
      shippingCost: event.shippingCost,
    );
    
    result.fold(
      (failure) => emit(CheckoutError(message: failure.message, data: failure.data)),
      (order) => emit(OrderPlaced(order: order)),
    );
  }
}
