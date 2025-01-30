import { useState, useCallback, useEffect } from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMapEvents } from 'react-leaflet';
import { useUserLocations } from '../hooks/data-fetch/useUserLocations';
import { Link } from 'react-router-dom';
import { Maximize2, Minimize2 } from 'lucide-react';
import 'leaflet/dist/leaflet.css';
import { Icon } from 'leaflet';

const MapEventHandler = ({ map }: { map: L.Map }) => {
  useMapEvents({});

  useEffect(() => {
    if (map) {
      const timer = setTimeout(() => {
        map.invalidateSize();
      }, 100);
      return () => clearTimeout(timer);
    }
  }, [map]);

  return null;
};

const UserLocationsMap = () => {
  const { data: locations, isLoading, error } = useUserLocations();
  const [isFullscreen, setIsFullscreen] = useState(false);
  const [map, setMap] = useState<L.Map | null>(null);

  const toggleFullscreen = useCallback(() => {
    setIsFullscreen((prev) => !prev);
    setTimeout(() => {
      if (map) {
        map.invalidateSize();
      }
    }, 100);
  }, [map]);

  if (isLoading) return <div>Loading locations...</div>;
  if (error) return <div>Error loading locations</div>;
  if (!locations?.length) return <div>No locations saved yet</div>;

  const initialLocation = locations[0].coordinates[0];

  const MuhsroomIcon = (iconUrl: string) =>
    new Icon({
      iconUrl: iconUrl,
      shadowUrl:
        'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
      iconSize: [36, 36],
      iconAnchor: [12, 41],
      popupAnchor: [1, -34],
      shadowSize: [41, 41]
    });

  const containerClasses = isFullscreen
    ? 'fixed inset-4 z-50 bg-white rounded-lg shadow-xl'
    : 'h-96 w-full rounded-lg overflow-hidden';

  return (
    <div className={containerClasses}>
      <div className="relative h-full">
        <div className="absolute top-4 right-4 z-[1000] flex gap-2">
          <button
            onClick={toggleFullscreen}
            className="bg-white p-2 rounded-lg shadow-md hover:bg-gray-100"
          >
            {isFullscreen ? (
              <Minimize2 className="w-5 h-5" />
            ) : (
              <Maximize2 className="w-5 h-5" />
            )}
          </button>
        </div>

        <MapContainer
          center={[initialLocation.latitude, initialLocation.longitude]}
          zoom={13}
          scrollWheelZoom={true}
          className="h-full w-full"
          ref={setMap}
        >
          <MapEventHandler map={map as L.Map} />
          <TileLayer
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />
          {locations.map((location) =>
            location.coordinates.map((coord) => (
              <Marker
                key={coord.id}
                position={[coord.latitude, coord.longitude]}
                icon={MuhsroomIcon(location.mushroomImages[0])}
              >
                <Popup>
                  <div className="flex flex-col gap-2">
                    <Link
                      to={`/mushroom/${location.mushroomId}/${coord.id}`}
                      className="font-semibold hover:text-primary-800"
                    >
                      {location.mushroomName}
                    </Link>
                    {location.mushroomImages[0] && (
                      <img
                        src={location.mushroomImages[0]}
                        alt={location.mushroomName}
                        className="w-32 h-32 object-cover rounded"
                      />
                    )}
                  </div>
                </Popup>
              </Marker>
            ))
          )}
        </MapContainer>
      </div>
    </div>
  );
};

export default UserLocationsMap;
