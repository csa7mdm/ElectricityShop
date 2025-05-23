# ElectricityShop - Flutter Mobile App

This project is the Flutter-based mobile application for the ElectricityShop e-commerce platform.

## Project Overview

This mobile application aims to provide a native and responsive user experience for customers to browse products, manage their shopping cart, place orders, and manage their accounts on the ElectricityShop platform using their iOS or Android devices. It interacts with the backend API services provided by the ElectricityShop main application.

## Backend API Integration Status

The backend API has been significantly developed, and the following features are now supported by available endpoints. Flutter development can proceed with integrating these functionalities.

### Product Features
- **Fetch Product List:** APIs are ready to deliver a list of products. Basic search by product name/description and filtering by CategoryId is supported.
- **Fetch Product Details:** API for retrieving detailed information for a specific product is available.
- **Product Management (Admin):** APIs for creating, updating, and deleting products are available (admin-only, typically not part of a customer-facing mobile app but good to be aware of).

### Shopping Cart Features
- **View Cart:** API to get the current user's shopping cart is functional.
- **Add to Cart:** API to add items to the user's cart is functional.
- **Update Cart Item:** API to update the quantity of an item in the cart (including setting quantity to 0 to remove) is functional.
- **Remove Cart Item:** API to remove an item directly from the cart is functional.
- **Clear Cart:** API to clear all items from the user's cart is functional.

### Order Features
- **Create Order:** API to create a new order from the items in the cart is ready.
- **View Orders:** APIs to fetch a user's order history and details of a specific order are available.
- **Cancel Order:** API to cancel an order (if it's in a cancelable state) is functional.
- **Process Payment:** API for processing payments for an order is available (currently simulated on the backend).

### Authentication
- User registration, login, and JWT token refresh functionalities are available via backend APIs.

**For detailed API endpoint documentation, please refer to the main project `README.md` in the root directory of the ElectricityShop repository.**

## Development Focus / Next Steps

With the backend APIs in place for core e-commerce functionalities, Flutter development can focus on:
- Building UI screens and widgets for product listing, product details, and product filtering/search.
- Implementing the shopping cart view and user interactions (add, update, remove items).
- Developing the multi-step checkout process UI, including order summary and payment form integration.
- Creating UI for users to view their order history and individual order details.
- Implementing user registration and login screens, and managing user sessions (tokens).
- State management for cart, user session, and product data (e.g., using Provider, BLoC/Cubit, Riverpod).
- Integrating with the backend services for all the features listed above using appropriate HTTP client packages (e.g., `http`, `dio`).

---

# Getting Started (Original Flutter Readme)

This project is a starting point for a Flutter application.

A few resources to get you started if this is your first Flutter project:

- [Lab: Write your first Flutter app](https://docs.flutter.dev/get-started/codelab)
- [Cookbook: Useful Flutter samples](https://docs.flutter.dev/cookbook)

For help getting started with Flutter development, view the
[online documentation](https://docs.flutter.dev/), which offers tutorials,
samples, guidance on mobile development, and a full API reference.
