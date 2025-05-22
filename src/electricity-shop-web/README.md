# ElectricityShop - E-commerce Frontend Application

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
