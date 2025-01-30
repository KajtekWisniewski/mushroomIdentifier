import { ForumPostDTO } from '../../contracts/forum/forum';
import { Edit2, Trash2, Clock } from 'lucide-react';

interface ForumPostProps {
  post: ForumPostDTO;
  currentUsername?: string;
  isAdmin?: boolean;
  onEdit: (id: number, content: string) => void;
  onDelete: (id: number) => void;
}

export const ForumPost = ({
  post,
  currentUsername,
  isAdmin,
  onEdit,
  onDelete
}: ForumPostProps) => {
  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const isOwnPost = currentUsername === post.username;
  const canModify = isAdmin || isOwnPost;

  return (
    <div className={`flex ${isOwnPost ? 'justify-end' : 'justify-start'}`}>
      <div
        className={`max-w-[70%] rounded-lg shadow-md overflow-hidden
                   ${
                     isOwnPost
                       ? 'bg-primary-700/10 border border-primary-700/20'
                       : 'bg-beige-600 border border-beige-600'
                   }`}
      >
        <div className="px-4 py-3 bg-beige-100/50 border-b border-beige-500">
          <div
            className={`flex items-center gap-3 ${
              isOwnPost ? 'flex-row-reverse' : 'flex-row'
            }`}
          >
            <div className={`flex flex-col ${isOwnPost ? 'items-end' : 'items-start'}`}>
              <span className="font-medium text-primary-900">{post.username}</span>
              <div className="flex items-center gap-1 text-sm text-black">
                <Clock size={14} />
                <span>{formatDate(post.createdAt)}</span>
                {post.updatedAt && <span className="text-primary-500">(edited)</span>}
              </div>
            </div>

            {canModify && (
              <div className="flex gap-2">
                <button
                  onClick={() => onEdit(post.id, post.content)}
                  className="p-1.5 rounded-md text-primary-600 hover:text-primary-800
                           hover:bg-primary-700/10 transition-colors duration-200"
                  title="Edit post"
                >
                  <Edit2 size={16} />
                </button>
                <button
                  onClick={() => onDelete(post.id)}
                  className="p-1.5 rounded-md text-red-500 hover:text-red-700
                           hover:bg-red-500/10 transition-colors duration-200"
                  title="Delete post"
                >
                  <Trash2 size={16} />
                </button>
              </div>
            )}
          </div>
        </div>

        <div className="p-4">
          <p className="text-primary-900 whitespace-pre-wrap break-words">
            {post.content}
          </p>
        </div>
      </div>
    </div>
  );
};
