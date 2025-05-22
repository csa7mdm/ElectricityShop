import React, { useState } from 'react';
import { Outlet } from 'react-router-dom';
import { 
  AppBar, 
  Box, 
  Toolbar, 
  Typography, 
  Button, 
  IconButton, 
  Badge, 
  Drawer, 
  List, 
  ListItem, 
  ListItemIcon, 
  ListItemText,
  Container,
  useMediaQuery,
  useTheme,
  Divider
} from '@mui/material';
import { 
  Menu as MenuIcon, 
  ShoppingCart as CartIcon, 
  Person as PersonIcon, 
  Home as HomeIcon,
  Storefront as ProductsIcon,
  Dashboard as DashboardIcon,
  ExitToApp as LogoutIcon
} from '@mui/icons-material';
import { Link as RouterLink, useNavigate } from 'react-router-dom';
import { useAppSelector, useAppDispatch } from '../../hooks/useAppDispatch';
import { logoutAction } from '../../store/slices/authSlice';
import GlobalNotification from '../common/GlobalNotification';

const MainLayout: React.FC = () => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'));
  const [mobileOpen, setMobileOpen] = useState(false);
  
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  
  const { user, token } = useAppSelector((state) => state.auth);
  const { cart } = useAppSelector((state) => state.cart);
  
  const isAdmin = user?.role === 'Admin';
  const cartItemCount = cart?.items.length || 0;

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };

  const handleLogout = () => {
    dispatch(logoutAction());
    navigate('/');
  };

  const navItems = [
    { text: 'Home', icon: <HomeIcon />, path: '/' },
    { text: 'Products', icon: <ProductsIcon />, path: '/products' },
  ];

  const authItems = token
    ? [
        { text: 'Cart', icon: <CartIcon />, path: '/cart' },
        { text: 'My Account', icon: <PersonIcon />, path: '/account' },
        ...(isAdmin ? [{ text: 'Admin', icon: <DashboardIcon />, path: '/admin' }] : []),
        { text: 'Logout', icon: <LogoutIcon />, onClick: handleLogout },
      ]
    : [
        { text: 'Login', icon: <PersonIcon />, path: '/login' },
        { text: 'Register', icon: <PersonIcon />, path: '/register' },
      ];

  const drawer = (
    <Box onClick={handleDrawerToggle} sx={{ textAlign: 'center' }}>
      <Typography variant="h6" sx={{ my: 2 }}>
        ElectricityShop
      </Typography>
      <Divider />
      <List>
        {navItems.map((item) => (
          <ListItem 
            button 
            key={item.text} 
            component={RouterLink} 
            to={item.path}
          >
            <ListItemIcon>{item.icon}</ListItemIcon>
            <ListItemText primary={item.text} />
          </ListItem>
        ))}
        <Divider sx={{ my: 1 }} />
        {authItems.map((item) => (
          <ListItem 
            button 
            key={item.text} 
            component={item.path ? RouterLink : 'div'}
            to={item.path}
            onClick={item.onClick}
          >
            <ListItemIcon>{item.icon}</ListItemIcon>
            <ListItemText primary={item.text} />
          </ListItem>
        ))}
      </List>
    </Box>
  );

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      <AppBar position="static" component="nav" color="default" elevation={1}>
        <Toolbar>
          {isMobile && (
            <IconButton
              color="inherit"
              aria-label="open drawer"
              edge="start"
              onClick={handleDrawerToggle}
              sx={{ mr: 2 }}
            >
              <MenuIcon />
            </IconButton>
          )}
          
          <Typography
            variant="h6"
            component={RouterLink}
            to="/"
            sx={{ 
              flexGrow: 1, 
              color: 'inherit', 
              textDecoration: 'none',
              display: 'flex',
              alignItems: 'center'
            }}
          >
            ElectricityShop
          </Typography>
          
          {!isMobile && (
            <Box sx={{ display: 'flex', alignItems: 'center' }}>
              {navItems.map((item) => (
                <Button 
                  key={item.text} 
                  component={RouterLink} 
                  to={item.path} 
                  sx={{ color: 'inherit', mx: 1 }}
                >
                  {item.text}
                </Button>
              ))}
              
              {token && (
                <IconButton 
                  color="inherit" 
                  component={RouterLink} 
                  to="/cart"
                  sx={{ ml: 1 }}
                >
                  <Badge badgeContent={cartItemCount} color="primary">
                    <CartIcon />
                  </Badge>
                </IconButton>
              )}
              
              {authItems.map((item) => (
                <Button 
                  key={item.text} 
                  component={item.path ? RouterLink : 'button'}
                  to={item.path}
                  onClick={item.onClick}
                  sx={{ color: 'inherit', ml: 1 }}
                >
                  {item.text}
                </Button>
              ))}
            </Box>
          )}
        </Toolbar>
      </AppBar>
      
      <Box component="nav">
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={handleDrawerToggle}
          ModalProps={{
            keepMounted: true, // Better open performance on mobile.
          }}
          sx={{
            display: { xs: 'block', md: 'none' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: 240 },
          }}
        >
          {drawer}
        </Drawer>
      </Box>
      
      <Box component="main" sx={{ flexGrow: 1 }}>
        <Container maxWidth="lg" sx={{ mt: 2, mb: 4 }}>
          <Outlet />
        </Container>
      </Box>
      
      <Box
        component="footer"
        sx={{
          py: 3,
          px: 2,
          mt: 'auto',
          backgroundColor: (theme) => theme.palette.grey[200],
        }}
      >
        <Container maxWidth="lg">
          <Typography variant="body2" color="text.secondary" align="center">
            © {new Date().getFullYear()} ElectricityShop. All rights reserved.
          </Typography>
        </Container>
      </Box>
      
      <GlobalNotification />
    </Box>
  );
};

export default MainLayout;
