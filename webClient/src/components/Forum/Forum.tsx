import { useState, useEffect } from 'react';
import { useSelector } from 'react-redux';
import { useInView } from 'react-intersection-observer';
import { RootState } from '../../store';
import { useForumPosts } from '../hooks/data-fetch/useForumPosts';
import PaginationInfo from '../Utils/PaginationInfo';
import { CreatePostForm } from './CreatePostForm';
import { EditPostForm } from './EditPostForm';
import { ForumPost } from './ForumPost';

interface ForumProps {
  mushroomId: number;
  mushroomName: string;
}

const Forum = ({ mushroomId, mushroomName }: ForumProps) => {
  const [newPost, setNewPost] = useState('');
  const [editingPost, setEditingPost] = useState<{
    id: number;
    content: string;
  } | null>(null);
  const { user } = useSelector((state: RootState) => state.auth);
  const { ref, inView } = useInView();

  const {
    posts: { data, isLoading, error, isFetchingNextPage, hasNextPage, fetchNextPage },
    createPost,
    updatePost,
    deletePost
  } = useForumPosts(mushroomId);

  useEffect(() => {
    if (inView && hasNextPage && !isFetchingNextPage) {
      fetchNextPage();
    }
  }, [inView, hasNextPage, isFetchingNextPage, fetchNextPage]);

  const handleDeletePost = async (postId: number) => {
    if (!window.confirm('Are you sure you want to delete this post?')) return;
    try {
      await deletePost.mutateAsync(postId);
    } catch (error) {
      console.error('Error deleting post:', error);
    }
  };

  const isEmptyList = data?.pages[0].items.length ? true : false;

  if (error) return <div className="text-center text-red-500">Error loading posts</div>;

  return (
    <div className="w-full max-w-[1288px] mx-auto">
      <div className="mb-8 bg-beige-100 p-6 rounded-lg shadow-md">
        <div className="flex items-center gap-4">
          <h2 className="text-2xl font-bold text-primary-900">
            Discussion:
            <span className="ml-2 text-primary-700">{mushroomName}</span>
          </h2>
        </div>
      </div>

      <div className="space-y-8">
        {user ? (
          <CreatePostForm
            newPost={newPost}
            setNewPost={setNewPost}
            createPost={createPost}
            mushroomId={mushroomId}
          />
        ) : (
          <div className="bg-beige-100 p-6 rounded-lg text-center shadow-md">
            <p className="text-primary-800 font-medium">
              Please log in to participate in the discussion.
            </p>
          </div>
        )}

        <div className="space-y-6">
          {data?.pages.map((page, i) => (
            <div key={i} className="space-y-4">
              {page.items.map((post) =>
                editingPost?.id === post.id ? (
                  <div
                    key={post.id}
                    className={`flex ${
                      user?.username === post.username ? 'justify-end' : 'justify-start'
                    }`}
                  >
                    <EditPostForm
                      editingPost={editingPost}
                      setEditingPost={setEditingPost}
                      updatePost={updatePost}
                    />
                  </div>
                ) : (
                  <ForumPost
                    key={post.id}
                    post={post}
                    currentUsername={user?.username}
                    isAdmin={user?.isAdmin}
                    onEdit={(id, content) => setEditingPost({ id, content })}
                    onDelete={handleDeletePost}
                  />
                )
              )}
            </div>
          ))}
        </div>

        <PaginationInfo
          ref={ref}
          isLoading={isLoading}
          isFetchingNextPage={isFetchingNextPage}
          hasNextPage={hasNextPage}
          emptyList={isEmptyList}
        />
      </div>
    </div>
  );
};

export default Forum;
