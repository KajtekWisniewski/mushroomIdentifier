import { useParams } from 'react-router-dom';
import { useMushroomsCategoryList } from '../components/hooks/data-fetch/useMushroomCategoryList';
import MushroomEntry from '../components/Library/Entry/MushroomEntry';
import { MushroomCategory } from '../contracts/mushroom/mushroom';
import { Link } from 'react-router-dom';
import CategoryTabs from '../components/Library/categories/CategoryTabs';

export default function CategoryMushroomList() {
  const { categoryId } = useParams();
  const numericCategoryId = Number(categoryId);

  const {
    data: mushrooms,
    isLoading,
    error
  } = useMushroomsCategoryList(numericCategoryId);

  if (error) return <div>{(error as Error).message}</div>;

  return (
    <div className="flex flex-col items-center gap-2">
      <CategoryTabs />
      {isLoading ? (
        <div>Loading ...</div>
      ) : (
        <>
          <h2 className="text-2xl font-bold mb-6">
            {MushroomCategory[numericCategoryId]} Mushrooms
          </h2>
          {mushrooms?.map((mushroom) => (
            <Link to={`/mushroom/${mushroom.id}`} key={mushroom.id}>
              <MushroomEntry mushroom={mushroom} id={mushroom.id} listView />
            </Link>
          ))}
        </>
      )}
    </div>
  );
}
