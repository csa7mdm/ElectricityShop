import 'package:equatable/equatable.dart';

/// Entity class for a shipping or billing address
class Address extends Equatable {
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

  const Address({
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

  /// Create a copy of this Address with provided parameters
  Address copyWith({
    String? id,
    String? fullName,
    String? phone,
    String? addressLine1,
    String? addressLine2,
    String? city,
    String? state,
    String? postalCode,
    String? country,
    bool? isDefault,
  }) {
    return Address(
      id: id ?? this.id,
      fullName: fullName ?? this.fullName,
      phone: phone ?? this.phone,
      addressLine1: addressLine1 ?? this.addressLine1,
      addressLine2: addressLine2 ?? this.addressLine2,
      city: city ?? this.city,
      state: state ?? this.state,
      postalCode: postalCode ?? this.postalCode,
      country: country ?? this.country,
      isDefault: isDefault ?? this.isDefault,
    );
  }

  /// Get formatted address as a single string
  String get formattedAddress {
    final buffer = StringBuffer();
    buffer.write(addressLine1);
    
    if (addressLine2 != null && addressLine2!.isNotEmpty) {
      buffer.write(', $addressLine2');
    }
    
    buffer.write('\n$city, $state $postalCode');
    buffer.write('\n$country');
    
    return buffer.toString();
  }

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
