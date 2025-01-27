import { Routes, Route } from 'react-router-dom';
import Library from '../pages/Library';
import Home from '../pages/Home';
import MushroomDetail from '../pages/MushroomDetail';
import CategoryMushroomList from '../pages/CategoryList';
import YourProfile from '../pages/Profile';
import PrivateRoute from '../components/Auth/PrivateRoute';
import Login from '../components/Auth/Login';
import Register from '../components/Auth/Register';

export default function Router() {
  return (
    <Routes>
      <Route path="/" element={<Home />}></Route>
      <Route path="/library" element={<Library />} />
      <Route path="/mushroom/:id" element={<MushroomDetail />} />
      <Route path="/library/category/:categoryId" element={<CategoryMushroomList />} />
      <Route path="/profile" element={<PrivateRoute element={<YourProfile />} />} />
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
    </Routes>
  );
}
