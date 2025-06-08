// React imports
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { CSSTransition, TransitionGroup } from 'react-transition-group';

// Components imports
import NavigationBar from './Components/NavigationBar/NavigationBar.jsx'

// Pages imports
import MainPage from './Pages/HomePage/HomePage.jsx';
import LoginPage from './Pages/LoginPage/LoginPage.jsx';
import RegisterPage from './Pages/RegisterPage/RegisterPage.jsx';
import ArticlePage from './Pages/ArticlePage/ArticlePage.jsx';
import EditArticlePage from './Pages/EditArticlePage/EditArticlePage.jsx';
import ArticlesJournalistPage from './Pages/ArticlesJournalistPage/ArticlesJournalistPage.jsx';
import AdminPage from './Pages/AdminPage/AdminPage.jsx';
import GeneralEditorPage from './Pages/GeneralEditorPage/GeneralEditorPage.jsx';
import ErrorPage from './Pages/ErrorPage/ErrorPage.jsx';

// Styles imports
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';


function App() {
    return (
        <Router>
            <NavigationBar />
            <TransitionGroup>
                <CSSTransition key={location.key} classNames="fade" timeout={300}>
                    <Routes>
                        <Route path="/" element={<MainPage />} />
                        <Route path="/register" element={<RegisterPage />} />
                        <Route path="/login" element={<LoginPage />} />
                        <Route path="/article" element={<ArticlePage />} />
                        <Route path="/edit-article" element={<EditArticlePage />} />
                        <Route path="/articles-view" element={<ArticlesJournalistPage />} />
                        <Route path="/general-editor" element={<GeneralEditorPage />} />
                        <Route path="/admin-requests" element={<AdminPage />} />
                        <Route path="*" element={<ErrorPage/> }/>
                    </Routes>
                </CSSTransition>
            </TransitionGroup>
        </Router>
    );


}

export default App;