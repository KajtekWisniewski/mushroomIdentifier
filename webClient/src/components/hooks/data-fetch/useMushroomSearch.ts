import { useInfiniteQuery } from '@tanstack/react-query';
import httpClient from '../../../utils/httpClient';
import { MushroomDTO, MushroomCategory } from '../../../contracts/mushroom/mushroom';
import { PagedList } from '../../../contracts/pagination/pagination';

interface SearchParams {
  searchTerm?: string;
  category?: MushroomCategory;
  season?: string;
  isEdible?: boolean;
}

export const useMushroomSearch = (searchParams: SearchParams) => {
  return useInfiniteQuery<PagedList<MushroomDTO>>({
    queryKey: ['mushrooms-search', searchParams],
    queryFn: async ({ pageParam = 1 }) => {
      const { data } = await httpClient.get<PagedList<MushroomDTO>>(
        '/api/mushrooms/search',
        {
          params: {
            page: pageParam,
            pageSize: 10,
            ...searchParams
          }
        }
      );
      return data;
    },
    getNextPageParam: (lastPage) =>
      lastPage.hasNextPage ? lastPage.currentPage + 1 : undefined,
    initialPageParam: 1
  });
};
