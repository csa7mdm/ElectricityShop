# Setting Up a New Project

If you continue to experience issues with the current setup, you can create a fresh React TypeScript project and migrate the components. Here are the steps:

## Step 1: Create a new React TypeScript project

```powershell
# Navigate to the parent directory
cd E:\Projects\ElectricityShop\src

# Create a new React TypeScript project
npx create-react-app electricity-shop-new --template typescript

# Navigate to the new project
cd electricity-shop-new
```

## Step 2: Install the required dependencies

```powershell
npm install @emotion/react @emotion/styled @hookform/resolvers @mui/icons-material @mui/material @reduxjs/toolkit axios jwt-decode react-hook-form react-redux react-router-dom web-vitals yup --legacy-peer-deps
```

## Step 3: Copy the source files from the existing project

```powershell
# Create the necessary directories
mkdir -p src/assets src/components/common src/components/layout src/components/features src/config src/hooks src/pages src/services src/store src/types src/utils

# Copy the source files from the old project to the new one
Copy-Item -Recurse "E:\Projects\ElectricityShop\src\electricity-shop-web\src\assets\*" ".\src\assets\"
Copy-Item -Recurse "E:\Projects\ElectricityShop\src\electricity-shop-web\src\components\*" ".\src\components\"
Copy-Item -Recurse "E:\Projects\ElectricityShop\src\electricity-shop-web\src\config\*" ".\src\config\"
Copy-Item -Recurse "E:\Projects\ElectricityShop\src\electricity-shop-web\src\hooks\*" ".\src\hooks\"
Copy-Item -Recurse "E:\Projects\ElectricityShop\src\electricity-shop-web\src\pages\*" ".\src\pages\"
Copy-Item -Recurse "E:\Projects\ElectricityShop\src\electricity-shop-web\src\services\*" ".\src\services\"
Copy-Item -Recurse "E:\Projects\ElectricityShop\src\electricity-shop-web\src\store\*" ".\src\store\"
Copy-Item -Recurse "E:\Projects\ElectricityShop\src\electricity-shop-web\src\types\*" ".\src\types\"
Copy-Item -Recurse "E:\Projects\ElectricityShop\src\electricity-shop-web\src\utils\*" ".\src\utils\"

# Copy the main files
Copy-Item "E:\Projects\ElectricityShop\src\electricity-shop-web\src\App.tsx" ".\src\"
Copy-Item "E:\Projects\ElectricityShop\src\electricity-shop-web\src\index.tsx" ".\src\"
Copy-Item "E:\Projects\ElectricityShop\src\electricity-shop-web\src\index.css" ".\src\"
Copy-Item "E:\Projects\ElectricityShop\src\electricity-shop-web\src\reportWebVitals.ts" ".\src\"
```

## Step 4: Start the new project

```powershell
npm start
```

This approach creates a fresh Create React App project with the latest configurations and dependencies, which should avoid the ESLint and Node.js compatibility issues you're experiencing.

## Alternative: Create a Vite-based project

Vite is a modern build tool that's faster and has fewer compatibility issues with newer Node.js versions. You can create a Vite-based React TypeScript project instead:

```powershell
# Navigate to the parent directory
cd E:\Projects\ElectricityShop\src

# Create a new Vite React TypeScript project
npm create vite@latest electricity-shop-vite -- --template react-ts

# Navigate to the new project
cd electricity-shop-vite

# Install dependencies
npm install

# Install the required additional dependencies
npm install @emotion/react @emotion/styled @hookform/resolvers @mui/icons-material @mui/material @reduxjs/toolkit axios jwt-decode react-hook-form react-redux react-router-dom web-vitals yup
```

Then follow Step 3 from above to copy the source files, adjusting as needed for the Vite project structure.
