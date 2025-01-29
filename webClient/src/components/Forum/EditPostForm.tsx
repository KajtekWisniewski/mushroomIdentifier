import { UseMutationResult } from '@tanstack/react-query';

interface EditPostFormProps {
  editingPost: { id: number; content: string };
  setEditingPost: (value: { id: number; content: string } | null) => void;
  updatePost: UseMutationResult<any, Error, { id: number; content: string }>;
}

export const EditPostForm = ({
  editingPost,
  setEditingPost,
  updatePost
}: EditPostFormProps) => {
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
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

  return (
    <form onSubmit={handleSubmit} className="space-y-4 w-full border p-3 rounded-md">
      <textarea
        value={editingPost.content}
        onChange={(e) => setEditingPost({ ...editingPost, content: e.target.value })}
        className="w-full p-2 border rounded-md"
        rows={3}
      />
      <div className="flex justify-end space-x-2">
        <button type="submit" className="!bg-green-500 text-white px-4 py-2 rounded">
          Save
        </button>
        <button
          type="button"
          onClick={() => setEditingPost(null)}
          className="!bg-gray-500 text-white px-4 py-2 rounded"
        >
          Cancel
        </button>
      </div>
    </form>
  );
};
