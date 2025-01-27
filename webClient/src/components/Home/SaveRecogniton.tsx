import { useSelector } from 'react-redux';
import { RootState } from '../../store';

export default function SaveRecognition() {
  const { user } = useSelector((state: RootState) => state.auth);
  if (!user) return <></>;

  return <button>Save recognition</button>;
}
