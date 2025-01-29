export interface ForumPostDTO {
  id: number;
  content: string;
  createdAt: string;
  updatedAt?: string;
  mushroomId: number;
  mushroomName: string;
  userId: number;
  username: string;
}

export interface CreateForumPostDTO {
  content: string;
  mushroomId: number;
}

export interface UpdateForumPostDTO {
  content: string;
}
