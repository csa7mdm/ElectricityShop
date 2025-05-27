# Quick Start with Vite

Vite is a modern, fast build tool that works well with newer Node.js versions. Follow these steps to quickly migrate your ElectricityShop project to Vite:

## Step 1: Create a new Vite project

```powershell
# Navigate to the parent directory
cd E:\Projects\ElectricityShop\src

# Create a new Vite project with React and TypeScript
npm create vite@latest electricity-shop-vite -- --template react-ts

# Navigate to the new project
cd electricity-shop-vite

# Install base dependencies
npm install
```

## Step 2: Install required dependencies

```powershell
# Install all the dependencies needed for the ElectricityShop project
npm install @emotion/react @emotion/styled @mui/icons-material @mui/material @reduxjs/toolkit @hookform/resolvers axios jwt-decode react-hook-form react-redux react-router-dom web-vitals yup
```

## Step 3: Copy your existing components and configuration

```powershell
# Create the directory structure
mkdir -p src/assets src/components/common src/components/layout src/components/features src/config src/hooks src/pages/admin src/services src/store src/types src/utils

# Copy the source files (adjust paths as needed)
Copy-Item -Recurse -Force "E:\Projects\ElectricityShop\src\electricity-shop-web\src\components\*" ".\src\components\"
Copy-Item -Recurse -Force "E:\Projects\ElectricityShop\src\electricity-shop-web\src\config\*" ".\src\config\"
Copy-Item -Recurse -Force "E:\Projects\ElectricityShop\src\electricity-shop-web\src\hooks\*" ".\src\hooks\"
Copy-Item -Recurse -Force "E:\Projects\ElectricityShop\src\electricity-shop-web\src\pages\*" ".\src\pages\"
Copy-Item -Recurse -Force "E:\Projects\ElectricityShop\src\electricity-shop-web\src\store\*" ".\src\store\"
Copy-Item -Recurse -Force "E:\Projects\ElectricityShop\src\electricity-shop-web\src\types\*" ".\src\types\"
Copy-Item -Recurse -Force "E:\Projects\ElectricityShop\src\electricity-shop-web\src\utils\*" ".\src\utils\"

# Copy App.tsx and modify for Vite if needed
Copy-Item "E:\Projects\ElectricityShop\src\electricity-shop-web\src\App.tsx" ".\src\"

# Copy CSS
Copy-Item "E:\Projects\ElectricityShop\src\electricity-shop-web\src\index.css" ".\src\"
```

## Step 4: Update the main.tsx file to include your Redux store and router

Edit the src/main.tsx file to look like this:

```tsx
import React from 'react'
import ReactDOM from 'react-dom/client'
import { Provider } from 'react-redux'
import { BrowserRouter } from 'react-router-dom'
import CssBaseline from '@mui/material/CssBaseline'
import { ThemeProvider } from '@mui/material/styles'

import App from './App'
import { store } from './store'
import theme from './config/theme'
import './index.css'

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <Provider store={store}>
      <BrowserRouter>
        <ThemeProvider theme={theme}>
          <CssBaseline />
          <App />
        </ThemeProvider>
      </BrowserRouter>
    </Provider>
  </React.StrictMode>,
)
```

## Step 5: Start the development server

```powershell
npm run dev
```

## Additional Tips for Vite Migration

1. **Importing CSS**: In Vite, CSS imports work slightly differently. Make sure all CSS imports are properly set up.

2. **Environment Variables**: Vite uses `import.meta.env` instead of `process.env`. Update any environment variable references.

3. **Asset Imports**: Vite handles asset imports differently from Create React App. You might need to adjust image imports.

4. **Path Aliases**: If you want to use path aliases (e.g., `@components/Button`), you'll need to configure them in `vite.config.ts`:

```ts
// vite.config.ts
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, 'src'),
      'components': path.resolve(__dirname, 'src/components'),
      'pages': path.resolve(__dirname, 'src/pages'),
      // Add more aliases as needed
    }
  }
})
```

This approach should provide a more modern, faster development experience without the compatibility issues you're encountering with Create React App.
