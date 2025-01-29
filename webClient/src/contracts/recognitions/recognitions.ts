import { MushroomCategory } from '../mushroom/mushroom';

export interface RecognitionDTO {
  category: MushroomCategory;
  confidence: string;
  savedAt: string;
}

export interface RecognitionBatchDTO {
  batchId: string;
  savedAt: string;
  predictions: RecognitionDTO[];
}

export interface UserRecognitionsDTO {
  userId: number;
  savedRecognitions: RecognitionDTO[];
}

export interface SaveRecognitionDTO {
  predictions: RecognitionDTO[];
}
