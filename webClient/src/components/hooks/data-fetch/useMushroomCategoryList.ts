import { useQuery } from '@tanstack/react-query';
import httpClient from '../../../utils/httpClient';
import { MushroomDTO, MushroomCategory } from '../../../contracts/mushroom/mushroom';

export const useMushroomsCategoryList = (categoryId: MushroomCategory) => {
  return useQuery<MushroomDTO[]>({
    queryKey: ['mushrooms-categories', categoryId],
    queryFn: async () => {
      const { data } = await httpClient.get(`/api/mushrooms/category/${categoryId}`);
      return data;
    }
  });
};
