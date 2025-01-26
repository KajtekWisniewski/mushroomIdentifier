import { useState } from 'react';

export default function Home() {
  const [count, setCount] = useState(0);
  return (
    <>
      <div className="flex flex-row justify-center">
        <div className="w-50 h-50 m-3 bg-beige-400 flex items-center justify-center">
          xd
        </div>
      </div>

      <div>
        <button onClick={() => setCount((count) => count + 1)}>count is {count}</button>
      </div>
    </>
  );
}
