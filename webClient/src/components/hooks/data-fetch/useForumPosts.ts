import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import httpClient from '../../../utils/httpClient';
import { CreateForumPostDTO, ForumPostDTO } from '../../../contracts/forum/forum';

export const useForumPosts = (mushroomId: number) => {
  const queryClient = useQueryClient();

  const posts = useQuery({
    queryKey: ['forum-posts', mushroomId],
    queryFn: async () => {
      const { data } = await httpClient.get<ForumPostDTO[]>(
        `/api/forum/mushroom/${mushroomId}`
      );
      return data;
    }
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
