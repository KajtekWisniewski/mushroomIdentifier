import { RecognitionDTO } from '../recognitions/recognitions';

export interface UserProfileDTO {
  username: string;
  email: string;
  isAdmin: boolean;
  savedRecognitions: RecognitionDTO[];
}

export interface UserLoginDTO {
  username: string;
  password: string;
}

export interface UserDTO {
  username: string;
  email: string;
  password: string;
}

export interface LoginResponseDTO {
  token: string;
  user: UserProfileDTO;
}
