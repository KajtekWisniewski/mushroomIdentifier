import { useInfiniteQuery } from '@tanstack/react-query';
import httpClient from '../../../utils/httpClient';
import { MushroomDTO } from '../../../contracts/mushroom/mushroom';
import { PagedList } from '../../../contracts/pagination/pagination';

export const useMushroomsList = () => {
  return useInfiniteQuery<PagedList<MushroomDTO>>({
    queryKey: ['mushrooms'],
    queryFn: async ({ pageParam = 1 }) => {
      const { data } = await httpClient.get<PagedList<MushroomDTO>>('/api/mushrooms', {
        params: {
          page: pageParam,
          pageSize: 10
        }
      });
      return data;
    },
    getNextPageParam: (lastPage) =>
      lastPage.hasNextPage ? lastPage.currentPage + 1 : undefined,
    initialPageParam: 1
  });
};
