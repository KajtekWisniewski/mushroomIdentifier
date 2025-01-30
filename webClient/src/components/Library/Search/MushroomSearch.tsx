import { useEffect } from 'react';
import { useInView } from 'react-intersection-observer';
import { Link } from 'react-router-dom';
import { MushroomCategory } from '../../../contracts/mushroom/mushroom';
import { useMushroomSearch } from '../../hooks/data-fetch/useMushroomSearch';
import MushroomEntry from '../Entry/MushroomEntry';
import PaginationInfo from '../../Utils/PaginationInfo';
import { Search } from 'lucide-react';
import { useForm, Controller } from 'react-hook-form';

interface SearchFormInputs {
  searchTerm: string;
  category?: MushroomCategory;
  season: string;
  isEdible?: boolean;
}

const MushroomSearch = () => {
  const { control, watch, reset } = useForm<SearchFormInputs>({
    defaultValues: {
      searchTerm: '',
      category: undefined,
      season: '',
      isEdible: undefined
    }
  });

  const { ref, inView } = useInView();
  const searchValues = watch();

  const { data, isLoading, error, isFetchingNextPage, hasNextPage, fetchNextPage } =
    useMushroomSearch(searchValues);

  useEffect(() => {
    if (inView && hasNextPage && !isFetchingNextPage) {
      fetchNextPage();
    }
  }, [inView, hasNextPage, isFetchingNextPage, fetchNextPage]);

  const handleReset = () => {
    reset({
      searchTerm: '',
      category: undefined,
      season: '',
      isEdible: undefined
    });
  };

  const isEmptyList = data?.pages[0].items.length ? true : false;

  return (
    <div className="w-full max-w-6xl mx-auto mt-2 px-4 flex flex-col items-center">
      <div className="sticky top-10 bg-beige-500 p-4 rounded-lg shadow-lg z-10 mb-6 w-full max-w-[800px]">
        <div className="flex flex-col gap-4">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
            <Controller
              name="searchTerm"
              control={control}
              render={({ field }) => (
                <input
                  {...field}
                  type="text"
                  placeholder="Search mushrooms..."
                  className="w-full pl-10 pr-4 py-2 border rounded-lg"
                />
              )}
            />
          </div>

          <div className="flex flex-wrap gap-4">
            <Controller
              name="category"
              control={control}
              render={({ field: { onChange, value } }) => (
                <select
                  value={value?.toString() || ''}
                  onChange={(e) => {
                    const val =
                      e.target.value === ''
                        ? undefined
                        : (parseInt(e.target.value) as MushroomCategory);
                    onChange(val);
                  }}
                  className="px-4 py-2 border rounded-lg"
                >
                  <option value="">Categories</option>
                  {Object.entries(MushroomCategory)
                    .filter(([key]) => isNaN(Number(key)))
                    .map(([key, value]) => (
                      <option key={value} value={value}>
                        {key}
                      </option>
                    ))}
                </select>
              )}
            />

            <Controller
              name="season"
              control={control}
              render={({ field }) => (
                <select {...field} className="px-4 py-2 border rounded-lg">
                  <option value="">Seasons</option>
                  <option value="Spring">Spring</option>
                  <option value="Summer">Summer</option>
                  <option value="Fall">Fall</option>
                  <option value="Winter">Winter</option>
                </select>
              )}
            />

            <Controller
              name="isEdible"
              control={control}
              render={({ field: { onChange, value } }) => (
                <select
                  value={value?.toString() || ''}
                  onChange={(e) => {
                    const val =
                      e.target.value === '' ? undefined : e.target.value === 'true';
                    onChange(val);
                  }}
                  className="px-4 py-2 border rounded-lg"
                >
                  <option value="">Edibility</option>
                  <option value="true">Edible</option>
                  <option value="false">Not Edible</option>
                </select>
              )}
            />

            <button
              onClick={handleReset}
              className="px-4 py-2 bg-gray-200 rounded-lg hover:bg-gray-300"
            >
              Reset Filters
            </button>
          </div>
        </div>
      </div>

      <div className="space-y-4">
        {error && (
          <div className="text-red-500 text-center">
            Error loading results: {(error as Error).message}
          </div>
        )}

        <div>
          {data?.pages.map((page, pageIndex) => (
            <div key={pageIndex} className="animate-fade-in">
              {page.items.map((mushroom) => (
                <Link to={`/mushroom/${mushroom.id}`} key={mushroom.id}>
                  <MushroomEntry mushroom={mushroom} id={mushroom.id} listView />
                </Link>
              ))}
            </div>
          ))}
        </div>

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
};

export default MushroomSearch;
