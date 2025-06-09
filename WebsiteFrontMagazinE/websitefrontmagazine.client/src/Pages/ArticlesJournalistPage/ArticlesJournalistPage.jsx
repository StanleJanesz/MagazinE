import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from "framer-motion";
import Button from "react-bootstrap/Button";
import './ArticlesJournalistPage.css';
import ListTile from '../../Components/ListTile/ListTile';
import { getTokenFromCookie } from "../../utils";

/**
 * ArticlesJournalistPage component
 * Page with journalists articles, which they may browse in order to
 * edit them, send to review, or rejecr
 * @param {number} journalistId  
 */
function ArticlesJournalistPage(journalistId) {
    const [articles, setArticles] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [chosenArticleId, setChosenArticleId] = useState(Number.MAX_SAFE_INTEGER);
    const [chosenArticle, setChosenArticle] = useState('');
    const navigate = useNavigate();
    

    const fetchData = async () => {
        setIsLoading(true);
        //const articlesList = data.map(article => ({ ...article }));
        const token = getTokenFromCookie();
        fetch('https://localhost:8083/articles/journalist/', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
            }
        })
            .then((response) => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then((data) => {
                const articlesList = data.map(articleDTO => ({
                    id: articleDTO.id,
                    title: articleDTO.title,
                    tags: articleDTO.tags,
                    isPremium: articleDTO.isPremium,
                    content: articleDTO.Content
                }));               
                setArticles(articlesList);
                setIsLoading(false);
            });
       
        
        
       // setArticles(articlesList);
        setIsLoading(false);
    }

    const handleRemove = async (id) => {
        try {
            const token = getTokenFromCookie(); // Assuming you have this function

            const response = await fetch(`https://localhost:8083/articles/${id}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                if (response.status === 401) {
                    throw new Error('Unauthorized');
                } else if (response.status === 404) {
                    throw new Error('Article not found');
                } else if (response.status === 400) {
                    throw new Error('Bad request');
                } else {
                    throw new Error('Network response was not ok');
                }
            }
            setArticles(articles.filter(article => article.id !== id)); 
            // Success (200 OK)
            return true;
        } catch (error) {
            console.error('Error deleting article:', error);
            throw error;
        }
    }

    const content = isLoading ? (
        <h1>Loading</h1>
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
                        transition={{duration: 0.3} }
                    >
                        <ListTile
                            id={article.id}
                            title={article.title}
                            onSelect={(id) => {
                                setChosenArticleId(id);
                                setChosenArticle(articles.find(article => article.id === id));
                            }}
                            reject={() => handleRemove(article.id)}
                            accept={() => handleRemove(article.id)}
                            isChosen={chosenArticleId === article.id}
                        />
                    </motion.div>
            ))}
            </AnimatePresence>
    );

    const articlePreviewContent = chosenArticleId !== Number.MAX_SAFE_INTEGER ? (
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
                onClick={() => navigate(`/edit-article?article_id=${chosenArticleId}`)}
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
                    <img className="actionImage" src="src/assets/plus.png" />Add
                </button>
                {content}
            </div>
            <div className="ajp-articlePreview">Article preview
                {articlePreviewContent}
            </div>
                
        </div>
    )
}

export default ArticlesJournalistPage;