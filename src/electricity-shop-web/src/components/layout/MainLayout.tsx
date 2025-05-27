import React from 'react';
import { Outlet } from 'react-router-dom';
import { Box, Container, CssBaseline } from '@mui/material';
import Header from './Header';
import Footer from './Footer';

/**
 * Main layout component that wraps all pages
 * Includes Header, main content area, and Footer
 */
const MainLayout: React.FC = () => {
  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        minHeight: '100vh',
      }}
    >
      <CssBaseline />
      <Header />
      <Container component="main" sx={{ flexGrow: 1, py: 4 }} maxWidth="lg">
        <Outlet />
      </Container>
      <Footer />
    </Box>
  );
};

export default MainLayout;
