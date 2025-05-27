import React from 'react';
import { Backdrop, CircularProgress, Box, Typography } from '@mui/material';

interface LoadingSpinnerProps {
  isOpen: boolean;
  message?: string;
}

/**
 * A loading spinner component with an optional message
 */
const LoadingSpinner: React.FC<LoadingSpinnerProps> = ({ isOpen, message }) => {
  return (
    <Backdrop
      sx={{ 
        color: '#fff', 
        zIndex: (theme) => theme.zIndex.drawer + 1,
        flexDirection: 'column'
      }}
      open={isOpen}
    >
      <CircularProgress color="inherit" size={60} thickness={4} />
      {message && (
        <Box mt={2}>
          <Typography variant="h6">{message}</Typography>
        </Box>
      )}
    </Backdrop>
  );
};

export default LoadingSpinner;
