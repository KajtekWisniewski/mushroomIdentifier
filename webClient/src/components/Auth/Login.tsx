import { useForm, SubmitHandler } from 'react-hook-form';
import { useDispatch } from 'react-redux';
import { setCredentials } from '../../store/slices/authSlice';
import httpClient from '../../utils/httpClient';
import { UserLoginDTO, LoginResponseDTO } from '../../contracts/user/user';
import { useNavigate } from 'react-router-dom';
import { useState } from 'react';

const Login = () => {
  const {
    register,
    handleSubmit,
    formState: { errors }
  } = useForm<UserLoginDTO>();
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const [apiError, setApiError] = useState<string | null>(null);

  const onSubmit: SubmitHandler<UserLoginDTO> = async (data) => {
    try {
      const response = await httpClient.post<LoginResponseDTO>('/api/auth/login', data);
      const { token, user } = response.data;
      dispatch(setCredentials({ token, user }));
      setApiError(null);
      navigate('/profile');
    } catch (err: any) {
      if (err.response && err.response.data) {
        setApiError(err.response.data);
      } else {
        setApiError('An unexpected error occurred. Please try again.');
      }
    }
  };

  return (
    <div className="flex flex-col items-center justify-center">
      <h2 className="text-2xl font-bold mb-4">Login</h2>
      <form onSubmit={handleSubmit(onSubmit)} className="w-full max-w-sm space-y-4">
        <div>
          <label className="block mb-1">Username</label>
          <input
            type="text"
            className="w-full border rounded px-3 py-2"
            {...register('username', { required: 'Username is required' })}
          />
          {errors.username && <p className="text-red-500">{errors.username.message}</p>}
        </div>
        <div>
          <label className="block mb-1">Password</label>
          <input
            type="password"
            className="w-full border rounded px-3 py-2"
            {...register('password', { required: 'Password is required' })}
          />
          {errors.password && <p className="text-red-500">{errors.password.message}</p>}
        </div>
        {apiError && <p className="text-red-500 text-center">{apiError}</p>}
        <button type="submit" className="w-full py-2 rounded">
          Login
        </button>
      </form>
    </div>
  );
};

export default Login;
