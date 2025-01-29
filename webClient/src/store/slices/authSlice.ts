import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { UserProfileDTO } from '../../contracts/user/user';

interface AuthState {
  token: string | null;
  user: UserProfileDTO | null;
}

const getInitialState = (): AuthState => {
  const token = localStorage.getItem('token');
  const userString = localStorage.getItem('user');
  const user = userString ? JSON.parse(userString) : null;

  return {
    token,
    user
  };
};

const initialState: AuthState = getInitialState();

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
      localStorage.setItem('user', JSON.stringify(action.payload.user));
    },
    logOut: (state) => {
      state.token = null;
      state.user = null;
      localStorage.removeItem('token');
      localStorage.removeItem('user');
    }
  }
});

export const { setCredentials, logOut } = authSlice.actions;
export default authSlice.reducer;
