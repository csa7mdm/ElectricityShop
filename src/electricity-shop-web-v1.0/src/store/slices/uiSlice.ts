import { createSlice, PayloadAction } from '@reduxjs/toolkit';

interface UiState {
  isLoading: boolean;
  notification: {
    message: string;
    type: 'success' | 'error' | 'info' | 'warning' | null;
    open: boolean;
  };
}

const initialState: UiState = {
  isLoading: false,
  notification: {
    message: '',
    type: null,
    open: false,
  },
};

const uiSlice = createSlice({
  name: 'ui',
  initialState,
  reducers: {
    setLoading: (state, action: PayloadAction<boolean>) => {
      state.isLoading = action.payload;
    },
    showNotification: (state, action: PayloadAction<{ message: string; type: 'success' | 'error' | 'info' | 'warning' }>) => {
      state.notification = {
        message: action.payload.message,
        type: action.payload.type,
        open: true,
      };
    },
    hideNotification: (state) => {
      state.notification.open = false;
    },
  },
});

export const { setLoading, showNotification, hideNotification } = uiSlice.actions;
export default uiSlice.reducer;
