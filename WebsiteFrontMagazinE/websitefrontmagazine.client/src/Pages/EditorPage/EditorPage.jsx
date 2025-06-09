import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from "motion/react";
import Button from "react-bootstrap/Button";
import '../ArticlesJournalistPage/ArticlesJournalistPage.css';
import ListTile from '../../Components/ListTile/ListTile';
import { getTokenFromCookie } from '../../utils';

function EditorPage() {
    const [articles, setArticles] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    const [chosenArticleId, setChosenArticleId] = useState(Number.MAX_SAFE_INTEGER);
    const [chosenArticle, setChosenArticle] = useState('');
    const navigate = useNavigate();

    const API_BASE_URL = 'https://localhost:8083';
    const API_URL = `${API_BASE_URL}/articles`;
    const EDITOR_API_URL = `${API_BASE_URL}/articles/editor`;

    const buildQueryParams = () => {
        const params = new URLSearchParams();

        //if (filters.Tags && filters.Tags.length > 0) {
        //    filters.Tags.forEach(tagId => params.append('Tags', tagId));
        //}

        //if (filters.Title) {
        //    params.append('Title', filters.Title);
        //}

        params.append('BatchSize',  10);
        params.append('Page', 0);

        return params.toString();
    };

    const fetchData = async () => {
        setIsLoading(true);
        setError(null);

        try {
            const token = getTokenFromCookie();
            const query = buildQueryParams();
            console.log(token);
            const response = await fetch(`https://localhost:8083/articles/editor?${query}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`, 
                },
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();


            const articlesList = Array.isArray(data) ? data : data.articles || [];

            setArticles(articlesList);
        } catch (err) {
            console.error('Error fetching articles:', err);
            setError(err.message);

            // Fallback to mock data in case of error (optional)
            const fallbackData = [
                { id: 0, title: "Analiza matematyczna 4 i Metody numeryczne 12 od nowego roku na MiNI", tags: "", isPremium: true },
                { id: 1, title: "Another interesting topic", tags: "", isPremium: false },
                { id: 2, title: "Another interesting topic2", tags: "", isPremium: false },
                { id: 3, title: "Another interesting topic3", tags: "", isPremium: false },
                { id: 4, title: "Another interesting topic4", tags: "", isPremium: false },
                { id: 5, title: "Another interesting topic5", tags: "", isPremium: false },
                { id: 6, title: "Another interesting topic6", tags: "", isPremium: false },
                { id: 7, title: "Another interesting topic7", tags: "", isPremium: true }
            ];
            setArticles(fallbackData);
        } finally {
            setIsLoading(false);
        }
    };

    const fetchArticleContent = async (articleId) => {
        try {
            // Use the specific article endpoint to get full content
            const response = await fetch(`${API_URL}/${articleId}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${getTokenFromCookie()}`,
                },
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const contentData = await response.json();
            return contentData.content || contentData;
        } catch (err) {
            console.error('Error fetching article content:', err);
            // Return fallback content
            return `
                Wydzial Matematyki i Nauk Informacyjnych Politechniki Warszawskiej oglosil, ze od nowego roku akademickiego wprowadza dwa nowe przedmioty obowiazkowe: *Analiza Matematyczna 4* oraz *Metody Numeryczne 3*. Decyzja ta jest odpowiedzia na wieloletnie postulaty studentow o "wieksze wyzwania akademickie" oraz "prawdziwe odczucie studiow inzynierskich".
                \n
                Po latach spekulacji, kiedy tylko zartowano o mozliwosci istnienia czwartego semestru analizy matematycznej, stalo sie to rzeczywistoscia. Nowy przedmiot obejmie:
                - Dowod, ze istnieje jeszcze jeden, trudniejszy dowod twierdzenia Stokesa,
                - Zastosowanie analizy zespolonej do gotowania makaronu,
                - Wplyw rachunku wariacyjnego na poziom stresu studentow,
                - Niezaleznosc hipotezy continuum od organizmu smiertelnego.

                Jak informuje jeden z wykladowcow: "Po trzecim semestrze analizy wielu studentow ma niedosyt. Czulismy, ze musimy im dac cos wiecej. Dlatego AM4 bedzie miala obowiazkowe projekty badawcze, a studenci na zaliczenie beda musieli napisac podrecznik do Analizy 5."`;
        }
    };

    const handleRemove = async (id) => {
        try {
            // Make DELETE request to API
            const response = await fetch(`${API_URL}/${id}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${getTokenFromCookie()}`,
                },
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            // Remove from local state after successful API call
            setArticles(articles.filter(article => article.id !== id));

            // Clear selection if the deleted article was selected
            if (chosenArticleId === id) {
                setChosenArticleId(Number.MAX_SAFE_INTEGER);
                setChosenArticle('');
            }
        } catch (err) {
            console.error('Error deleting article:', err);
            // Optionally show error message to user
            alert('Failed to delete article. Please try again.');
        }
    };

    const handleArticleSelect = async (id) => {
        setChosenArticleId(id);
        const selectedArticle = articles.find(article => article.id === id);

        if (selectedArticle) {
            // Fetch the full article content
            const content = await fetchArticleContent(id);
            setChosenArticle({
                ...selectedArticle,
                content: content
            });
        }
    };

    const content = isLoading ? (
        <div className="loading-container">
            <h1>Loading unpublished articles...</h1>
        </div>
    ) : error && articles.length === 0 ? (
        <div className="error-container">
            <h2>Error loading unpublished articles</h2>
            <p>{error}</p>
            <button onClick={fetchData} className="retry-button">
                Retry
            </button>
        </div>
    ) : articles.length === 0 ? (
        <div className="no-articles-container">
            <h2>No unpublished articles</h2>
            <p>All articles have been reviewed and published.</p>
        </div>
    ) : (
        <AnimatePresence>
            {articles.map(article => (
                <motion.div
                    key={article.id}
                    layout
                    layoutTransition={{ type: "spring", stiffness: 900, damping: 40 }}
                    initial={{ opacity: 0, x: -20 }}
                    animate={{ opacity: 1, x: 0 }}
                    exit={{ opacity: 0, scale: 0 }}
                    transition={{ duration: 0.3 }}
                >
                    <ListTile
                        id={article.id}
                        title={article.title}
                        onSelect={handleArticleSelect}
                        reject={() => handleRemove(article.id)}
                        accept={() => handleRemove(article.id)}
                        isChosen={chosenArticleId === article.id}
                    />
                </motion.div>
            ))}
        </AnimatePresence>
    );

    const articlePreviewContent = chosenArticleId !== Number.MAX_SAFE_INTEGER && chosenArticle ? (
        <>
            <h1>{chosenArticle.title}</h1>
            <p>{chosenArticle.content}</p>

            <Button
                style={{
                    background: '#E2F9B8',
                    color: '#313715',
                    border: '2px #313715 solid',
                    position: 'absolute',
                    bottom: '5%',
                    right: '5%'
                }}
                onClick={() => navigate('/edit-article', { state: { article: chosenArticle } })}
            >
                Edit
            </Button>
        </>
    ) : <p>No article selected</p>;

    useEffect(() => {
        fetchData();
    }, []);

    return (
        <div className="ajp-container">
            <div className="ajp-articlesView">
                <button className="actionButton" onClick={() => navigate('/edit-article')}>
                    <img className="actionImage" src="src/assets/plus.png" alt="Add" />
                    Add
                </button>
                {content}
            </div>
            <div className="ajp-articlePreview">
                Unpublished Article Preview
                {articlePreviewContent}
            </div>
        </div>
    );
}

export default EditorPage;