import { useInfiniteQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import httpClient from '../../../utils/httpClient';
import { PagedList } from '../../../contracts/pagination/pagination';
import { RecognitionBatchDTO } from '../../../contracts/recognitions/recognitions';

export const useRecognitions = () => {
  const queryClient = useQueryClient();

  const { data, isLoading, error, isFetchingNextPage, hasNextPage, fetchNextPage } =
    useInfiniteQuery<PagedList<RecognitionBatchDTO>>({
      queryKey: ['user-recognitions'],
      queryFn: async ({ pageParam = 1 }) => {
        const response = await httpClient.get('/api/profile/recognitions', {
          params: {
            page: pageParam,
            pageSize: 5
          }
        });
        return response.data;
      },
      getNextPageParam: (lastPage) =>
        lastPage.hasNextPage ? lastPage.currentPage + 1 : undefined,
      initialPageParam: 1
    });

  const deleteBatch = useMutation({
    mutationFn: async (batchId: string) => {
      await httpClient.delete(`/api/profile/recognitions/${batchId}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['user-recognitions'] });
    }
  });

  return {
    data,
    isLoading,
    error,
    isFetchingNextPage,
    hasNextPage,
    fetchNextPage,
    deleteBatch
  };
};
