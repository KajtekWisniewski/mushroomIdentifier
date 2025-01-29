import { useInfiniteQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import httpClient from '../../../utils/httpClient';
import { ForumPostDTO, CreateForumPostDTO } from '../../../contracts/forum/forum';
import { PagedList } from '../../../contracts/pagination/pagination';

export const useForumPosts = (mushroomId: number) => {
  const queryClient = useQueryClient();

  const posts = useInfiniteQuery<PagedList<ForumPostDTO>>({
    queryKey: ['forum-posts', mushroomId],
    queryFn: async ({ pageParam = 1 }) => {
      const { data } = await httpClient.get<PagedList<ForumPostDTO>>(
        `/api/forum/mushroom/${mushroomId}`,
        {
          params: {
            page: pageParam,
            pageSize: 10
          }
        }
      );
      return data;
    },
    getNextPageParam: (lastPage) =>
      lastPage.hasNextPage ? lastPage.currentPage + 1 : undefined,
    initialPageParam: 1
  });

  const createPost = useMutation({
    mutationFn: async (newPost: CreateForumPostDTO) => {
      const { data } = await httpClient.post<ForumPostDTO>('/api/forum', newPost);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['forum-posts', mushroomId] });
    }
  });

  const updatePost = useMutation({
    mutationFn: async ({ id, content }: { id: number; content: string }) => {
      const { data } = await httpClient.put<ForumPostDTO>(`/api/forum/${id}`, {
        content
      });
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['forum-posts', mushroomId] });
    }
  });

  const deletePost = useMutation({
    mutationFn: async (postId: number) => {
      await httpClient.delete(`/api/forum/${postId}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['forum-posts', mushroomId] });
    }
  });

  return {
    posts,
    createPost,
    updatePost,
    deletePost
  };
};
