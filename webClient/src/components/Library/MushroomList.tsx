import { Suspense } from 'react';
import { useMushrooms } from '../Hooks/useMushrooms';

export default function MushroomList() {
  const { data: mushrooms, isLoading, error } = useMushrooms();

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>{(error as Error).message}</div>;

  return (
    <div>
      <Suspense>
        {mushrooms?.map((mushroom) => (
          <div key={mushroom.id}>
            <div>{mushroom.commonNames[0]}</div>
            <div>{mushroom.category}</div>
            {mushroom.imageUrls.map((url, index) => (
              <img key={index} src={url} alt={mushroom.name} />
            ))}
          </div>
        ))}
      </Suspense>
    </div>
  );
}
