import { UseMutationResult } from '@tanstack/react-query';
import { Save, X } from 'lucide-react';

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
    <form
      onSubmit={handleSubmit}
      className="w-full max-w-[70%] bg-beige-100 p-6 rounded-lg shadow-md space-y-4"
    >
      <div className="space-y-2">
        <label className="text-sm font-medium text-primary-800">Edit Post</label>
        <textarea
          value={editingPost.content}
          onChange={(e) => setEditingPost({ ...editingPost, content: e.target.value })}
          className="w-full p-3 border border-beige-300 rounded-lg resize-none
                   focus:ring-2 focus:ring-primary-500 focus:border-transparent
                   transition-all duration-200 bg-white"
          rows={4}
        />
      </div>

      <div className="flex justify-end gap-3">
        <button
          type="button"
          onClick={() => setEditingPost(null)}
          className="flex items-center gap-2 px-4 py-2 bg-beige-300 text-primary-800
                   rounded-lg hover:bg-beige-400 transition-colors duration-200"
        >
          <X size={16} />
          <span>Cancel</span>
        </button>
        <button
          type="submit"
          className="flex items-center gap-2 px-4 py-2 bg-primary-700 text-white
                   rounded-lg hover:bg-primary-800 transition-colors duration-200"
        >
          <Save size={16} />
          <span>Save</span>
        </button>
      </div>
    </form>
  );
};
