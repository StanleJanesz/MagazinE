import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
// Components imports
import NavigationBar from './Components/NavigationBar/NavigationBar.jsx'

// Pages imports
import MainPage from './Pages/HomePage.jsx';
import LoginPage from './Pages/LoginPage.jsx';
import RegisterPage from './Pages/RegisterPage.jsx';
import ArticlePage from './Pages/ArticlePage/ArticlePage.jsx';

// Styles imports
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';

function App() {


    return (
        <Router>
            <NavigationBar />
            <Routes>
                <Route path="/" element={<MainPage />} />
                <Route path="/register" element={<RegisterPage />} />
                <Route path="/login" element={<LoginPage />} />
                <Route path="/article" element={<ArticlePage/>} />
            </Routes>
        </Router>
    );


}

export default App;