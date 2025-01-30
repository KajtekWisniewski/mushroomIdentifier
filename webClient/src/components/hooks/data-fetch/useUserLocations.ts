import { useQuery } from '@tanstack/react-query';
import httpClient from '../../../utils/httpClient';

interface UserLocation {
  coordinates: {
    id: number;
    latitude: number;
    longitude: number;
    userId: number;
    username: string;
  }[];
  mushroomName: string;
  mushroomId: number;
  mushroomImages: string[];
}

export const useUserLocations = () => {
  return useQuery<UserLocation[]>({
    queryKey: ['user-locations'],
    queryFn: async () => {
      const { data } = await httpClient.get('/api/profile/locations');
      return data;
    }
  });
};
