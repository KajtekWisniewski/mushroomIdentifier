import { useMutation } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { ArrowRight, Upload, AlertCircle, CheckCircle2, AlertTriangle } from 'lucide-react';
import { useEffect } from 'react';
import { MushroomPrediction, MushroomCategory } from '../contracts/mushroom/mushroom';
import ImageUpload from '../components/Home/ImageUpload';
import SaveRecognition from '../components/Home/SaveRecogniton';
import httpClient from '../utils/httpClient';
import { useRecogniserHealth } from '../components/hooks/status-fetch/useRecogniserHealth';

export default function Home() {

  const { isError } = useRecogniserHealth();

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

  useEffect(() => {
    if (uploadMutation.isSuccess) {
      window.scrollTo({
        top: document.documentElement.scrollHeight,
        behavior: 'smooth'
      });
    }

    return () => {
      window.scrollTo({
        top: 0,
        behavior: 'instant'
      });
    };
  }, [uploadMutation.isSuccess]);

  const onImageCapture = async (file: File) => {
    try {
      const data = await uploadMutation.mutateAsync(file);
      return data;
    } catch (error) {
      console.error('Error in onImageCapture:', error);
    }
  };

  const getCategoryEnumValue = (categoryId: number | string): MushroomCategory => {
    if (typeof categoryId === 'number' || !isNaN(Number(categoryId))) {
      return Number(categoryId) as MushroomCategory;
    }
    return MushroomCategory[categoryId as keyof typeof MushroomCategory];
  };

  const { data } = uploadMutation;

  return (
    <div className="bg-gradient-to-b from-primary-900/10 to-transparent">
      <div className="max-w-7xl mx-auto px-4 py-12">
        <div className="text-center mb-12">
          <h1 className="text-4xl font-bold text-primary-900 mb-4">
            Mushroom Recognition
          </h1>
          <p className="text-lg text-primary-800 max-w-2xl mx-auto">
            Upload a photo of a mushroom to identify its species and get detailed
            information about its characteristics.
          </p>
        </div>

        <div className="max-w-md mx-auto mb-8">
          {uploadMutation.isPending && (
            <div className="flex items-center justify-center gap-2 text-primary-600 bg-primary-300 p-4 rounded-lg">
              <Upload className="w-5 h-5 animate-spin" />
              <span>Processing your image...</span>
            </div>
          )}
          {uploadMutation.isError && (
            <div className="flex items-center justify-center gap-2 text-red-600 bg-red-50 p-4 rounded-lg">
              <AlertCircle className="w-5 h-5" />
              <span>Upload failed! Please try again.</span>
            </div>
          )}
          {uploadMutation.isSuccess && (
            <div className="flex items-center justify-center gap-2 text-green-600 bg-green-50 p-4 rounded-lg">
              <CheckCircle2 className="w-5 h-5" />
              <span>Analysis complete!</span>
            </div>
          )}
        </div>

        <div className="max-w-2xl mx-auto mb-12 bg-beige-500 p-8 rounded-2xl shadow-lg">
          {isError ? 
            <div className='flex flex-row gap-4 items-center justify-center'>
              <AlertTriangle className="text-red-500 animate-pulse" size={80}/> 
              <div>Recognition service disconnected</div>
            </div> 
            :
            <ImageUpload onImageCapture={onImageCapture} /> }
        </div>

        {data && (
          <div className="max-w-3xl mx-auto">
            <h2 className="text-2xl font-semibold text-primary-800 mb-6 text-center">
              Analysis Results
            </h2>
            <div className="flex justify-center mb-2">
              <SaveRecognition predictions={data.predictions} />
            </div>
            <div className="space-y-4 mb-8">
              {data.predictions.map((prediction: MushroomPrediction) => {
                const categoryId = getCategoryEnumValue(prediction.category);
                const categoryName = MushroomCategory[categoryId];
                const confidence = parseFloat(prediction.confidence);

                return (
                  <div key={`prediction-${prediction.category}`}>
                    <Link
                      to={`/library/category/${categoryId.toString()}`}
                      className="block bg-beige-100 hover:bg-beige-200 rounded-xl shadow-sm
                               transition-all duration-200 hover:shadow-md"
                    >
                      <div className="p-6 flex items-center justify-between">
                        <div className="flex items-center gap-6">
                          <div className="flex flex-col">
                            <span className="text-xl font-medium text-primary-900 capitalize">
                              {categoryName}
                            </span>
                            <div className="flex items-center gap-2 mt-1">
                              <div className="w-24 h-2 bg-beige-200 rounded-full overflow-hidden">
                                <div
                                  className="h-full bg-primary-600 rounded-full"
                                  style={{ width: `${confidence}%` }}
                                />
                              </div>
                              <span className="text-sm text-primary-600">
                                {prediction.confidence}
                              </span>
                            </div>
                          </div>
                        </div>
                        <ArrowRight className="text-primary-600" />
                      </div>
                    </Link>
                  </div>
                );
              })}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
