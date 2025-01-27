import { Routes, Route } from 'react-router-dom';
import Library from '../pages/Library';
import Home from '../pages/Home';
import MushroomDetail from '../pages/MushroomDetail';
import CategoryMushroomList from '../pages/CategoryList';

export default function Router() {
  return (
    <Routes>
      <Route path="/" element={<Home />}></Route>
      <Route path="/library" element={<Library />} />
      <Route path="/mushroom/:id" element={<MushroomDetail />} />
      <Route path="/library/category/:categoryId" element={<CategoryMushroomList />} />
    </Routes>
  );
}
