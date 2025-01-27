import { useForm, SubmitHandler } from 'react-hook-form';
import httpClient from '../../utils/httpClient';
import { UserDTO } from '../../contracts/user/user';
import { useNavigate } from 'react-router-dom';
import { useState } from 'react';

const Register = () => {
  const {
    register,
    handleSubmit,
    formState: { errors }
  } = useForm<UserDTO>();
  const navigate = useNavigate();
  const [apiError, setApiError] = useState<string | null>(null);

  const onSubmit: SubmitHandler<UserDTO> = async (data) => {
    try {
      const response = await httpClient.post('/api/auth/register', data);
      console.log('Registration successful', response.data);
      setApiError(null);
      navigate('/login');
    } catch (err: any) {
      if (err.response && err.response.data) {
        setApiError(err.response.data.message);
      } else {
        setApiError('An unexpected error occurred. Please try again.');
      }
    }
  };

  return (
    <div className="flex flex-col items-center justify-center">
      <h2 className="text-2xl font-bold mb-4">Register</h2>
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
          <label className="block mb-1">Email</label>
          <input
            type="email"
            className="w-full border rounded px-3 py-2"
            {...register('email', {
              required: 'Email is required',
              pattern: {
                value: /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/,
                message: 'Invalid email address'
              }
            })}
          />
          {errors.email && <p className="text-red-500">{errors.email.message}</p>}
        </div>
        <div>
          <label className="block mb-1">Password</label>
          <input
            type="password"
            className="w-full border rounded px-3 py-2"
            {...register('password', {
              required: 'Password is required',
              minLength: { value: 6, message: 'Password must be at least 6 characters' }
            })}
          />
          {errors.password && <p className="text-red-500">{errors.password.message}</p>}
        </div>
        {apiError && <p className="text-red-500 text-center">{apiError}</p>}
        <button type="submit" className="w-full bg-blue-500 text-white py-2 rounded">
          Register
        </button>
      </form>
    </div>
  );
};

export default Register;
