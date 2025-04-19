import { useRecogniserHealth } from '../hooks/status-fetch/useRecogniserHealth';
import { TriangleAlert } from 'lucide-react';
import Tippy from '@tippyjs/react';
import 'tippy.js/dist/tippy.css';

export const RecogniserHealthStatus = () => {
  const { isError } = useRecogniserHealth();

  const shouldShowWarning = isError

  if (!shouldShowWarning) return null;

  return (
    <Tippy content="Mushroom recognition service disconnected" placement="top">
      <div>
        <TriangleAlert className="text-red-500 animate-pulse cursor-pointer" size={30} />
      </div>
    </Tippy>
  );
};
