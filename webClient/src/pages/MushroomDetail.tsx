import { useParams } from 'react-router-dom';
import { useMushroom } from '../components/hooks/data-fetch/useMushroom';
import MushroomEntry from '../components/Library/Entry/MushroomEntry';

export default function MushroomDetail() {
  const { id } = useParams();
  const { data: mushroom, isLoading, error } = useMushroom(Number(id));

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>{(error as Error).message}</div>;
  if (!mushroom) return <div>Mushroom not found</div>;

  return (
    <div>
      <div className="flex items-center flex-row justify-center">
        <MushroomEntry mushroom={mushroom} id={mushroom.id} />
        <div className="h-[400px] w-[400px] bg-beige-400 m-2 rounded-2xl">
          fake map component
        </div>
      </div>
      <div className="flex m-4 h-[400px] bg-beige-400 rounded-2xl p-4">fake forum</div>
    </div>
  );
}
