// React imports
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';

// Components imports
import NavigationBar from './Components/NavigationBar/NavigationBar.jsx'

// Pages imports
import MainPage from './Pages/HomePage/HomePage.jsx';
import LoginPage from './Pages/LoginPage/LoginPage.jsx';
import RegisterPage from './Pages/RegisterPage/RegisterPage.jsx';
import ArticlePage from './Pages/ArticlePage/ArticlePage.jsx';
import EditArticlePage from './Pages/EditArticlePage/EditArticlePage.jsx';
import ArticlesJournalistPage from './Pages/ArticlesJournalistPage/ArticlesJournalistPage.jsx';
import GeneralEditorPage from './Pages/GeneralEditorPage/GeneralEditorPage.jsx';
import AdminPanel from './Pages/AdminPanel/AdminPanel.jsx'
import ReportsView from './Pages/AdminPanel/ReportsView/ReportsView.jsx';
import BrowseArticlesAdmin from './Pages/AdminPanel/BrowseArticles/BrowseArticlesAdmin.jsx';
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
                <Route path="/article" element={<ArticlePage />} />
                <Route path="/edit-article" element={<EditArticlePage />} />
                <Route path="/articles-view" element={<ArticlesJournalistPage />} />
                <Route path="/general-editor" element={<GeneralEditorPage />} />
                <Route path="/admin-panel" element={<AdminPanel />} />
                <Route path="/admin-panel/reports" element={<ReportsView />} />
                <Route path="/admin-panel/comments" element={<BrowseArticlesAdmin />} />


            </Routes>
        </Router>
    );


}

export default App;