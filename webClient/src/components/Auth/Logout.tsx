import { useDispatch } from 'react-redux';
import { logOut } from '../../store/slices/authSlice';

const Logout = () => {
  const dispatch = useDispatch();

  const handleLogout = () => {
    dispatch(logOut());
  };

  return (
    <button
      className="h-[36px] flex items-center justify-center text-black"
      onClick={handleLogout}
    >
      Logout
    </button>
  );
};

export default Logout;
