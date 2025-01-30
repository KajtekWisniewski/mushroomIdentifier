import { Link, useLocation } from 'react-router-dom';
import { MushroomCategory } from '../../../contracts/mushroom/mushroom';

export default function CategoryTabs() {
  const location = useLocation();

  return (
    <div className="flex justify-center space-x-3 mt-4 flex-wrap items-center gap-1">
      <Link
        to="/search"
        className={`px-4 py-2 rounded text-white hover:bg-primary-900 ${
          location.pathname === '/search' ? 'bg-primary-900' : 'bg-primary-800'
        }`}
      >
        Search
      </Link>
      <Link
        to="/library"
        className={`px-4 py-2 rounded text-white hover:bg-primary-900 ${
          location.pathname === '/library' ? 'bg-primary-900' : 'bg-primary-800'
        }`}
      >
        Library
      </Link>

      {Object.entries(MushroomCategory).map(([key, value]) => {
        if (isNaN(Number(key))) {
          return (
            <Link
              key={key}
              to={`/library/category/${value}`}
              className={`px-4 py-2 rounded text-white hover:bg-primary-900 ${
                location.pathname === `/library/category/${value}`
                  ? 'bg-primary-900'
                  : 'bg-primary-800'
              }`}
            >
              {key}
            </Link>
          );
        }
        return null;
      })}
    </div>
  );
}
