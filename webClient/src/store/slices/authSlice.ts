import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { UserProfileDTO } from '../../contracts/user/user';

interface AuthState {
  token: string | null;
  user: UserProfileDTO | null;
}

const initialState: AuthState = {
  token: localStorage.getItem('token') || null,
  user: null
};

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setCredentials: (
      state,
      action: PayloadAction<{ token: string; user: UserProfileDTO }>
    ) => {
      state.token = action.payload.token;
      state.user = action.payload.user;
      localStorage.setItem('token', action.payload.token);
    },
    logOut: (state) => {
      state.token = null;
      state.user = null;
      localStorage.removeItem('token');
    }
  }
});

export const { setCredentials, logOut } = authSlice.actions;
export default authSlice.reducer;
