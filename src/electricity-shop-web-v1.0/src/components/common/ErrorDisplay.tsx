import React from 'react';
import { Alert, Box, Button, Typography } from '@mui/material';

interface ErrorDisplayProps {
  message: string;
  onRetry?: () => void;
}

const ErrorDisplay: React.FC<ErrorDisplayProps> = ({ message, onRetry }) => {
  return (
    <Box sx={{ my: 4, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
      <Alert severity="error" sx={{ mb: 2, width: '100%', maxWidth: 600 }}>
        {message}
      </Alert>
      <Typography variant="body1" sx={{ mb: 2 }}>
        We encountered a problem while loading the data.
      </Typography>
      {onRetry && (
        <Button variant="contained" color="primary" onClick={onRetry}>
          Try Again
        </Button>
      )}
    </Box>
  );
};

export default ErrorDisplay;
