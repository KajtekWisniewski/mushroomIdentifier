import { useEffect } from 'react';
import { useInView } from 'react-intersection-observer';
import { Link } from 'react-router-dom';
import { useMushroomsList } from '../hooks/data-fetch/useMushroomList';
import MushroomEntry from './Entry/MushroomEntry';
import PaginationInfo from '../Utils/PaginationInfo';

export default function MushroomList() {
  const { ref, inView } = useInView();
  const { data, isLoading, error, isFetchingNextPage, hasNextPage, fetchNextPage } =
    useMushroomsList();

  useEffect(() => {
    if (inView && hasNextPage && !isFetchingNextPage) {
      fetchNextPage();
    }
  }, [inView, hasNextPage, isFetchingNextPage, fetchNextPage]);

  const isEmptyList = data?.pages[0].items.length ? true : false;

  if (error)
    return <div className="text-center text-red-500">{(error as Error).message}</div>;

  return (
    <div className="flex flex-col items-center justify-center gap-4">
      {data?.pages.map((page, pageIndex) => (
        <div
          key={pageIndex}
          className="w-full flex flex-col justify-center items-center"
        >
          {page.items.map((mushroom) => (
            <Link to={`/mushroom/${mushroom.id}`} key={mushroom.id}>
              <MushroomEntry mushroom={mushroom} id={mushroom.id} listView />
            </Link>
          ))}
        </div>
      ))}

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
