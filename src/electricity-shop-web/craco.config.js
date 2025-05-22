module.exports = {
  // CRACO configuration options
  webpack: {
    configure: {
      // Webpack configuration customizations
      resolve: {
        fallback: {
          // Polyfills for Node.js core modules
          // Add any necessary fallbacks here if warnings appear
          // Example: "crypto": require.resolve("crypto-browserify")
        },
      },
    },
  },
  // You can add plugins here as needed
  plugins: [],
  // TypeScript configuration
  typescript: {
    enableTypeChecking: true,
  },
};
