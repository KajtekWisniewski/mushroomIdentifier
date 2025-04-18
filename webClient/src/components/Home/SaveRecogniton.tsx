import { useState } from 'react';
import { useSelector } from 'react-redux';
import { RootState } from '../../store';
import httpClient from '../../utils/httpClient';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { MushroomPrediction } from '../../contracts/mushroom/mushroom';
import { SaveRecognitionDTO } from '../../contracts/recognitions/recognitions';
import { MushroomCategory } from '../../contracts/mushroom/mushroom';

interface SaveRecognitionProps {
  predictions?: MushroomPrediction[];
  enable?: boolean;
}

export default function SaveRecognition({ predictions, enable }: SaveRecognitionProps) {
  const { user } = useSelector((state: RootState) => state.auth);
  const [error, setError] = useState<string | null>(null);
  const queryClient = useQueryClient();

  const saveMutation = useMutation({
    mutationFn: async (data: SaveRecognitionDTO) => {
      const response = await httpClient.post('/api/profile/recognitions', data);
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['user-recognitions'] });
      setError(null);
    },
    onError: (error: any) => {
      setError(error.response?.data?.message || 'Failed to save recognitions');
    }
  });

  const convertCategoryToEnum = (
    category: string | MushroomCategory
  ): MushroomCategory => {
    if (typeof category === 'number') {
      return category as MushroomCategory;
    }
    return MushroomCategory[category as keyof typeof MushroomCategory];
  };

  const handleSave = () => {
    if (!predictions) {
      setError('No predictions to save');
      return;
    }

    try {
      const recognitionsData: SaveRecognitionDTO = {
        predictions: predictions.map((pred) => ({
          category: convertCategoryToEnum(pred.category),
          confidence: pred.confidence,
          savedAt: new Date().toISOString()
        }))
      };
      saveMutation.mutate(recognitionsData);
    } catch (err) {
      setError('Error converting prediction categories');
    }
  };

  if (!user || !predictions) return null;

  const disableButton = saveMutation.isSuccess || enable;

  return (
    <div className="mt-4">
      <button
        onClick={handleSave}
        disabled={saveMutation.isPending || disableButton}
        className="px-4 py-2 bg-primary-800 rounded hover:bg-primary-900 disabled:opacity-50 disabled:!cursor-not-allowed disabled:hover:!opacity-50"
      >
        {saveMutation.isPending ? 'Saving...' : 'Save Recognition'}
      </button>
      {error && <div className="text-red-500 mt-2">{error}</div>}
      {saveMutation.isSuccess && (
        <div className="text-green-500 mt-2">Recognition saved successfully!</div>
      )}
    </div>
  );
}
