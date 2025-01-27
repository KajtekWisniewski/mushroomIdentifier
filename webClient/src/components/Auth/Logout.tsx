import { useDispatch } from 'react-redux';
import { logOut } from '../../store/slices/authSlice';

const Logout = () => {
  const dispatch = useDispatch();

  const handleLogout = () => {
    dispatch(logOut());
  };

  return <button onClick={handleLogout}>Logout</button>;
};

export default Logout;
