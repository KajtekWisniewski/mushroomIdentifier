import { useQuery } from '@tanstack/react-query';
import httpClient from '../../utils/httpClient';
import { MushroomDTO } from '../../contracts/mushroom/mushroom';

export const useMushrooms = () => {
  return useQuery<MushroomDTO[]>({
    queryKey: ['mushrooms'],
    queryFn: async () => {
      const { data } = await httpClient.get('/api/mushrooms');
      return data;
    }
  });
};
