import 'package:form_builder_validators/form_builder_validators.dart';
import 'package:flutter/material.dart';

/// Custom validators for form fields
class AppValidators {
  AppValidators._();

  /// Validates email format
  static String? email(String? value, BuildContext context) {
    final validator = FormBuilderValidators.email(
      errorText: 'Please enter a valid email address',
    );
    return validator(context, value);
  }

  /// Validates required fields
  static String? required(String? value, BuildContext context) {
    final validator = FormBuilderValidators.required(
      errorText: 'This field is required',
    );
    return validator(context, value);
  }

  /// Validates minimum password length
  static String? password(String? value, BuildContext context) {
    if (value == null || value.isEmpty) {
      return 'Password is required';
    }
    if (value.length < 8) {
      return 'Password must be at least 8 characters long';
    }
    // Check for at least one uppercase letter
    if (!value.contains(RegExp(r'[A-Z]'))) {
      return 'Password must contain at least one uppercase letter';
    }
    // Check for at least one digit
    if (!value.contains(RegExp(r'[0-9]'))) {
      return 'Password must contain at least one number';
    }
    return null;
  }

  /// Validates that passwords match
  static String? passwordsMatch(String? value, String? passwordToMatch, BuildContext context) {
    if (value != passwordToMatch) {
      return 'Passwords do not match';
    }
    return null;
  }

  /// Validates credit card number format
  static String? creditCardNumber(String? value, BuildContext context) {
    if (value == null || value.isEmpty) {
      return 'Credit card number is required';
    }
    
    // Remove spaces and dashes
    final cleanValue = value.replaceAll(RegExp(r'[\s-]'), '');
    
    // Check if it contains only digits
    if (!RegExp(r'^[0-9]+$').hasMatch(cleanValue)) {
      return 'Card number can only contain digits';
    }
    
    // Check card number length (most cards are 16 digits, but some are 13-19)
    if (cleanValue.length < 13 || cleanValue.length > 19) {
      return 'Invalid card number length';
    }
    
    // Implement Luhn algorithm for credit card validation
    int sum = 0;
    bool alternate = false;
    for (int i = cleanValue.length - 1; i >= 0; i--) {
      int digit = int.parse(cleanValue[i]);
      
      if (alternate) {
        digit *= 2;
        if (digit > 9) {
          digit -= 9;
        }
      }
      
      sum += digit;
      alternate = !alternate;
    }
    
    if (sum % 10 != 0) {
      return 'Invalid credit card number';
    }
    
    return null;
  }

  /// Validates credit card expiry date format (MM/YY)
  static String? expiryDate(String? value, BuildContext context) {
    if (value == null || value.isEmpty) {
      return 'Expiry date is required';
    }
    
    // Validate format MM/YY
    if (!RegExp(r'^(0[1-9]|1[0-2])\/([0-9]{2})$').hasMatch(value)) {
      return 'Use format MM/YY';
    }
    
    final parts = value.split('/');
    final month = int.parse(parts[0]);
    final year = int.parse('20${parts[1]}');
    
    final now = DateTime.now();
    final currentYear = now.year;
    final currentMonth = now.month;
    
    // Check if card is expired
    if (year < currentYear || (year == currentYear && month < currentMonth)) {
      return 'Card has expired';
    }
    
    // Check if date is too far in the future (most cards are valid for 5 years)
    if (year > currentYear + 10) {
      return 'Invalid expiry date';
    }
    
    return null;
  }

  /// Validates CVV format (3-4 digits)
  static String? cvv(String? value, BuildContext context) {
    if (value == null || value.isEmpty) {
      return 'CVV is required';
    }
    
    if (!RegExp(r'^[0-9]{3,4}$').hasMatch(value)) {
      return 'CVV must be 3-4 digits';
    }
    
    return null;
  }

  /// Validates zip/postal code format
  static String? zipCode(String? value, BuildContext context) {
    if (value == null || value.isEmpty) {
      return 'ZIP code is required';
    }
    
    // Basic validation - can be extended for specific country formats
    if (value.length < 4 || value.length > 10) {
      return 'Please enter a valid ZIP code';
    }
    
    return null;
  }
}
