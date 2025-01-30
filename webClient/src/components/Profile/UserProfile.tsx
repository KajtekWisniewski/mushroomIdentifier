import { useSelector } from 'react-redux';
import { RootState } from '../../store';
import { useState } from 'react';
import SavedRecognitions from './SavedRecognitions';
import UserLocationsMap from '../Map/UserLocationsMap';

const UserProfile = () => {
  const { user } = useSelector((state: RootState) => state.auth);
  const [hideRecognitions, setHideRecognitions] = useState<boolean>(false);
  const [hideMap, setHideMap] = useState<boolean>(false);

  if (!user) {
    return <div>Please log in to see your profile.</div>;
  }

  const handleHideRecognitions = () => {
    setHideRecognitions(!hideRecognitions);
  };

  const handleHideMap = () => {
    setHideMap(!hideMap);
  };

  return (
    <div className="container mx-auto px-4 py-6 max-w-2xl">
      <div className="bg-beige-400 rounded-lg p-6 mb-6">
        <h2 className="text-2xl font-bold mb-4">Profile Information</h2>
        <div className="space-y-2">
          <p>
            <span className="font-medium">Username:</span> {user.username}
          </p>
          <p>
            <span className="font-medium">Email:</span> {user.email}
          </p>
          <p>
            <span className="font-medium">Role:</span> {user.isAdmin ? 'Admin' : 'User'}
          </p>
        </div>
      </div>

      <button onClick={handleHideMap} className="mb-2">
        Toggle map
      </button>
      {!hideMap && (
        <>
          <h3 className="text-xl font-bold mb-4">Reported Findings Map</h3>{' '}
          <UserLocationsMap />
        </>
      )}
      <button onClick={handleHideRecognitions} className="mt-2">
        Toggle Recognitions
      </button>
      {!hideRecognitions && <SavedRecognitions />}
    </div>
  );
};

export default UserProfile;
