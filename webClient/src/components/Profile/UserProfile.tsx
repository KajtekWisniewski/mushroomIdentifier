import { useSelector } from 'react-redux';
import { RootState } from '../../store';

const UserProfile = () => {
  const { user } = useSelector((state: RootState) => state.auth);

  if (!user) {
    return <div>Please log in to see your profile.</div>;
  }

  return (
    <div>
      <h2>Welcome, {user.username}</h2>
      <p>Email: {user.email}</p>
      <p>Admin: {user.isAdmin ? 'Yes' : 'No'}</p>
      <h3>Saved Recognitions</h3>
      <ul>
        {user.savedRecognitions.map((category, index) => (
          <li key={index}>{category}</li>
        ))}
      </ul>
    </div>
  );
};

export default UserProfile;
