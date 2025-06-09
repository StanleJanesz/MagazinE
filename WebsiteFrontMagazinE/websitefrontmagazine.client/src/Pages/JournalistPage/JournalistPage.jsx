import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from "framer-motion";
import Button from "react-bootstrap/Button";
import './JournalistPage.css';
import ListTile from '../../Components/ListTile/ListTile';
import { getTokenFromCookie } from '../../utils';

function JournalistPage() {
    const [articles, setArticles] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [chosenArticleId, setChosenArticleId] = useState(Number.MAX_SAFE_INTEGER);
    const [chosenArticle, setChosenArticle] = useState(null);
    const navigate = useNavigate();

    const fetchJournalistArticles = async () => {
        try {
            const token = getTokenFromCookie();
            if (!token) {
                navigate('/login');
                return;
            }

            // Fetch journalist data with article IDs
            const journalistResponse = await fetch('https://localhost:8083/api/Journalist/me', {
                method: 'GET',
                headers: {
                    'accept': 'text/plain',
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!journalistResponse.ok) {
                throw new Error('Failed to fetch journalist data');
            }

            const journalistData = await journalistResponse.json();
            const articleIds = journalistData.articlesIds || [];

            // Fetch all articles data
            const articlesPromises = articleIds.map(id =>
                fetch(`https://localhost:8083/articles/${id}`, {
                    method: 'GET',
                    headers: {
                        'accept': 'text/plain',
                        'Authorization': `Bearer ${token}`
                    }
                }).then(res => res.ok ? res.json() : null)
            );

            const articlesData = await Promise.all(articlesPromises);
            setArticles(articlesData.filter(article => article !== null));

        } catch (error) {
            console.error('Error fetching articles:', error);
        } finally {
            setIsLoading(false);
        }
    };

    const handleRemove = async (id) => {
        try {
            const token = getTokenFromCookie();
            const response = await fetch(`https://localhost:8083/articles/${id}`, {
                method: 'DELETE',
                headers: {
                    'accept': '*/*',
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.ok) {
                setArticles(prev => prev.filter(article => article.id !== id));
                if (chosenArticleId === id) {
                    setChosenArticleId(Number.MAX_SAFE_INTEGER);
                    setChosenArticle(null);
                }
            }
        } catch (error) {
            console.error('Error deleting article:', error);
        }
    };
    const handleAccept = async (id) => {
        try {
            const token = getTokenFromCookie();
            const response = await fetch(`https://localhost:8083/articles/Publish/${id}`, {
                method: 'POST',
                headers: {
                    'accept': '*/*',
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.ok) {
                setArticles(prev => prev.filter(article => article.id !== id));
                if (chosenArticleId === id) {
                    setChosenArticleId(Number.MAX_SAFE_INTEGER);
                    setChosenArticle(null);
                }
            }
        } catch (error) {
            console.error('Error deleting article:', error);
        }
    };
    const content = isLoading ? (
        <h1>Loading...</h1>
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
                        onSelect={(id) => {
                            setChosenArticleId(id);
                            setChosenArticle(articles.find(a => a.id === id));
                        }}
                        reject={() => handleRemove(article.id)}
                        accept={() => handleAccept(article.id)}
                        isChosen={chosenArticleId === article.id}
                    />
                </motion.div>
            ))}
        </AnimatePresence>
    );

    const articlePreviewContent = chosenArticle ? (
        <>
            <h1>{chosenArticle.title}</h1>
            <p>{chosenArticle.content}</p>
            {chosenArticle.photos?.length > 0 && (
                <img
                    src={`https://localhost:8083/api/Photo/${chosenArticle.photos[0]}`}
                    alt="Article"
                    style={{ maxWidth: '100%', margin: '10px 0' }}
                    onError={(e) => {
                        e.target.style.display = 'none';
                    }}
                />
            )}
            <Button
                style={{
                    background: '#E2F9B8',
                    color: '#313715',
                    border: '2px #313715 solid',
                    position: 'absolute',
                    bottom: '5%',
                    right: '5%'
                }}
                onClick={() => navigate(`/edit-article?id=${chosenArticleId}`)}
            >
                Edit
            </Button>
        </>
    ) : <p>No article selected</p>;

    useEffect(() => {
        fetchJournalistArticles();
    }, []);

    return (
        <div className="ajp-container">
            <div className="ajp-articlesView">
                <button className="actionButton" onClick={() => navigate('/edit-article')}>
                    <img className="actionImage" src="/plus.png" alt="Add" />Add
                </button>
                {content}
            </div>
            <div className="ajp-articlePreview">
                {articlePreviewContent}
            </div>
        </div>
    );
}

export default JournalistPage;