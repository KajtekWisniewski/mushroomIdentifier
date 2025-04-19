import { useQuery } from '@tanstack/react-query';
import httpClient from '../../../utils/httpClient';

const fetchHealth = async () => {
  const response = await httpClient.get('/health', {
    baseURL: 'http://localhost:5000',
  });
  return response.data;
};

export const useRecogniserHealth = () => {
  const {
    isError,
  } = useQuery({
    queryKey: ['recogniser-health'],
    queryFn: fetchHealth,
    refetchInterval: 90000,
    retry: false,
  });

  return {
    isError,
  };
};
