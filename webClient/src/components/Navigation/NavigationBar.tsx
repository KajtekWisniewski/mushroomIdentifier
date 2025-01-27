import { Link } from 'react-router-dom';
import Logout from '../Auth/Logout';
import { useSelector } from 'react-redux';
import { RootState } from '../../store';

export default function NavigationBar() {
  const { user } = useSelector((state: RootState) => state.auth);

  return (
    <>
      <nav className="sticky top-0 flex flex-row items-center justify-between p-2 gap-2 w-full bg-primary-900 text-white h-10 z-10">
        <div className="flex flex-row gap-3 items-center justify-center">
          <Link to="/">Home</Link>
          <Link to="/library">Library</Link>
        </div>
        <div className="flex flex-row gap-3 items-center justify-center">
          {!user && (
            <>
              <Link to="/login">Login</Link>
              <Link to="/register">Register</Link>
            </>
          )}
          {user && (
            <>
              <Link to="profile">Profile</Link>
              <Logout />
            </>
          )}
        </div>
      </nav>
    </>
  );
}
