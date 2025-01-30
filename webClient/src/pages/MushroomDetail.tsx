import { useParams } from 'react-router-dom';
import { useMushroom } from '../components/hooks/data-fetch/useMushroom';
import MushroomEntry from '../components/Library/Entry/MushroomEntry';
import Forum from '../components/Forum/Forum';
import MushroomMap from '../components/Map/MushroomMap';

export default function MushroomDetail() {
  const { id, locationId } = useParams();
  const { data: mushroom, isLoading, error } = useMushroom(Number(id));

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>{(error as Error).message}</div>;
  if (!mushroom) return <div>Mushroom not found</div>;

  return (
    <div className="flex flex-wrap justify-center">
      <div className="flex items-center flex-row justify-center flex-wrap">
        <MushroomEntry mushroom={mushroom} id={mushroom.id} />
        <div className="h-[501px] w-[501px] bg-beige-400 m-2 rounded-2xl overflow-hidden">
          <MushroomMap
            mushroomId={Number(id)}
            locations={mushroom.locations}
            mushroomName={mushroom.name}
            initialLocationId={locationId ? Number(locationId) : undefined}
          />
        </div>
      </div>
      <div className="flex m-4 bg-beige-400 rounded-2xl p-4 max-w-[1288px]">
        <Forum mushroomName={mushroom.name} mushroomId={mushroom.id} />
      </div>
    </div>
  );
}
