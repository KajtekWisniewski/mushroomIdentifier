import ImageUpload from '../components/Home/ImageUpload';
import { useMutation } from '@tanstack/react-query';
import httpClient from '../utils/httpClient';
import {
  CategoriesDTO,
  MushroomPrediction,
  MushroomCategory
} from '../contracts/mushroom/mushroom';
import { Link } from 'react-router-dom';

const fetchMushroomCategories = async (): Promise<CategoriesDTO> => {
  const { data } = await httpClient.get<CategoriesDTO>('/api/mushrooms/mock');
  return data;
};

export default function Home() {
  // commented out section will be used for actual image upload

  // const uploadMutation = useMutation({
  //   mutationFn: async (file: File) => {
  //     const formData = new FormData();
  //     formData.append('image', file);
  //     const response = await httpClient.post('/api/upload', formData, {
  //       headers: {
  //         'Content-Type': 'multipart/form-data'
  //       }
  //     });
  //     return response.data;
  //   },
  //   onSuccess: (data) => {
  //     console.log('Upload successful:', data);
  //   },
  //   onError: (error) => {
  //     console.error('Upload failed:', error);
  //   }
  // });

  const { mutateAsync, isPending, error, data } = useMutation<
    CategoriesDTO,
    Error,
    void
  >({
    mutationFn: fetchMushroomCategories
  });

  // const onImageCapture = async (file: File) => {
  const onImageCapture = async () => {
    try {
      const data = await mutateAsync();
      return data;
    } catch (error) {
      console.error('Error in onImageCapture:', error);
    }
  };

  const getCategoryEnumValue = (categoryName: string): MushroomCategory => {
    const normalizedName =
      categoryName.charAt(0).toUpperCase() + categoryName.slice(1).toLowerCase();
    return MushroomCategory[
      normalizedName as keyof typeof MushroomCategory
    ] as MushroomCategory;
  };

  return (
    <div className="container mx-auto px-4">
      {/* {uploadMutation.isPending && (
        <div className="text-center text-blue-500 my-2">Uploading image...</div>
      )}
      {uploadMutation.isError && (
        <div className="text-center text-red-500 my-2">
          Upload failed! Please try again.
        </div>
      )}
      {uploadMutation.isSuccess && (
        <div className="text-center text-green-500 my-2">Upload successful!</div>
      )} */}
      {isPending && (
        <div className="text-center text-blue-500 my-2">Loading data...</div>
      )}
      {error && (
        <div className="text-center text-red-500 my-2">
          Failed to fetch data: {error.message}
        </div>
      )}
      {data && (
        <div className="flex flex-col gap-4 mt-6 items-center">
          {data.predictions.map((prediction: MushroomPrediction, index: number) => {
            const categoryId = getCategoryEnumValue(prediction.category);
            return (
              <Link
                key={index}
                to={`/library/category/${categoryId.toString()}`}
                className="flex flex-row gap-4 p-4 border rounded-lg hover:bg-gray-50 transition-colors"
              >
                <div className="flex justify-between gap-4 items-center">
                  <span className="text-lg font-medium capitalize">
                    {prediction.category}
                  </span>
                  <div className="flex items-center gap-4">
                    <span className="text-gray-600">
                      Confidence: {prediction.confidence}%
                    </span>
                    <span className="text-blue-500">→</span>
                  </div>
                </div>
              </Link>
            );
          })}
        </div>
      )}
      <ImageUpload onImageCapture={onImageCapture} />
    </div>
  );
}
