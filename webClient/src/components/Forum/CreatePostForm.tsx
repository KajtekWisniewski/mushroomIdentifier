import React, { useState } from 'react';
import { UseMutationResult } from '@tanstack/react-query';
import { ForumPostDTO, CreateForumPostDTO } from '../../contracts/forum/forum';

interface CreatePostFormProps {
  newPost: string;
  setNewPost: (value: string) => void;
  createPost: UseMutationResult<ForumPostDTO, Error, CreateForumPostDTO>;
  mushroomId: number;
}

export const CreatePostForm = ({
  newPost,
  setNewPost,
  createPost,
  mushroomId
}: CreatePostFormProps) => {
  const [hideCreateReplyForm, setHideCreateReplyForm] = useState<boolean>(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newPost.trim()) return;

    try {
      await createPost.mutateAsync({
        content: newPost,
        mushroomId
      });
      setNewPost('');
    } catch (error) {
      console.error('Error creating post:', error);
    }
  };
  const handleHide = () => {
    setHideCreateReplyForm(!hideCreateReplyForm);
  };

  return (
    <>
      <div className="sticky top-10 bg-beige-400 w-full p-4 rounded-lg rounded-tl-none rounded-tr-none shadow-lg z-5">
        {!hideCreateReplyForm && (
          <form onSubmit={handleSubmit}>
            <textarea
              value={newPost}
              onChange={(e) => setNewPost(e.target.value)}
              placeholder="Write your post..."
              className="w-full p-4 border rounded-lg mb-2 bg-white"
              rows={4}
            />
            <div className="flex justify-end">
              <button
                type="submit"
                disabled={createPost.isPending}
                className="!bg-primary-800 text-white px-6 py-2 rounded-lg hover:bg-primary-900 disabled:opacity-50"
              >
                {createPost.isPending ? 'Posting...' : 'Post'}
              </button>
            </div>
          </form>
        )}
        <button
          type="button"
          onClick={handleHide}
          className="!bg-primary-800 text-white ml-2 w-fit"
        >
          {hideCreateReplyForm ? 'show form' : 'hide form'}
        </button>
      </div>
    </>
  );
};
