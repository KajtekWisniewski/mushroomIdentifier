import { useState } from 'react';
import { useSelector } from 'react-redux';
import { RootState } from '../../store';
import { useForumPosts } from '../hooks/data-fetch/useForumPosts';
import { ForumPostDTO } from '../../contracts/forum/forum';

const Forum = ({
  mushroomId,
  mushroomName
}: {
  mushroomId: number;
  mushroomName: string;
}) => {
  const [newPost, setNewPost] = useState('');
  const [editingPost, setEditingPost] = useState<{
    id: number;
    content: string;
  } | null>(null);
  const { user } = useSelector((state: RootState) => state.auth);
  const { posts, createPost, updatePost, deletePost } = useForumPosts(mushroomId);

  const handleCreatePost = async (e: React.FormEvent) => {
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

  const handleUpdatePost = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingPost) return;

    try {
      await updatePost.mutateAsync({
        id: editingPost.id,
        content: editingPost.content
      });
      setEditingPost(null);
    } catch (error) {
      console.error('Error updating post:', error);
    }
  };

  const handleDeletePost = async (postId: number) => {
    if (!window.confirm('Are you sure you want to delete this post?')) return;

    try {
      await deletePost.mutateAsync(postId);
    } catch (error) {
      console.error('Error deleting post:', error);
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const renderPost = (post: ForumPostDTO) => {
    const isEditing = editingPost?.id === post.id;
    const canModify = user && (user.isAdmin || user.username === post.username);

    return (
      <div key={post.id} className="bg-white rounded-lg shadow-md p-4 mb-4">
        {isEditing ? (
          <form onSubmit={handleUpdatePost} className="space-y-4">
            <textarea
              value={editingPost.content}
              onChange={(e) =>
                setEditingPost({ ...editingPost, content: e.target.value })
              }
              className="w-full p-2 border rounded-md"
              rows={3}
            />
            <div className="flex justify-end space-x-2">
              <button
                type="submit"
                className="bg-green-500 text-white px-4 py-2 rounded"
              >
                Save
              </button>
              <button
                type="button"
                onClick={() => setEditingPost(null)}
                className="bg-gray-500 text-white px-4 py-2 rounded"
              >
                Cancel
              </button>
            </div>
          </form>
        ) : (
          <>
            <div className="flex justify-between items-start mb-2">
              <div>
                <span className="font-semibold">{post.username}</span>
                <span className="text-gray-500 text-sm ml-2">
                  {formatDate(post.createdAt)}
                  {post.updatedAt && ' (edited)'}
                </span>
              </div>
              {canModify && (
                <div className="space-x-2">
                  <button
                    onClick={() =>
                      setEditingPost({ id: post.id, content: post.content })
                    }
                    className="text-blue-500 hover:text-blue-700"
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => handleDeletePost(post.id)}
                    className="text-red-500 hover:text-red-700"
                  >
                    Delete
                  </button>
                </div>
              )}
            </div>
            <p className="text-gray-700 whitespace-pre-wrap">{post.content}</p>
          </>
        )}
      </div>
    );
  };

  if (posts.isLoading) return <div className="text-center">Loading posts...</div>;
  if (posts.error)
    return <div className="text-red-500 text-center">Error loading posts</div>;

  return (
    <div className="max-w-4xl mx-auto p-4">
      <h2 className="text-2xl font-bold mb-6">Discussion: {mushroomName}</h2>

      {user ? (
        <form onSubmit={handleCreatePost} className="mb-8">
          <textarea
            value={newPost}
            onChange={(e) => setNewPost(e.target.value)}
            placeholder="Write your post..."
            className="w-full p-4 border rounded-lg mb-2"
            rows={4}
          />
          <button
            type="submit"
            disabled={createPost.isPending}
            className="bg-primary-800 text-white px-6 py-2 rounded-lg hover:bg-primary-900 disabled:opacity-50"
          >
            {createPost.isPending ? 'Posting...' : 'Post'}
          </button>
        </form>
      ) : (
        <div className="text-center mb-8 p-4 bg-gray-100 rounded-lg">
          Please log in to participate in the discussion.
        </div>
      )}

      <div className="space-y-4">
        {posts.data && posts.data.length > 0 ? (
          posts.data.map(renderPost)
        ) : (
          <div className="text-center text-gray-500">
            No posts yet. Be the first to start the discussion!
          </div>
        )}
      </div>
    </div>
  );
};

export default Forum;
