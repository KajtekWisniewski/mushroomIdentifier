export enum MushroomCategory {
  Agaricus = 0,
  Amanita = 1,
  Boletus = 2,
  Cortinarius = 3,
  Entoloma = 4,
  Hygrocybe = 5,
  Lactarius = 6,
  Russula = 7,
  Suillus = 8
}

export interface Coordinates {
  id: number;
  latitude: number;
  longitude: number;
  userId: number;
  username: string;
}

export interface SaveCoordinatesDTO {
  latitude: number;
  longitude: number;
}

export interface MushroomPrediction {
  category: string;
  confidence: string;
}

export interface CategoriesDTO {
  predictions: MushroomPrediction[];
}

export interface MushroomDTO {
  id: number;
  name: string;
  scientificName: string;
  category: MushroomCategory;
  description: string;
  isEdible: boolean;
  habitat: string;
  season: string;
  commonNames: string[];
  imageUrls: string[];
  locations: Coordinates[];
  lastUpdated: Date;
}

export interface UpsertMushroomDTO {
  name: string;
  scientificName: string;
  category: MushroomCategory;
  description: string;
  isEdible: boolean;
  habitat: string;
  season: string;
  commonNames: string[];
  imageUrls: string[];
  locations: Coordinates[];
  lastUpdated: string;
}
