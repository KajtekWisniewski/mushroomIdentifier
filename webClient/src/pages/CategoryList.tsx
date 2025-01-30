import { useEffect } from 'react';
import { useInView } from 'react-intersection-observer';
import { useParams } from 'react-router-dom';
import { Link } from 'react-router-dom';
import { useMushroomsCategoryList } from '../components/hooks/data-fetch/useMushroomCategoryList';
import MushroomEntry from '../components/Library/Entry/MushroomEntry';
import { MushroomCategory } from '../contracts/mushroom/mushroom';
import CategoryTabs from '../components/Library/categories/CategoryTabs';
import PaginationInfo from '../components/Utils/PaginationInfo';

export default function CategoryMushroomList() {
  const { categoryId } = useParams();
  const numericCategoryId = Number(categoryId);
  const { ref, inView } = useInView();

  const { data, isLoading, error, isFetchingNextPage, hasNextPage, fetchNextPage } =
    useMushroomsCategoryList(numericCategoryId);

  useEffect(() => {
    if (inView && hasNextPage && !isFetchingNextPage) {
      fetchNextPage();
    }
  }, [inView, hasNextPage, isFetchingNextPage, fetchNextPage]);

  if (error)
    return <div className="text-center text-red-500">{(error as Error).message}</div>;

  const isEmptyList = data?.pages[0].items.length ? true : false;

  return (
    <div className="flex flex-col items-center gap-4">
      <CategoryTabs />

      <h2 className="text-2xl font-bold mb-4">
        {MushroomCategory[numericCategoryId]} Mushrooms
      </h2>

      <div className="flex flex-col items-center justify-center">
        {data?.pages.map((page, pageIndex) => (
          <div key={pageIndex} className="w-full animate-fade-in">
            {page.items.map((mushroom) => (
              <Link to={`/mushroom/${mushroom.id}`} key={mushroom.id}>
                <MushroomEntry mushroom={mushroom} id={mushroom.id} listView />
              </Link>
            ))}
          </div>
        ))}
      </div>

      <PaginationInfo
        ref={ref}
        isLoading={isLoading}
        isFetchingNextPage={isFetchingNextPage}
        hasNextPage={hasNextPage}
        emptyList={isEmptyList}
      ></PaginationInfo>
    </div>
  );
}
