import './App.css';
import Router from './router/Router';
import NavigationBar from './components/Navigation/NavigationBar';
import BackToTop from './components/Foundation/BackToTop';

function App() {
  return (
    <>
      <NavigationBar />
      <Router />
      <BackToTop />
    </>
  );
}

export default App;
