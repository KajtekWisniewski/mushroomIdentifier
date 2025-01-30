import React, { useState } from 'react';
import { UseMutationResult } from '@tanstack/react-query';
import { ForumPostDTO, CreateForumPostDTO } from '../../contracts/forum/forum';
import { MessageSquarePlus, MessageSquareDiff } from 'lucide-react';

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
    <div className="bg-beige-100 rounded-lg shadow-md overflow-hidden">
      <div className="p-6">
        <div className="flex items-center justify-between mb-6">
          <div className="flex items-center gap-3">
            <MessageSquarePlus className="w-5 h-5 text-primary-600" />
            <h3 className="text-lg font-medium text-primary-900">Create Post</h3>
          </div>
          <button
            type="button"
            onClick={handleHide}
            className="flex items-center gap-2 px-4 py-2 bg-primary-700 text-white rounded-lg hover:bg-primary-800 transition-all duration-200 shadow-sm"
          >
            {hideCreateReplyForm ? (
              <>
                <MessageSquarePlus className="w-5 h-5" />
                <span className="hidden sm:inline">Show Form</span>
              </>
            ) : (
              <>
                <MessageSquareDiff className="w-5 h-5" />
                <span className="hidden sm:inline">Hide Form</span>
              </>
            )}
          </button>
        </div>

        {!hideCreateReplyForm && (
          <form onSubmit={handleSubmit} className="space-y-6">
            <div className="relative">
              <textarea
                value={newPost}
                onChange={(e) => setNewPost(e.target.value)}
                placeholder="Share your thoughts..."
                className="w-full p-4 border border-beige-300 rounded-lg bg-white 
                          focus:ring-2 focus:ring-primary-500 focus:border-transparent
                          transition-all duration-200 resize-y min-h-[120px]
                          placeholder-beige-400"
                rows={4}
              />
            </div>

            <div className="flex justify-end">
              <button
                type="submit"
                disabled={createPost.isPending}
                className="inline-flex items-center justify-center px-6 py-2.5 
                         bg-primary-700 text-white rounded-lg 
                         hover:bg-primary-800 transition-all duration-200
                         disabled:opacity-50 disabled:cursor-not-allowed
                         shadow-sm hover:shadow-md"
              >
                {createPost.isPending ? (
                  <span className="flex items-center gap-2">
                    <span
                      className="inline-block w-4 h-4 border-2 border-white 
                                   border-t-transparent rounded-full animate-spin"
                    />
                    Posting...
                  </span>
                ) : (
                  'Post'
                )}
              </button>
            </div>
          </form>
        )}
      </div>
    </div>
  );
};
