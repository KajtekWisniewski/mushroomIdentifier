import { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useSelector } from 'react-redux';
import { RootState } from '../../store';
import { Home, Library, User, LogIn } from 'lucide-react';
import clsx from 'clsx';
import Logout from '../Auth/Logout';
import AuthModal from '../Auth/Modal/AuthModal';
import { RecogniserHealthStatus } from './RecogniserHealthStatus';

export default function NavigationBar() {
  const { user } = useSelector((state: RootState) => state.auth);
  const [isAuthModalOpen, setIsAuthModalOpen] = useState(false);
  const [initialAuthTab, setInitialAuthTab] = useState<'login' | 'register'>('login');
  const location = useLocation();

  const handleAuthClick = (tab: 'login' | 'register') => {
    setInitialAuthTab(tab);
    setIsAuthModalOpen(true);
  };

  const NavLink = ({ to, children }: { to: string; children: React.ReactNode }) => (
    <Link
      to={to}
      className={clsx(
        'flex items-center gap-2 px-4 py-2 rounded-lg transition-all',
        'hover:bg-beige-200/10',
        location.pathname === to ? 'bg-beige-200/20 text-white' : 'text-beige-100'
      )}
    >
      {children}
    </Link>
  );

  const AuthButton = ({
    onClick,
    children
  }: {
    onClick: () => void;
    children: React.ReactNode;
  }) => (
    <button
      onClick={onClick}
      className={clsx(
        'flex items-center gap-2 px-4 py-2 rounded-lg transition-all',
        'bg-beige-200/10 text-beige-100 hover:bg-beige-200/20'
      )}
    >
      {children}
    </button>
  );

  return (
    <>
      <nav className="sticky top-0 w-full bg-primary-900 shadow-lg z-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex items-center justify-between h-16">
            <div className="flex items-center space-x-2">
              <RecogniserHealthStatus></RecogniserHealthStatus>
              <NavLink to="/">
                <Home size={18} />
                <span className="hidden sm:inline">Home</span>
              </NavLink>
              <NavLink to="/library">
                <Library size={18} />
                <span className="hidden sm:inline">Library</span>
              </NavLink>
            </div>

            <div className="flex items-center space-x-2">
              {!user ? (
                <>
                  <AuthButton onClick={() => handleAuthClick('login')}>
                    <LogIn size={18} />
                    <span className="hidden sm:inline">Login</span>
                  </AuthButton>
                  <AuthButton onClick={() => handleAuthClick('register')}>
                    <User size={18} />
                    <span className="hidden sm:inline">Register</span>
                  </AuthButton>
                </>
              ) : (
                <>
                  <NavLink to="/profile">
                    <User size={18} />
                    <span className="hidden sm:inline">Profile</span>
                  </NavLink>
                  <Logout />
                </>
              )}
            </div>
          </div>
        </div>
      </nav>

      <AuthModal
        isOpen={isAuthModalOpen}
        onClose={() => setIsAuthModalOpen(false)}
        initialTab={initialAuthTab}
      />
    </>
  );
}
