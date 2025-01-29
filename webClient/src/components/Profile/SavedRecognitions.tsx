import { useQuery } from '@tanstack/react-query';
import httpClient from '../../utils/httpClient';
import { UserRecognitionsDTO } from '../../contracts/recognitions/recognitions';
import { Link } from 'react-router-dom';
import { MushroomCategory } from '../../contracts/mushroom/mushroom';

export default function SavedRecognitions() {
  const {
    data: recognitions,
    isLoading,
    error
  } = useQuery<UserRecognitionsDTO>({
    queryKey: ['user-recognitions'],
    queryFn: async () => {
      const response = await httpClient.get('/api/profile/recognitions');
      return response.data;
    }
  });

  if (isLoading) return <div>Loading saved recognitions...</div>;
  if (error) return <div>Error loading recognitions</div>;
  if (!recognitions?.savedRecognitions?.length) return <div>No saved recognitions</div>;

  return (
    <div className="mt-6">
      <h3 className="text-xl font-bold mb-4">Saved Recognitions</h3>
      <div className="space-y-4">
        {recognitions.savedRecognitions.map((recognition, index) => (
          <div key={index} className="bg-beige-400 p-4 rounded-lg shadow">
            <Link
              to={`/library/category/${recognition.category}`}
              className="flex justify-between items-center hover:opacity-80"
            >
              <div>
                <span className="font-medium capitalize">
                  {MushroomCategory[recognition.category]}
                </span>
                <span className="text-sm text-gray-600 ml-2">
                  Confidence: {recognition.confidence}
                </span>
              </div>
              <span className="text-sm text-gray-500">
                {new Date(recognition.savedAt).toLocaleDateString()}
              </span>
            </Link>
          </div>
        ))}
      </div>
    </div>
  );
}
