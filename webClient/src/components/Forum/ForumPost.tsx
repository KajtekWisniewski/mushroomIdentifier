import { ForumPostDTO } from '../../contracts/forum/forum';

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
    <div className={`flex ${isOwnPost ? 'justify-end' : 'justify-start'} mb-4`}>
      <div
        className={`max-w-[70%] ${
          isOwnPost ? 'bg-primary-700 bg-opacity-10 text-white' : 'bg-white'
        } rounded-lg shadow-md p-4`}
      >
        <div
          className={`flex items-start gap-2 mb-2 ${
            isOwnPost ? 'flex-row-reverse' : 'flex-row'
          }`}
        >
          <div className={`flex flex-col ${isOwnPost ? 'items-end' : 'items-start'}`}>
            <span className="font-semibold">{post.username}</span>
            <span className="text-white text-sm">
              {formatDate(post.createdAt)}
              {post.updatedAt && ' (edited)'}
            </span>
          </div>
          {canModify && (
            <div className="space-x-2">
              <button
                onClick={() => onEdit(post.id, post.content)}
                className="text-blue-500 hover:text-blue-700"
              >
                Edit
              </button>
              <button
                onClick={() => onDelete(post.id)}
                className="text-red-500 hover:text-red-700"
              >
                Delete
              </button>
            </div>
          )}
        </div>
        <p className="text-gray-700 whitespace-pre-wrap break-words">{post.content}</p>
      </div>
    </div>
  );
};
