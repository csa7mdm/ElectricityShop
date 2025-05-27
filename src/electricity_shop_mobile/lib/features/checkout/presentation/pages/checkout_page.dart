import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../app/routes.dart';
import '../../../cart/presentation/bloc/cart_bloc.dart';
import '../../../cart/presentation/bloc/cart_event.dart';
import '../../../cart/presentation/bloc/cart_state.dart';
import '../../../cart/domain/entities/cart.dart';
import '../../domain/entities/address.dart';
import '../../domain/entities/payment_method.dart';
import '../bloc/checkout_bloc.dart';
import '../bloc/checkout_event.dart';
import '../bloc/checkout_state.dart';
import '../widgets/address_card.dart';
import '../widgets/address_form.dart';
import '../widgets/order_summary.dart';
import '../widgets/payment_method_card.dart';
import '../widgets/payment_method_form.dart';
import 'order_confirmation_page.dart';

class CheckoutPage extends StatefulWidget {
  static const String routeName = AppRoutes.checkout;

  const CheckoutPage({super.key});

  @override
  State<CheckoutPage> createState() => _CheckoutPageState();
}

class _CheckoutPageState extends State<CheckoutPage> {
  // Track the current step in the checkout process
  int _currentStep = 0;
  
  // Shipping cost
  double _shippingCost = 0.0;
  
  // Track if we're showing address/payment forms
  bool _showAddressForm = false;
  bool _showPaymentForm = false;
  
  // Address being edited (if any)
  Address? _editAddress;
  
