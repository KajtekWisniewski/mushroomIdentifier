import { useSelector } from 'react-redux';
import { RootState } from '../../store';
import SavedRecognitions from './SavedRecognitions';

const UserProfile = () => {
  const { user } = useSelector((state: RootState) => state.auth);

  if (!user) {
    return <div>Please log in to see your profile.</div>;
  }

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

      <SavedRecognitions />
    </div>
  );
};

export default UserProfile;
