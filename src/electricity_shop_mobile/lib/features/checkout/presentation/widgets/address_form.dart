import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:form_builder_validators/form_builder_validators.dart';
import '../../domain/entities/address.dart';

/// Form for adding or editing an address
class AddressForm extends StatefulWidget {
  final Address? address;
  final Function(
    String fullName,
    String phone,
    String addressLine1,
    String? addressLine2,
    String city,
    String state,
    String postalCode,
    String country,
    bool isDefault,
  ) onSubmit;
  final VoidCallback onCancel;

  const AddressForm({
    super.key,
    this.address,
    required this.onSubmit,
    required this.onCancel,
  });

  @override
  State<AddressForm> createState() => _AddressFormState();
}

class _AddressFormState extends State<AddressForm> {
  final _formKey = GlobalKey<FormBuilderState>();
  bool _isDefault = false;

  @override
  void initState() {
    super.initState();
    
    // Set isDefault from existing address if editing
    if (widget.address != null) {
      _isDefault = widget.address!.isDefault;
    }
  }

  @override
  Widget build(BuildContext context) {
    return FormBuilder(
      key: _formKey,
      initialValue: widget.address != null
          ? {
              'fullName': widget.address!.fullName,
              'phone': widget.address!.phone,
              'addressLine1': widget.address!.addressLine1,
              'addressLine2': widget.address!.addressLine2 ?? '',
              'city': widget.address!.city,
              'state': widget.address!.state,
              'postalCode': widget.address!.postalCode,
              'country': widget.address!.country,
            }
          : {},
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Contact Information',
            style: TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 16),
          
          // Full Name
          FormBuilderTextField(
            name: 'fullName',
            decoration: const InputDecoration(
              labelText: 'Full Name',
              border: OutlineInputBorder(),
              prefixIcon: Icon(Icons.person),
            ),
            validator: FormBuilderValidators.compose([
              FormBuilderValidators.required(),
              FormBuilderValidators.maxLength(100),
            ]),
          ),
          const SizedBox(height: 16),
          
          // Phone
          FormBuilderTextField(
            name: 'phone',
            decoration: const InputDecoration(
              labelText: 'Phone Number',
              border: OutlineInputBorder(),
              prefixIcon: Icon(Icons.phone),
            ),
            keyboardType: TextInputType.phone,
            validator: FormBuilderValidators.compose([
              FormBuilderValidators.required(),
              FormBuilderValidators.minLength(10),
              FormBuilderValidators.maxLength(15),
            ]),
          ),
          const SizedBox(height: 24),
          
          const Text(
            'Address',
            style: TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 16),
          
          // Address Line 1
          FormBuilderTextField(
            name: 'addressLine1',
            decoration: const InputDecoration(
              labelText: 'Street Address',
              border: OutlineInputBorder(),
              prefixIcon: Icon(Icons.home),
            ),
            validator: FormBuilderValidators.compose([
              FormBuilderValidators.required(),
              FormBuilderValidators.maxLength(100),
            ]),
          ),
          const SizedBox(height: 16),
          
          // Address Line 2
          FormBuilderTextField(
            name: 'addressLine2',
            decoration: const InputDecoration(
              labelText: 'Apt, Suite, Building (optional)',
              border: OutlineInputBorder(),
              prefixIcon: Icon(Icons.apartment),
            ),
            validator: FormBuilderValidators.maxLength(100),
          ),
          const SizedBox(height: 16),
          
          // City and State in a row
          Row(
            children: [
              // City
              Expanded(
                child: FormBuilderTextField(
                  name: 'city',
                  decoration: const InputDecoration(
                    labelText: 'City',
                    border: OutlineInputBorder(),
                  ),
                  validator: FormBuilderValidators.compose([
                    FormBuilderValidators.required(),
                    FormBuilderValidators.maxLength(50),
                  ]),
                ),
              ),
              const SizedBox(width: 16),
              
              // State/Province
              Expanded(
                child: FormBuilderTextField(
                  name: 'state',
                  decoration: const InputDecoration(
                    labelText: 'State/Province',
                    border: OutlineInputBorder(),
                  ),
                  validator: FormBuilderValidators.compose([
                    FormBuilderValidators.required(),
                    FormBuilderValidators.maxLength(50),
                  ]),
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          
          // Postal Code and Country in a row
          Row(
            children: [
              // Postal Code
              Expanded(
                child: FormBuilderTextField(
                  name: 'postalCode',
                  decoration: const InputDecoration(
                    labelText: 'Postal/ZIP Code',
                    border: OutlineInputBorder(),
                  ),
                  keyboardType: TextInputType.number,
                  validator: FormBuilderValidators.compose([
                    FormBuilderValidators.required(),
                    FormBuilderValidators.maxLength(10),
                  ]),
                ),
              ),
              const SizedBox(width: 16),
              
              // Country
              Expanded(
                child: FormBuilderTextField(
                  name: 'country',
                  decoration: const InputDecoration(
                    labelText: 'Country',
                    border: OutlineInputBorder(),
                  ),
                  validator: FormBuilderValidators.compose([
                    FormBuilderValidators.required(),
                    FormBuilderValidators.maxLength(50),
                  ]),
                ),
              ),
            ],
          ),
          const SizedBox(height: 24),
          
          // Default address checkbox
          CheckboxListTile(
            title: const Text('Set as default address'),
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
                  child: Text(widget.address == null ? 'ADD ADDRESS' : 'UPDATE ADDRESS'),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  void _submitForm() {
    if (_formKey.currentState?.saveAndValidate() ?? false) {
      final formData = _formKey.currentState!.value;
      
      widget.onSubmit(
        formData['fullName'],
        formData['phone'],
        formData['addressLine1'],
        formData['addressLine2'],
        formData['city'],
        formData['state'],
        formData['postalCode'],
        formData['country'],
        _isDefault,
      );
    }
  }
}
