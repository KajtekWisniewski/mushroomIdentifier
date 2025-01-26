import { Link } from 'react-router-dom';

export default function NavigationBar() {
  return (
    <nav className="flex flex-row items-center p-2 gap-2 w-full bg-primary-900 text-white h-10">
      <Link to="/">Home</Link>
      <Link to="/library">Library</Link>
    </nav>
  );
}
