import { MushroomDTO, MushroomCategory } from '../../../contracts/mushroom/mushroom';
import MushroomImageCarousel from './sub-entries/MushroomImageCarousel';
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
    <div className="flex flex-col max-w-[800px] m-2 rounded-2xl bg-beige-400">
      <header>
        <h1 className="font-bold">{name}</h1>
        <p className="scientific-name">{scientificName}</p>
      </header>

      <div className="flex flex-row text-left">
        <div className="flex flex-row items-center justify-center bg-black">
          <MushroomImageCarousel images={imageUrls} disabled={listView} />
        </div>
        <div className="pl-1">
          <section className="details">
            <div className="category">Category: {MushroomCategory[category]}</div>
            <div
              className={clsx(
                isEdible ? 'text-green-500' : 'text-red-500',
                'font-bold'
              )}
            >
              {isEdible ? 'Edible' : 'Not edible'}
            </div>
            <div className="habitat">Habitat: {habitat}</div>
            <div className="season">Season: {season}</div>
          </section>

          <section className="description">
            <p>{description}</p>
          </section>

          <section className="common-names">
            <h2>Common Names</h2>
            <ul>
              {commonNames.map((commonName, index) => (
                <li key={`${id}-common-${index}`}>{commonName}</li>
              ))}
            </ul>
          </section>

          {locations.length > 0 && (
            <section className="locations">
              {listView && (
                <>
                  <h2 className="font-bold">Known Locations</h2>
                  <span>Click to view location details</span>
                </>
              )}
            </section>
          )}
          <footer className="text-center">
            <small>Last updated: {new Date(lastUpdated).toLocaleDateString()}</small>
          </footer>
        </div>
      </div>
    </div>
  );
};

export default MushroomEntry;
