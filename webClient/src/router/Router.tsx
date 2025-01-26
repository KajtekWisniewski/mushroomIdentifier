import { Routes, Route } from 'react-router-dom';
import Library from '../pages/Library';
import Home from '../pages/Home';

export default function Router() {
  return (
    <Routes>
      <Route path="/" element={<Home />}></Route>
      <Route path="/library" element={<Library />} />
    </Routes>
  );
}
