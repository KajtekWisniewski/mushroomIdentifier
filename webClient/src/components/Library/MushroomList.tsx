import { useMushroomsList } from '../hooks/data-fetch/useMushroomList';
import { Link } from 'react-router-dom';
import MushroomEntry from './Entry/MushroomEntry';

export default function MushroomList() {
  const { data: mushrooms, isLoading, error } = useMushroomsList();

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>{(error as Error).message}</div>;

  return (
    <div className="flex flex-col items-center gap-2">
      {isLoading ? (
        <div>Loading...</div>
      ) : (
        mushrooms?.map((mushroom) => (
          <Link to={`/mushroom/${mushroom.id}`} key={mushroom.id}>
            <MushroomEntry mushroom={mushroom} id={mushroom.id} listView />
          </Link>
        ))
      )}
    </div>
  );
}
