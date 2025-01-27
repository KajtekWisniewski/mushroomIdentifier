import { useQuery } from '@tanstack/react-query';
import httpClient from '../../../utils/httpClient';
import { MushroomDTO } from '../../../contracts/mushroom/mushroom';

export const useMushroom = (id: number) => {
  return useQuery<MushroomDTO>({
    queryKey: ['mushroom', id],
    queryFn: async () => {
      const { data } = await httpClient.get(`/api/mushrooms/${id}`);
      return data;
    },
    enabled: !!id
  });
};
