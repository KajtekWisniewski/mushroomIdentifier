import { useState, useEffect } from 'react';
import { createPortal } from 'react-dom';
import { X } from 'lucide-react';
import { useForm, SubmitHandler } from 'react-hook-form';
import { useDispatch } from 'react-redux';
import { setCredentials } from '../../../store/slices/authSlice';
import httpClient from '../../../utils/httpClient';
import { UserLoginDTO, LoginResponseDTO, UserDTO } from '../../../contracts/user/user';
import clsx from 'clsx';

interface AuthModalProps {
  isOpen: boolean;
  onClose: () => void;
  initialTab?: 'login' | 'register';
}

const AuthModal = ({ isOpen, onClose, initialTab = 'login' }: AuthModalProps) => {
  const [activeTab, setActiveTab] = useState<'login' | 'register'>(initialTab);
  const [apiError, setApiError] = useState<string | null>(null);
  const dispatch = useDispatch();

  useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = 'hidden';
    } else {
      document.body.style.overflow = 'unset';
    }
    return () => {
      document.body.style.overflow = 'unset';
    };
  }, [isOpen]);

  const LoginForm = () => {
    const {
      register,
      handleSubmit,
      formState: { errors }
    } = useForm<UserLoginDTO>();

    const onSubmit: SubmitHandler<UserLoginDTO> = async (data) => {
      try {
        const response = await httpClient.post<LoginResponseDTO>(
          '/api/auth/login',
          data
        );
        const { token, user } = response.data;
        dispatch(setCredentials({ token, user }));
        setApiError(null);
        onClose();
      } catch (err: any) {
        if (err.response && err.response.data) {
          setApiError(err.response.data);
        } else {
          setApiError('An unexpected error occurred. Please try again.');
        }
      }
    };

    return (
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 min-h-[300px]">
        <div>
          <label className="block mb-1 text-sm font-medium">Username</label>
          <input
            type="text"
            className="w-full px-3 py-2 border border-beige-300 rounded-lg focus:ring-2 focus:ring-beige-500"
            {...register('username', { required: 'Username is required' })}
          />
          {errors.username && (
            <p className="text-red-500 text-sm mt-1">{errors.username.message}</p>
          )}
        </div>
        <div>
          <label className="block mb-1 text-sm font-medium">Password</label>
          <input
            type="password"
            className="w-full px-3 py-2 border border-beige-300 rounded-lg focus:ring-2 focus:ring-beige-500"
            {...register('password', { required: 'Password is required' })}
          />
          {errors.password && (
            <p className="text-red-500 text-sm mt-1">{errors.password.message}</p>
          )}
        </div>
        <button
          type="submit"
          className="w-full py-2 bg-beige-700 text-white rounded-lg hover:bg-beige-800 transition-colors mt-20.5"
        >
          Login
        </button>
      </form>
    );
  };

  const RegisterForm = () => {
    const {
      register,
      handleSubmit,
      formState: { errors }
    } = useForm<UserDTO>();

    const onSubmit: SubmitHandler<UserDTO> = async (data) => {
      try {
        await httpClient.post('/api/auth/register', data);
        setApiError(null);
        setActiveTab('login');
      } catch (err: any) {
        if (err.response && err.response.data) {
          setApiError(err.response.data.message);
        } else {
          setApiError('An unexpected error occurred. Please try again.');
        }
      }
    };

    return (
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 min-h-[300px]">
        <div>
          <label className="block mb-1 text-sm font-medium">Username</label>
          <input
            type="text"
            className="w-full px-3 py-2 border border-beige-300 rounded-lg focus:ring-2 focus:ring-beige-500"
            {...register('username', { required: 'Username is required' })}
          />
          {errors.username && (
            <p className="text-red-500 text-sm mt-1">{errors.username.message}</p>
          )}
        </div>
        <div>
          <label className="block mb-1 text-sm font-medium">Email</label>
          <input
            type="email"
            className="w-full px-3 py-2 border border-beige-300 rounded-lg focus:ring-2 focus:ring-beige-500"
            {...register('email', {
              required: 'Email is required',
              pattern: {
                value: /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/,
                message: 'Invalid email address'
              }
            })}
          />
          {errors.email && (
            <p className="text-red-500 text-sm mt-1">{errors.email.message}</p>
          )}
        </div>
        <div>
          <label className="block mb-1 text-sm font-medium">Password</label>
          <input
            type="password"
            className="w-full px-3 py-2 border border-beige-300 rounded-lg focus:ring-2 focus:ring-beige-500"
            {...register('password', {
              required: 'Password is required',
              minLength: { value: 6, message: 'Password must be at least 6 characters' }
            })}
          />
          {errors.password && (
            <p className="text-red-500 text-sm mt-1">{errors.password.message}</p>
          )}
        </div>
        <button
          type="submit"
          className="w-full py-2 bg-beige-700 text-white rounded-lg hover:bg-beige-800 transition-colors"
        >
          Register
        </button>
      </form>
    );
  };

  if (!isOpen) return null;

  return createPortal(
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="absolute inset-0 bg-black/50 backdrop-blur-sm transition-all duration-300" />

      <div className="relative w-full max-w-md p-6 bg-white rounded-xl shadow-xl m-4 animate-fade-in">
        <button
          onClick={onClose}
          className="absolute top-4 right-4 text-gray-500 hover:text-gray-700 -mt-16 -mr-4"
        >
          <X className="w-5 h-5" />
        </button>

        <div className="flex border-b border-beige-200 mb-6">
          <button
            onClick={() => setActiveTab('login')}
            className={clsx(
              'flex-1 py-3 font-medium border-b-2 transition-colors',
              activeTab === 'login'
                ? 'border-beige-700 text-beige-700 !bg-beige-700'
                : 'border-transparent text-gray-500 hover:text-gray-700'
            )}
          >
            Login
          </button>
          <button
            onClick={() => setActiveTab('register')}
            className={clsx(
              'flex-1 py-3 font-medium border-b-2 transition-colors',
              activeTab === 'register'
                ? 'border-beige-700 text-beige-700 !bg-beige-700'
                : 'border-transparent text-gray-500 hover:text-gray-700'
            )}
          >
            Register
          </button>
        </div>

        {apiError && (
          <div className="mb-4 p-3 bg-red-50 text-red-500 text-sm rounded-lg">
            {apiError}
          </div>
        )}

        {activeTab === 'login' ? <LoginForm /> : <RegisterForm />}
      </div>
    </div>,
    document.body
  );
};

export default AuthModal;
