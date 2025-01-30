import { useDispatch } from 'react-redux';
import { logOut } from '../../store/slices/authSlice';
import { LogOut as LogOutIcon } from 'lucide-react';
import clsx from 'clsx';

const Logout = () => {
  const dispatch = useDispatch();

  const handleLogout = () => {
    dispatch(logOut());
  };

  return (
    <button
      onClick={handleLogout}
      className={clsx(
        'flex items-center gap-2 px-4 py-2 rounded-lg transition-all',
        'bg-red-500/10 text-red-100 hover:bg-red-500/20'
      )}
    >
      <LogOutIcon size={18} />
      <span className="hidden sm:inline">Logout</span>
    </button>
  );
};

export default Logout;
