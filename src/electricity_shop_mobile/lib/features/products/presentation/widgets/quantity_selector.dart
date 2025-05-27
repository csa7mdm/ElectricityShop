import 'package:flutter/material.dart';

class QuantitySelector extends StatelessWidget {
  final int quantity;
  final Function(int) onChanged;

  const QuantitySelector({
    super.key,
    required this.quantity,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        const Text(
          'Quantity:',
          style: TextStyle(
            fontSize: 16,
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(width: 16),
        Container(
          decoration: BoxDecoration(
            border: Border.all(color: Colors.grey[300]!),
            borderRadius: BorderRadius.circular(4),
          ),
          child: Row(
            children: [
              // Decrease button
              IconButton(
                icon: const Icon(Icons.remove),
                onPressed: quantity > 1
                    ? () => onChanged(quantity - 1)
                    : null,
                iconSize: 20,
              ),
              // Quantity display
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8),
                child: Text(
                  quantity.toString(),
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
              // Increase button
              IconButton(
                icon: const Icon(Icons.add),
                onPressed: () => onChanged(quantity + 1),
                iconSize: 20,
              ),
            ],
          ),
        ),
      ],
    );
  }
}
