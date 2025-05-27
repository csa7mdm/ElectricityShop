## Node.js Compatibility

If you're using Node.js version 17 or higher, you might encounter OpenSSL-related errors. This is because newer Node.js versions use a newer version of OpenSSL that's not compatible with some webpack configurations in older versions of Create React App.

The scripts in package.json have been updated to include the `--openssl-legacy-provider` flag which resolves this issue:

```json
"scripts": {
  "start": "set NODE_OPTIONS=--openssl-legacy-provider && react-scripts start",
  "build": "set NODE_OPTIONS=--openssl-legacy-provider && react-scripts build",
  "test": "set NODE_OPTIONS=--openssl-legacy-provider && react-scripts test"
}
```

Alternatively, you can:

1. Set the environment variable globally:
   ```powershell
   $env:NODE_OPTIONS="--openssl-legacy-provider"
   ```

2. Downgrade to Node.js 16.x which doesn't have this issue.

# ElectricityShop Installation Guide

This guide helps you set up the ElectricityShop e-commerce frontend application correctly, resolving dependency conflicts and addressing common issues.

## Initial Setup

1. **Clean the existing installation:**
   ```powershell
   # Navigate to the project directory
   cd E:\Projects\ElectricityShop\src\electricity-shop-web
   
   # Remove existing dependencies and lock files
   Remove-Item -Recurse -Force node_modules
   Remove-Item package-lock.json
   ```

2. **Install dependencies:**
   ```powershell
   # Use npm with the legacy-peer-deps flag to avoid peer dependency issues
   npm install --legacy-peer-deps
   ```

3. **Start the development server:**
   ```powershell
   npm start
   ```

## Dependency Conflict Solutions

The following changes have been made to resolve dependency conflicts:

1. **Removed direct ESLint dependency** and added it to the overrides section in package.json
2. **Added specific versions for Babel plugins** in the overrides section to prevent deprecation warnings
3. **Updated react-scripts** to a more compatible version (4.0.3)
4. **Created .npmrc** with legacy-peer-deps setting to avoid peer dependency conflicts

## Handling EISDIR Errors

If you encounter EISDIR errors during installation:

1. Ensure you're using the `--legacy-peer-deps` flag when installing packages
2. Consider running npm with administrator privileges if permission issues occur
3. Try installing packages one at a time if bulk installation fails

## Troubleshooting TypeScript Issues

If you encounter TypeScript-related errors:

1. You may need to add an empty `src/react-app-env.d.ts` file with the content:
   ```typescript
   /// <reference types="react-scripts" />
   ```

2. Ensure TypeScript version is compatible with the React Scripts version (4.0.3)

## Core-js Warnings

The warnings about core-js being deprecated are related to dependencies of dependencies and can be safely ignored for now. These don't affect the functionality of the application.

## Installing without Cleaning

If you prefer not to delete the existing node_modules directory:

```powershell
npm install --legacy-peer-deps --force
```

This might help bypass some of the dependency conflicts, but it's generally recommended to start with a clean installation.

## If All Else Fails

If you continue to face installation issues:

1. Try using Yarn instead of npm:
   ```powershell
   # Install Yarn if not already installed
   npm install -g yarn
   
   # Install dependencies using Yarn
   yarn install
   
   # Start the application
   yarn start
   ```

2. Consider creating a new React TypeScript project and merging in the ElectricityShop components:
   ```powershell
   npx create-react-app new-electricity-shop --template typescript
   ```