  @override
  void initState() {
    super.initState();
    // Load addresses and payment methods when page loads
    context.read<CheckoutBloc>().add(const LoadAddressesEvent());
    context.read<CheckoutBloc>().add(const LoadPaymentMethodsEvent());
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Checkout'),
      ),
      body: BlocConsumer<CheckoutBloc, CheckoutState>(
        listener: _checkoutBlocListener,
        builder: (context, checkoutState) {
          return BlocBuilder<CartBloc, CartState>(
            builder: (context, cartState) {
              if (cartState is CartLoaded) {
                return _buildCheckoutContent(cartState.cart, checkoutState);
              } else {
                return const Center(
                  child: CircularProgressIndicator(),
                );
              }
            },
          );
        },
      ),
    );
  }

  // Listen for events from the checkout bloc
  void _checkoutBlocListener(BuildContext context, CheckoutState state) {
    // Listen for shipping cost calculation results
    if (state is ShippingCostCalculated) {
      setState(() {
        _shippingCost = state.shippingCost;
      });
    }
    // Listen for order placed result
    else if (state is OrderPlaced) {
      // Clear the cart
      context.read<CartBloc>().add(const ClearCartEvent());
      
      // Navigate to order confirmation
      Navigator.pushReplacementNamed(
        context,
        OrderConfirmationPage.routeName,
        arguments: state.order,
      );
    }
    // Listen for errors
    else if (state is CheckoutError) {
      _showErrorSnackBar(state.message);
    }
  }
  
  void _showErrorSnackBar(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: Colors.red,
        duration: const Duration(seconds: 3),
      ),
    );
  }

  void _showDeleteConfirmationDialog(
    String title,
    String message,
    VoidCallback onConfirm,
  ) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(title),
        content: Text(message),
        actions: [
          TextButton(
            onPressed: () {
              Navigator.of(context).pop();
            },
            child: const Text('CANCEL'),
          ),
          TextButton(
            onPressed: () {
              Navigator.of(context).pop();
              onConfirm();
            },
            style: TextButton.styleFrom(
              foregroundColor: Colors.red,
            ),
            child: const Text('DELETE'),
          ),
        ],
      ),
    );
  }

  void _handleStepContinue(Cart cart) {
    // For step 0 (shipping address), validate that an address is selected
    if (_currentStep == 0) {
      final state = context.read<CheckoutBloc>().state;
      if (state is AddressesLoaded && state.selectedAddress != null) {
        // Calculate shipping cost based on cart and selected address
        context.read<CheckoutBloc>().add(
              CalculateShippingCostEvent(
                cart: cart,
                shippingAddress: state.selectedAddress!,
              ),
            );
        
        setState(() {
          _currentStep++;
        });
      } else {
        _showErrorSnackBar('Please select or add a shipping address');
      }
    }
    // For step 1 (payment method), validate that a payment method is selected
    else if (_currentStep == 1) {
      final state = context.read<CheckoutBloc>().state;
      if (state is PaymentMethodsLoaded && state.selectedMethod != null) {
        setState(() {
          _currentStep++;
        });
      } else {
        _showErrorSnackBar('Please select or add a payment method');
      }
    }
  }

  void _placeOrder(Cart cart) {
    // Get the shipping address
    Address? shippingAddress;
    if (context.read<CheckoutBloc>().state is AddressesLoaded) {
      final addressState = context.read<CheckoutBloc>().state as AddressesLoaded;
      shippingAddress = addressState.selectedAddress;
    }
    
    // Get the payment method
    PaymentMethod? paymentMethod;
    if (context.read<CheckoutBloc>().state is PaymentMethodsLoaded) {
      final paymentState = context.read<CheckoutBloc>().state as PaymentMethodsLoaded;
      paymentMethod = paymentState.selectedMethod;
    }
    
    // Validate we have everything needed for the order
    if (shippingAddress == null) {
      _showErrorSnackBar('Please select a shipping address');
      setState(() {
        _currentStep = 0;
      });
      return;
    }
    
    if (paymentMethod == null) {
      _showErrorSnackBar('Please select a payment method');
      setState(() {
        _currentStep = 1;
      });
      return;
    }
    
    // Place the order
    context.read<CheckoutBloc>().add(
          PlaceOrderEvent(
            cart: cart,
            shippingAddress: shippingAddress,
            paymentMethod: paymentMethod,
            shippingCost: _shippingCost,
          ),
        );
  }

  Widget _buildCheckoutContent(Cart cart, CheckoutState checkoutState) {
    // If we're showing forms, display them instead of stepper
    if (_showAddressForm) {
      return _buildAddressForm();
    } else if (_showPaymentForm) {
      return _buildPaymentForm();
    }
    
    // Show the checkout stepper
    return _buildCheckoutStepper(cart, checkoutState);
  }

  Widget _buildAddressForm() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: AddressForm(
        address: _editAddress,
        onSubmit: _handleAddressSubmit,
        onCancel: () {
          setState(() {
            _showAddressForm = false;
            _editAddress = null;
          });
        },
      ),
    );
  }

  void _handleAddressSubmit(
    String fullName,
    String phone,
    String addressLine1,
    String? addressLine2,
    String city,
    String state,
    String postalCode,
    String country,
    bool isDefault,
  ) {
    if (_editAddress != null) {
      // Update existing address
      context.read<CheckoutBloc>().add(
            UpdateAddressEvent(
              id: _editAddress!.id,
              fullName: fullName,
              phone: phone,
              addressLine1: addressLine1,
              addressLine2: addressLine2,
              city: city,
              state: state,
              postalCode: postalCode,
              country: country,
              isDefault: isDefault,
            ),
          );
    } else {
      // Add new address
      context.read<CheckoutBloc>().add(
            AddAddressEvent(
              fullName: fullName,
              phone: phone,
              addressLine1: addressLine1,
              addressLine2: addressLine2,
              city: city,
              state: state,
              postalCode: postalCode,
              country: country,
              isDefault: isDefault,
            ),
          );
    }
    
    setState(() {
      _showAddressForm = false;
      _editAddress = null;
    });
  }

  Widget _buildPaymentForm() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: PaymentMethodForm(
        onSubmit: _handlePaymentMethodSubmit,
        onCancel: () {
          setState(() {
            _showPaymentForm = false;
          });
        },
      ),
    );
  }

  void _handlePaymentMethodSubmit(
    PaymentMethodType type,
    String name,
    String? cardNumber,
    String? cardHolderName,
    String? expiryDate,
    bool isDefault,
  ) {
    // Add new payment method
    context.read<CheckoutBloc>().add(
          AddPaymentMethodEvent(
            type: type,
            name: name,
            cardNumber: cardNumber,
            cardHolderName: cardHolderName,
            expiryDate: expiryDate,
            isDefault: isDefault,
          ),
        );
    
    setState(() {
      _showPaymentForm = false;
    });
  }

  Widget _buildCheckoutStepper(Cart cart, CheckoutState checkoutState) {
    return Stepper(
      currentStep: _currentStep,
      onStepContinue: () {
        if (_currentStep < 2) {
          _handleStepContinue(cart);
        } else {
          _placeOrder(cart);
        }
      },
      onStepCancel: () {
        if (_currentStep > 0) {
          setState(() {
            _currentStep--;
          });
        } else {
          Navigator.of(context).pop();
        }
      },
      controlsBuilder: (context, details) {
        return _buildStepperControls(details);
      },
      steps: [
        // Step 1: Shipping address
        Step(
          title: const Text('Shipping Address'),
          content: _buildShippingAddressStep(checkoutState),
          isActive: _currentStep >= 0,
          state: _currentStep > 0 ? StepState.complete : StepState.indexed,
        ),
        // Step 2: Payment method
        Step(
          title: const Text('Payment Method'),
          content: _buildPaymentMethodStep(checkoutState),
          isActive: _currentStep >= 1,
          state: _currentStep > 1 ? StepState.complete : StepState.indexed,
        ),
        // Step 3: Review order
        Step(
          title: const Text('Review Order'),
          content: _buildOrderReviewStep(cart, checkoutState),
          isActive: _currentStep >= 2,
          state: _currentStep > 2 ? StepState.complete : StepState.indexed,
        ),
      ],
    );
  }
