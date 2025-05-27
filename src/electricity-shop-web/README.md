# ElectricityShop - React Frontend

This project is the React-based frontend for the ElectricityShop e-commerce application. It was bootstrapped with [Create React App](https://github.com/facebook/create-react-app).

## Project Overview

This frontend application aims to provide a user-friendly interface for customers to browse products, manage their shopping cart, place orders, and manage their accounts on the ElectricityShop platform. It interacts with the backend API services provided by the ElectricityShop main application.

## Backend API Integration Status

The backend API has been significantly developed, and the following features are now supported by available endpoints. Frontend development can proceed with integrating these functionalities.

### Product Features
- **Fetch Product List:** APIs are ready to deliver a list of products. Basic search by product name/description and filtering by CategoryId is supported.
- **Fetch Product Details:** API for retrieving detailed information for a specific product is available.
- **Product Management (Admin):** APIs for creating, updating, and deleting products are available (admin-only).

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

**For detailed API endpoint documentation, please refer to the main project `README.md` in the root directory.**

## Development Focus / Next Steps

With the backend APIs in place for core e-commerce functionalities, frontend development can focus on:
- Building UI components for product listing, product details, and product filtering/search.
- Implementing the shopping cart view and user interactions (add, update, remove items).
- Developing the multi-step checkout process UI, including order summary and payment form.
- Creating UI for users to view their order history and individual order details.
- Implementing user registration and login forms.
- State management for cart, user session, and product data.
- Integrating with the backend services for all the features listed above.

---

# Getting Started with Create React App (Original Readme)

ElectricityShop is a modern React e-commerce application for electrical products, built with TypeScript and Material-UI.

## Features

- **User Authentication**
  - JWT-based authentication
  - User registration and login
  - Protected routes
  - Role-based access control (Admin/User)

- **Product Catalog**
  - Responsive product grid
  - Filtering & searching
  - Pagination
  - Detailed product view

- **Shopping Cart**
  - Real-time cart management
  - Add/remove items
  - Update quantities
  - Price calculations

- **Checkout Flow**
  - Multi-step checkout process
  - Address management
  - Payment integration

- **User Dashboard**
  - Order history
  - Profile management
  - Address management

- **Admin Panel**
  - Product management (CRUD)
  - Order management
  - User management
  - Analytics dashboard

## Tech Stack

- **Frontend:**
  - React 18+ with TypeScript
  - Material-UI v5
  - Redux Toolkit
  - React Router v6
  - React Hook Form + Yup
  - Axios

- **Testing:**
  - Jest
  - React Testing Library

## Project Structure

```
src/
  ├── assets/           # Static assets (images, icons, etc.)
  ├── components/
  │   ├── common/       # Reusable UI components
  │   ├── layout/       # Layout components (Header, Footer, etc.)
  │   └── features/     # Feature-specific components
  ├── config/           # Configuration files
  ├── hooks/            # Custom React hooks
  ├── pages/            # Page components
  ├── services/         # API services
  ├── store/            # Redux store setup and slices
  ├── types/            # TypeScript type definitions
  └── utils/            # Utility functions
```

## Getting Started

### Prerequisites

- Node.js (v16+)
- npm or yarn

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/electricity-shop.git
   cd electricity-shop
   ```

2. Install dependencies:
   ```bash
   npm install
   # or
   yarn install
   ```

3. Start the development server:
   ```bash
   npm start
   # or
   yarn start
   ```

4. Open [http://localhost:3000](http://localhost:3000) in your browser.

### Environment Variables

Create a `.env` file in the root directory with the following variables:

```
REACT_APP_API_URL=http://your-backend-api-url
```

## API Integration

The application integrates with the following API endpoints:

- Authentication: `/api/auth/`
- Products: `/api/products/`
- Cart: `/api/cart/`
- Orders: `/api/orders/`
- Users: `/api/users/`

## Building for Production

```bash
npm run build
# or
yarn build
```

This creates a `build` folder with production-ready files.

## Testing

```bash
# Run tests
npm test
# or
yarn test

# Run tests with coverage
npm run coverage
# or
yarn coverage
```

## Performance Targets

- First Contentful Paint < 2s
- Time to Interactive < 3.5s
- Lighthouse score > 90

## Security Measures

- CSRF protection
- XSS prevention
- Secure token storage
- Input sanitization

## Contributing

1. Fork the repository
2. Create your feature branch: `git checkout -b feature/my-feature`
3. Commit your changes: `git commit -m 'Add my feature'`
4. Push to the branch: `git push origin feature/my-feature`
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.
