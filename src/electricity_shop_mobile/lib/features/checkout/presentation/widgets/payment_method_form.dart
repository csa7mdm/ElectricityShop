import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:form_builder_validators/form_builder_validators.dart';
import '../../../../core/utils/formatters.dart';
import '../../domain/entities/payment_method.dart';

/// Form for adding a new payment method
class PaymentMethodForm extends StatefulWidget {
  final Function(
    PaymentMethodType type,
    String name,
    String? cardNumber,
    String? cardHolderName,
    String? expiryDate,
    bool isDefault,
  ) onSubmit;
  final VoidCallback onCancel;

  const PaymentMethodForm({
    super.key,
    required this.onSubmit,
    required this.onCancel,
  });

  @override
  State<PaymentMethodForm> createState() => _PaymentMethodFormState();
}

class _PaymentMethodFormState extends State<PaymentMethodForm> {
  final _formKey = GlobalKey<FormBuilderState>();
  PaymentMethodType _selectedType = PaymentMethodType.creditCard;
  bool _isDefault = false;

  @override
  Widget build(BuildContext context) {
    return FormBuilder(
      key: _formKey,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Payment Method',
            style: TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 16),
          
          // Payment method type selector
          FormBuilderRadioGroup<PaymentMethodType>(
            name: 'paymentType',
            initialValue: _selectedType,
            decoration: const InputDecoration(
              border: InputBorder.none,
              contentPadding: EdgeInsets.zero,
            ),
            options: const [
              FormBuilderFieldOption(
                value: PaymentMethodType.creditCard,
                child: Text('Credit/Debit Card'),
              ),
              FormBuilderFieldOption(
                value: PaymentMethodType.paypal,
                child: Text('PayPal'),
              ),
              FormBuilderFieldOption(
                value: PaymentMethodType.applePay,
                child: Text('Apple Pay'),
              ),
              FormBuilderFieldOption(
                value: PaymentMethodType.googlePay,
                child: Text('Google Pay'),
              ),
              FormBuilderFieldOption(
                value: PaymentMethodType.cashOnDelivery,
                child: Text('Cash on Delivery'),
              ),
            ],
            onChanged: (value) {
              if (value != null) {
                setState(() {
                  _selectedType = value;
                });
              }
            },
          ),
          const SizedBox(height: 16),
          
          // Credit card details (only shown for credit card type)
          if (_selectedType == PaymentMethodType.creditCard) ...[
            const Divider(),
            const SizedBox(height: 16),
            
            // Card name
            FormBuilderTextField(
              name: 'cardName',
              decoration: const InputDecoration(
                labelText: 'Name on Card',
                border: OutlineInputBorder(),
                prefixIcon: Icon(Icons.person),
              ),
              validator: FormBuilderValidators.compose([
                FormBuilderValidators.required(),
                FormBuilderValidators.maxLength(100),
              ]),
            ),
            const SizedBox(height: 16),
            
            // Card number
            FormBuilderTextField(
              name: 'cardNumber',
              decoration: const InputDecoration(
                labelText: 'Card Number',
                border: OutlineInputBorder(),
                prefixIcon: Icon(Icons.credit_card),
              ),
              keyboardType: TextInputType.number,
              inputFormatters: [
                CreditCardNumberFormatter(),
              ],
              validator: FormBuilderValidators.compose([
                FormBuilderValidators.required(),
                FormBuilderValidators.minLength(19), // 16 digits + 3 spaces
                FormBuilderValidators.maxLength(19),
              ]),
            ),
            const SizedBox(height: 16),
            
            // Expiry date and CVV in a row
            Row(
              children: [
                // Expiry date
                Expanded(
                  child: FormBuilderTextField(
                    name: 'expiryDate',
                    decoration: const InputDecoration(
                      labelText: 'Expiry Date (MM/YY)',
                      border: OutlineInputBorder(),
                    ),
                    keyboardType: TextInputType.number,
                    inputFormatters: [
                      ExpiryDateFormatter(),
                    ],
                    validator: FormBuilderValidators.compose([
                      FormBuilderValidators.required(),
                      FormBuilderValidators.minLength(5), // MM/YY
                      FormBuilderValidators.maxLength(5),
                    ]),
                  ),
                ),
                const SizedBox(width: 16),
                
                // CVV
                Expanded(
                  child: FormBuilderTextField(
                    name: 'cvv',
                    decoration: const InputDecoration(
                      labelText: 'CVV',
                      border: OutlineInputBorder(),
                    ),
                    keyboardType: TextInputType.number,
                    obscureText: true,
                    inputFormatters: [
                      DigitOnlyFormatter(),
                    ],
                    validator: FormBuilderValidators.compose([
                      FormBuilderValidators.required(),
                      FormBuilderValidators.minLength(3),
                      FormBuilderValidators.maxLength(4),
                      FormBuilderValidators.numeric(),
                    ]),
                  ),
                ),
              ],
            ),
          ] else ...[
            // For non-credit card methods, just a nickname field
            FormBuilderTextField(
              name: 'nickname',
              decoration: InputDecoration(
                labelText: 'Nickname for this ${_getPaymentMethodName(_selectedType)}',
                border: const OutlineInputBorder(),
                prefixIcon: const Icon(Icons.label),
              ),
              validator: FormBuilderValidators.compose([
                FormBuilderValidators.required(),
                FormBuilderValidators.maxLength(50),
              ]),
            ),
          ],
          
          const SizedBox(height: 24),
          
          // Default payment method checkbox
          CheckboxListTile(
            title: const Text('Set as default payment method'),
            value: _isDefault,
            onChanged: (value) {
              setState(() {
                _isDefault = value ?? false;
              });
            },
            contentPadding: EdgeInsets.zero,
            controlAffinity: ListTileControlAffinity.leading,
          ),
          const SizedBox(height: 24),
          
          // Submit and Cancel buttons
          Row(
            children: [
              Expanded(
                child: OutlinedButton(
                  onPressed: widget.onCancel,
                  style: OutlinedButton.styleFrom(
                    padding: const EdgeInsets.symmetric(vertical: 16),
                  ),
                  child: const Text('CANCEL'),
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: ElevatedButton(
                  onPressed: _submitForm,
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.blue,
                    foregroundColor: Colors.white,
                    padding: const EdgeInsets.symmetric(vertical: 16),
                  ),
                  child: const Text('ADD PAYMENT METHOD'),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  String _getPaymentMethodName(PaymentMethodType type) {
    switch (type) {
      case PaymentMethodType.creditCard:
        return 'Credit/Debit Card';
      case PaymentMethodType.paypal:
        return 'PayPal';
      case PaymentMethodType.applePay:
        return 'Apple Pay';
      case PaymentMethodType.googlePay:
        return 'Google Pay';
      case PaymentMethodType.cashOnDelivery:
        return 'Cash on Delivery';
      default:
        return 'Payment Method';
    }
  }

  void _submitForm() {
    if (_formKey.currentState?.saveAndValidate() ?? false) {
      final formData = _formKey.currentState!.value;
      
      String name;
      String? cardNumber;
      String? cardHolderName;
      String? expiryDate;
      
      if (_selectedType == PaymentMethodType.creditCard) {
        name = 'Card ending in ${formData['cardNumber'].toString().substring(formData['cardNumber'].toString().length - 4)}';
        cardNumber = formData['cardNumber'].toString().replaceAll(' ', '');
        cardHolderName = formData['cardName'];
        expiryDate = formData['expiryDate'];
      } else {
        name = formData['nickname'];
        cardNumber = null;
        cardHolderName = null;
        expiryDate = null;
      }
      
      widget.onSubmit(
        _selectedType,
        name,
        cardNumber,
        cardHolderName,
        expiryDate,
        _isDefault,
      );
    }
  }
}
