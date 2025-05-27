import React from 'react';
import { Link as RouterLink } from 'react-router-dom';
import {
  Box,
  Container,
  Grid,
  Link,
  Typography,
  Divider,
  IconButton,
  List,
  ListItem,
} from '@mui/material';
import {
  Facebook as FacebookIcon,
  Twitter as TwitterIcon,
  Instagram as InstagramIcon,
  LinkedIn as LinkedInIcon,
  ElectricBolt as ElectricBoltIcon,
} from '@mui/icons-material';

const Footer: React.FC = () => {
  const currentYear = new Date().getFullYear();

  return (
    <Box
      component="footer"
      sx={{
        py: 6,
        px: 2,
        mt: 'auto',
        backgroundColor: (theme) =>
          theme.palette.mode === 'light'
            ? theme.palette.grey[200]
            : theme.palette.grey[800],
      }}
    >
      <Container maxWidth="lg">
        <Grid container spacing={4} justifyContent="space-between">
          <Grid item xs={12} sm={4} md={3}>
            <Box display="flex" alignItems="center" mb={2}>
              <ElectricBoltIcon sx={{ mr: 1, color: 'primary.main' }} />
              <Typography variant="h6" color="text.primary" gutterBottom>
                ElectricityShop
              </Typography>
            </Box>
            <Typography variant="body2" color="text.secondary" paragraph>
              Your one-stop shop for all electrical products and accessories.
              Quality products, expert advice, and fast delivery.
            </Typography>
            <Box sx={{ mt: 2 }}>
              <IconButton color="primary" aria-label="facebook">
                <FacebookIcon />
              </IconButton>
              <IconButton color="primary" aria-label="twitter">
                <TwitterIcon />
              </IconButton>
              <IconButton color="primary" aria-label="instagram">
                <InstagramIcon />
              </IconButton>
              <IconButton color="primary" aria-label="linkedin">
                <LinkedInIcon />
              </IconButton>
            </Box>
          </Grid>

          <Grid item xs={12} sm={4} md={3}>
            <Typography variant="h6" color="text.primary" gutterBottom>
              Quick Links
            </Typography>
            <List dense>
              <ListItem disablePadding>
                <Link component={RouterLink} to="/" color="inherit" underline="hover">
                  Home
                </Link>
              </ListItem>
              <ListItem disablePadding>
                <Link component={RouterLink} to="/products" color="inherit" underline="hover">
                  Products
                </Link>
              </ListItem>
              <ListItem disablePadding>
                <Link component={RouterLink} to="/cart" color="inherit" underline="hover">
                  My Cart
                </Link>
              </ListItem>
              <ListItem disablePadding>
                <Link component={RouterLink} to="/profile" color="inherit" underline="hover">
                  My Account
                </Link>
              </ListItem>
            </List>
          </Grid>

          <Grid item xs={12} sm={4} md={3}>
            <Typography variant="h6" color="text.primary" gutterBottom>
              Customer Service
            </Typography>
            <List dense>
              <ListItem disablePadding>
                <Link href="#" color="inherit" underline="hover">
                  Contact Us
                </Link>
              </ListItem>
              <ListItem disablePadding>
                <Link href="#" color="inherit" underline="hover">
                  FAQs
                </Link>
              </ListItem>
              <ListItem disablePadding>
                <Link href="#" color="inherit" underline="hover">
                  Shipping Policy
                </Link>
              </ListItem>
              <ListItem disablePadding>
                <Link href="#" color="inherit" underline="hover">
                  Return Policy
                </Link>
              </ListItem>
            </List>
          </Grid>

          <Grid item xs={12} sm={4} md={3}>
            <Typography variant="h6" color="text.primary" gutterBottom>
              Contact Information
            </Typography>
            <Typography variant="body2" color="text.secondary">
              123 Electric Avenue
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Circuit City, CA 91234
            </Typography>
            <Typography variant="body2" color="text.secondary" mt={1}>
              Email: info@electricityshop.com
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Phone: (123) 456-7890
            </Typography>
          </Grid>
        </Grid>

        <Divider sx={{ my: 4 }} />

        <Box textAlign="center">
          <Typography variant="body2" color="text.secondary">
            © {currentYear} ElectricityShop. All rights reserved.
          </Typography>
          <Box mt={1}>
            <Link href="#" color="inherit" sx={{ mx: 1 }}>
              Privacy Policy
            </Link>
            <Link href="#" color="inherit" sx={{ mx: 1 }}>
              Terms of Service
            </Link>
            <Link href="#" color="inherit" sx={{ mx: 1 }}>
              Cookies Policy
            </Link>
          </Box>
        </Box>
      </Container>
    </Box>
  );
};

export default Footer;
