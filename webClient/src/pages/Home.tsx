import ImageUpload from '../components/Home/ImageUpload';
import { useMutation } from '@tanstack/react-query';
import httpClient from '../utils/httpClient';
import {
  // CategoriesDTO,
  MushroomPrediction,
  MushroomCategory
} from '../contracts/mushroom/mushroom';
import { Link } from 'react-router-dom';
import SaveRecognition from '../components/Home/SaveRecogniton';
import { ArrowRight } from 'lucide-react';

// mock data endpoint
// const fetchMushroomCategories = async (): Promise<CategoriesDTO> => {
//   const { data } = await httpClient.get<CategoriesDTO>('/api/mushrooms/mock');
//   return data;
// };

export default function Home() {
  // ai model post request section
  const uploadMutation = useMutation({
    mutationFn: async (file: File) => {
      const formData = new FormData();
      formData.append('image', file);
      const response = await httpClient.post(
        'http://localhost:5000/predict',
        formData,
        {
          headers: {
            'Content-Type': 'multipart/form-data'
          },
          baseURL: ''
        }
      );
      return response.data;
    },
    onSuccess: (data) => {
      console.log('Upload successful:', data);
    },
    onError: (error) => {
      console.error('Upload failed:', error);
    }
  });

  const onImageCapture = async (file: File) => {
    try {
      const data = await uploadMutation.mutateAsync(file);
      return data;
    } catch (error) {
      console.error('Error in onImageCapture:', error);
    }
  };

  // mock data section

  // const { mutateAsync, isPending, error, data } = useMutation<
  //   CategoriesDTO,
  //   Error,
  //   void
  // >({
  //   mutationFn: fetchMushroomCategories
  // });

  // const onImageCapture = async () => {
  //   try {
  //     const data = await mutateAsync();
  //     return data;
  //   } catch (error) {
  //     console.error('Error in onImageCapture:', error);
  //   }
  // };

  const getCategoryEnumValue = (categoryId: number | string): MushroomCategory => {
    if (typeof categoryId === 'number' || !isNaN(Number(categoryId))) {
      return Number(categoryId) as MushroomCategory;
    }

    return MushroomCategory[categoryId as keyof typeof MushroomCategory];
  };

  const { data } = uploadMutation;

  return (
    // ai model section
    <div className="container mx-auto px-4">
      {uploadMutation.isPending && (
        <div className="text-center text-blue-500 my-2">Uploading image...</div>
      )}
      {uploadMutation.isError && (
        <div className="text-center text-red-500 my-2">
          Upload failed! Please try again.
        </div>
      )}
      {uploadMutation.isSuccess && (
        <div className="text-center text-green-500 my-2">Upload successful!</div>
      )}
      {/* mock section */}
      {/* {isPending && (
        <div className="text-center text-blue-500 my-2">Loading data...</div>
      )}
      {error && (
        <div className="text-center text-red-500 my-2">
          Failed to fetch data: {error.message}
        </div>
      )} */}
      {data && (
        <div className="flex flex-col gap-4 mt-6 items-center">
          {data.predictions.map((prediction: MushroomPrediction) => {
            const categoryId = getCategoryEnumValue(prediction.category);
            const categoryName = MushroomCategory[categoryId];
            return (
              <div className="w-[40%]" key={`prediction-${prediction.category}`}>
                <Link
                  to={`/library/category/${categoryId.toString()}`}
                  className="flex flex-row p-4 border rounded-lg hover:bg-primary-700 transition-colors w-full hover:scale-101"
                >
                  <div className="flex justify-between w-full items-center">
                    <div className="flex justify-between gap-4 items-center">
                      <span className="text-lg font-medium capitalize">
                        {categoryName}
                      </span>
                      <div className="flex items-center gap-4">
                        <span className="text-gray-600">
                          Confidence: {prediction.confidence}
                        </span>
                      </div>
                    </div>
                    <ArrowRight />
                  </div>
                </Link>
              </div>
            );
          })}
          <SaveRecognition predictions={data.predictions} />
        </div>
      )}
      <div className="flex flex-col items-center justify-center">
        <ImageUpload onImageCapture={onImageCapture} />
      </div>
    </div>
  );
}
