import { Coordinates } from '../../../../contracts/mushroom/mushroom';

interface LocationProps {
  locations: Coordinates[];
  className?: string;
}

export default function LocationsList({ locations, className }: LocationProps) {
  return (
    <div className={className}>
      <h2 className="font-bold">Known Locations</h2>
      <ul>
        {locations.map((location) => (
          <li key={`$location-${location.id}`}>
            Lat: {location.latitude}, Long: {location.longitude}
          </li>
        ))}
      </ul>
    </div>
  );
}
