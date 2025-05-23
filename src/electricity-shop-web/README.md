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

This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app).

## Available Scripts

In the project directory, you can run:

### `npm start`

Runs the app in the development mode.\
Open [http://localhost:3000](http://localhost:3000) to view it in your browser.

The page will reload when you make changes.\
You may also see any lint errors in the console.

### `npm test`

Launches the test runner in the interactive watch mode.\
See the section about [running tests](https://facebook.github.io/create-react-app/docs/running-tests) for more information.

### `npm run build`

Builds the app for production to the `build` folder.\
It correctly bundles React in production mode and optimizes the build for the best performance.

The build is minified and the filenames include the hashes.\
Your app is ready to be deployed!

See the section about [deployment](https://facebook.github.io/create-react-app/docs/deployment) for more information.

### `npm run eject`

**Note: this is a one-way operation. Once you `eject`, you can't go back!**

If you aren't satisfied with the build tool and configuration choices, you can `eject` at any time. This command will remove the single build dependency from your project.

Instead, it will copy all the configuration files and the transitive dependencies (webpack, Babel, ESLint, etc) right into your project so you have full control over them. All of the commands except `eject` will still work, but they will point to the copied scripts so you can tweak them. At this point you're on your own.

You don't have to ever use `eject`. The curated feature set is suitable for small and middle deployments, and you shouldn't feel obligated to use this feature. However we understand that this tool wouldn't be useful if you couldn't customize it when you are ready for it.

## Learn More

You can learn more in the [Create React App documentation](https://facebook.github.io/create-react-app/docs/getting-started).

To learn React, check out the [React documentation](https://reactjs.org/).

### Code Splitting

This section has moved here: [https://facebook.github.io/create-react-app/docs/code-splitting](https://facebook.github.io/create-react-app/docs/code-splitting)

### Analyzing the Bundle Size

This section has moved here: [https://facebook.github.io/create-react-app/docs/analyzing-the-bundle-size](https://facebook.github.io/create-react-app/docs/analyzing-the-bundle-size)

### Making a Progressive Web App

This section has moved here: [https://facebook.github.io/create-react-app/docs/making-a-progressive-web-app](https://facebook.github.io/create-react-app/docs/making-a-progressive-web-app)

### Advanced Configuration

This section has moved here: [https://facebook.github.io/create-react-app/docs/advanced-configuration](https://facebook.github.io/create-react-app/docs/advanced-configuration)

### Deployment

This section has moved here: [https://facebook.github.io/create-react-app/docs/deployment](https://facebook.github.io/create-react-app/docs/deployment)

### `npm run build` fails to minify

This section has moved here: [https://facebook.github.io/create-react-app/docs/troubleshooting#npm-run-build-fails-to-minify](https://facebook.github.io/create-react-app/docs/troubleshooting#npm-run-build-fails-to-minify)
