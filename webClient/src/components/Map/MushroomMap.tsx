import { useState, useCallback, useEffect } from 'react';
import { useSelector } from 'react-redux';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Lock, Trash2, Maximize2, Minimize2 } from 'lucide-react';
import { MapContainer, TileLayer, Marker, Popup, useMapEvents } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import httpClient from '../../utils/httpClient';
import { RootState } from '../../store';
import { Coordinates, SaveCoordinatesDTO } from '../../contracts/mushroom/mushroom';
import { OwnIcon, SomeoneIcon } from './MarkerIcons/MapIcons';

interface MushroomMapProps {
  mushroomId: number;
  locations: Coordinates[];
  mushroomName: string;
  initialLocationId?: number;
}

const MapClickHandler = ({
  onMapClick,
  map
}: {
  onMapClick: (latlng: L.LatLng) => void;
  map: L.Map;
}) => {
  useMapEvents({
    click: (e) => onMapClick(e.latlng)
  });

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

const MushroomMap = ({
  mushroomId,
  locations,
  mushroomName,
  initialLocationId
}: MushroomMapProps) => {
  const { user } = useSelector((state: RootState) => state.auth);
  const [selectedLocation, setSelectedLocation] = useState<L.LatLng | null>(null);
  const [isFullscreen, setIsFullscreen] = useState(false);
  const [map, setMap] = useState<L.Map | null>(null);
  const queryClient = useQueryClient();

  const addLocation = useMutation({
    mutationFn: async (coordinates: SaveCoordinatesDTO) => {
      const response = await httpClient.post(
        `/api/mushrooms/${mushroomId}/coordinates`,
        coordinates
      );
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['mushroom', mushroomId] });
      setSelectedLocation(null);
    }
  });

  const deleteLocation = useMutation({
    mutationFn: async (locationId: number) => {
      await httpClient.delete(`/api/mushrooms/${mushroomId}/coordinates/${locationId}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['mushroom', mushroomId] });
    }
  });

  const handleMapClick = useCallback(
    (latlng: L.LatLng) => {
      if (!user) return;
      setSelectedLocation(latlng);
    },
    [user]
  );

  const handleAddLocation = useCallback(() => {
    if (!selectedLocation) return;
    addLocation.mutate({
      latitude: selectedLocation.lat,
      longitude: selectedLocation.lng
    });
  }, [selectedLocation, addLocation]);

  const handleDeleteLocation = useCallback(
    (locationId: number) => {
      if (window.confirm('Are you sure you want to delete this location?')) {
        deleteLocation.mutate(locationId);
      }
    },
    [deleteLocation]
  );

  const toggleFullscreen = useCallback(() => {
    setIsFullscreen((prev) => !prev);
    setTimeout(() => {
      if (map) {
        map.invalidateSize();
      }
    }, 100);
  }, [map]);

  const initialLocation = initialLocationId
    ? locations.find((loc) => loc.id === initialLocationId)
    : locations[0];

  const mapCenter: [number, number] = initialLocation
    ? [initialLocation.latitude, initialLocation.longitude]
    : [51.505, -0.09];

  if (!user) {
    return (
      <div className="h-full w-full bg-gray-100 rounded-lg flex flex-col items-center justify-center text-gray-500">
        <Lock className="w-12 h-12 mb-2" />
        <p className="text-lg">Please log in to view and add mushroom locations</p>
      </div>
    );
  }

  const mapWrapperClasses = isFullscreen
    ? 'fixed inset-4 z-50 bg-white rounded-lg shadow-xl'
    : 'relative h-full w-full rounded-lg z-0';

  return (
    <div className={mapWrapperClasses}>
      <button
        onClick={toggleFullscreen}
        className="absolute top-4 right-4 z-[1000] bg-white p-2 rounded-lg shadow-md hover:bg-gray-100"
      >
        {isFullscreen ? (
          <Minimize2 className="w-5 h-5" />
        ) : (
          <Maximize2 className="w-5 h-5" />
        )}
      </button>
      <MapContainer
        center={mapCenter}
        zoom={13}
        scrollWheelZoom={true}
        className="h-full w-full rounded-lg"
        ref={setMap}
      >
        <TileLayer
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />
        <MapClickHandler onMapClick={handleMapClick} map={map as L.Map} />
        {locations.map((location, index) => {
          const isOwnMarker = user?.username === location.username;

          return (
            <Marker
              key={location.id || index}
              position={[location.latitude, location.longitude]}
              icon={isOwnMarker ? OwnIcon : SomeoneIcon}
            >
              <Popup>
                <div className="flex flex-col gap-2">
                  <div>
                    <p className="font-semibold">Posted by: {location.username}</p>
                    <p>
                      Lat: {location.latitude.toFixed(4)}, Lng:{' '}
                      {location.longitude.toFixed(4)}
                    </p>
                    <p>Name: {mushroomName}</p>
                  </div>
                  {isOwnMarker && (
                    <button
                      onClick={() => handleDeleteLocation(location.id)}
                      disabled={deleteLocation.isPending}
                      className="flex items-center gap-1 text-red-500 hover:text-red-700"
                    >
                      <Trash2 className="w-4 h-4" />
                      Delete
                    </button>
                  )}
                </div>
              </Popup>
            </Marker>
          );
        })}
        {selectedLocation && (
          <Marker position={[selectedLocation.lat, selectedLocation.lng]}>
            <Popup>
              <div className="flex flex-col gap-2">
                <p>New location</p>
                <button
                  onClick={handleAddLocation}
                  disabled={addLocation.isPending}
                  className="!bg-primary-800 text-white px-2 py-1 rounded hover:bg-primary-900 disabled:opacity-50 text-sm"
                >
                  {addLocation.isPending ? 'Adding...' : 'Confirm Location'}
                </button>
              </div>
            </Popup>
          </Marker>
        )}
      </MapContainer>
      {(addLocation.isError || deleteLocation.isError) && (
        <div className="absolute bottom-4 right-4 bg-red-100 text-red-700 px-4 py-2 rounded z-[1000]">
          Failed to {addLocation.isError ? 'add' : 'delete'} location. Please try again.
        </div>
      )}
    </div>
  );
};

export default MushroomMap;
