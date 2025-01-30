import { MushroomDTO, MushroomCategory } from '../../../contracts/mushroom/mushroom';
import MushroomImageCarousel from './sub-entries/MushroomImageCarousel';
import { MapPin, Calendar, Home } from 'lucide-react';
import clsx from 'clsx';

interface MushroomEntryProps {
  mushroom: MushroomDTO;
  id: number;
  listView?: boolean;
}

const MushroomEntry = ({ mushroom, id, listView }: MushroomEntryProps) => {
  const {
    name,
    scientificName,
    category,
    description,
    isEdible,
    habitat,
    season,
    commonNames,
    imageUrls,
    locations,
    lastUpdated
  } = mushroom;

  return (
    <div className="m-2 rounded-4xl">
      <div className="max-w-3xl bg-beige-300 rounded-xl overflow-hidden shadow-lg">
        <div className="p-6 bg-beige-500 border-b border-beige-300">
          <h1 className="text-2xl font-bold text-gray-800">{name}</h1>
          <p className="text-gray-600 italic mt-1">{scientificName}</p>
        </div>

        <div className="flex flex-col md:flex-row">
          <div className="md:w-1/2 p-4">
            <MushroomImageCarousel
              images={imageUrls}
              disabled={listView}
              className="rounded-lg overflow-hidden shadow-md"
            />
          </div>

          <div className="md:w-1/2 p-6 space-y-4">
            <div className="flex items-center gap-2 flex-wrap">
              <span className="px-3 py-1 bg-beige-200 rounded-full text-sm">
                Category: {MushroomCategory[category]}
              </span>
              <span
                className={clsx(
                  'px-3 py-1 rounded-full text-sm font-medium',
                  isEdible
                    ? 'bg-green-100 text-green-700'
                    : 'bg-red-100 text-red-700 !font-bold'
                )}
              >
                {isEdible ? 'Edible' : 'Not Edible'}
              </span>
            </div>

            <div className="space-y-2">
              <div className="flex items-center gap-2 text-gray-700">
                <Home className="w-4 h-4" />
                <span>{habitat}</span>
              </div>
              <div className="flex items-center gap-2 text-gray-700">
                <Calendar className="w-4 h-4" />
                <span>{season}</span>
              </div>
            </div>

            <p className="text-gray-700 py-2 border-t border-b border-beige-200">
              {description}
            </p>

            <div>
              <h3 className="text-sm font-semibold text-gray-600 mb-2">Common Names</h3>
              <div className="flex flex-wrap gap-2">
                {commonNames.map((commonName, index) => (
                  <span
                    key={`${id}-common-${index}`}
                    className="px-2 py-1 bg-beige-200 rounded-lg text-sm"
                  >
                    {commonName}
                  </span>
                ))}
              </div>
            </div>

            {locations.length > 0 && (
              <div>
                <button className="flex items-center gap-2 text-primary-800 hover:text-primary-900">
                  <MapPin className="w-4 h-4" />
                  <span>
                    {listView
                      ? 'View Known Locations'
                      : 'Check known Locations on the map'}
                  </span>
                </button>
              </div>
            )}

            <div className="text-xs text-gray-500 mt-4">
              Last updated: {new Date(lastUpdated).toLocaleDateString()}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default MushroomEntry;
