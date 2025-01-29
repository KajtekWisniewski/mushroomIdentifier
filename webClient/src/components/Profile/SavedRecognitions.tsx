import { useEffect } from 'react';
import { useInView } from 'react-intersection-observer';
import { Link } from 'react-router-dom';
import { MushroomCategory } from '../../contracts/mushroom/mushroom';
import PaginationInfo from '../Utils/PaginationInfo';
import { ArrowRight, Trash2 } from 'lucide-react';
import { useRecognitions } from '../hooks/data-fetch/useRecognitions';

export default function SavedRecognitions() {
  const { ref, inView } = useInView();
  const {
    data,
    isLoading,
    error,
    isFetchingNextPage,
    hasNextPage,
    fetchNextPage,
    deleteBatch
  } = useRecognitions();

  useEffect(() => {
    if (inView && hasNextPage && !isFetchingNextPage) {
      fetchNextPage();
    }
  }, [inView, hasNextPage, isFetchingNextPage, fetchNextPage]);

  const handleDelete = async (batchId: string) => {
    if (!window.confirm('Are you sure you want to delete this prediction batch?')) {
      return;
    }
    try {
      await deleteBatch.mutateAsync(batchId);
    } catch (error) {
      console.error('Error deleting predictions:', error);
    }
  };

  if (isLoading) return <div>Loading saved recognitions...</div>;
  if (error) return <div>Error loading recognitions</div>;

  const isEmptyList = data?.pages[0].items.length ? true : false;

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  return (
    <div className="mt-6">
      <h3 className="text-xl font-bold mb-4">Saved Recognitions</h3>
      <div className="space-y-6">
        {data?.pages.map((page, pageIndex) => (
          <div key={pageIndex} className="space-y-6">
            {page.items.map((batch) => (
              <div
                key={batch.batchId}
                className="bg-beige-400 p-4 rounded-lg shadow group"
              >
                <div className="flex justify-between items-center mb-2">
                  <span className="text-sm text-gray-600">
                    Saved on: {formatDate(batch.savedAt)}
                  </span>
                  <button
                    onClick={() => handleDelete(batch.batchId)}
                    className="text-red-500 hover:text-red-700 opacity-0 group-hover:opacity-100 transition-opacity"
                    title="Delete predictions"
                  >
                    <Trash2 className="h-5 w-5" />
                  </button>
                </div>
                <div className="flex flex-row items-center gap-1">
                  <img
                    src="https://media.istockphoto.com/id/1147544807/vector/thumbnail-image-vector-graphic.jpg?s=612x612&w=0&k=20&c=rnCKVbdxqkjlcs3xH87-9gocETqpspHFXu5dIGB4wuM="
                    className="w-[200px] h-[244px] rounded-lg"
                    alt="Prediction thumbnail"
                  />
                  <div className="space-y-2 w-full">
                    {batch.predictions.map((recognition, idx) => (
                      <Link
                        key={idx}
                        to={`/library/category/${recognition.category}`}
                        className="flex justify-between items-center p-2 rounded hover:bg-beige-300 transition-colors border hover:scale-101"
                      >
                        <div>
                          <span className="font-medium capitalize">
                            {MushroomCategory[recognition.category]}
                          </span>
                          <span className="text-sm text-gray-600 ml-2">
                            Confidence: {recognition.confidence}
                          </span>
                        </div>
                        <ArrowRight />
                      </Link>
                    ))}
                  </div>
                </div>
              </div>
            ))}
          </div>
        ))}
        <PaginationInfo
          ref={ref}
          isLoading={isLoading}
          isFetchingNextPage={isFetchingNextPage}
          hasNextPage={hasNextPage}
          emptyList={isEmptyList}
        />
      </div>
    </div>
  );
}
