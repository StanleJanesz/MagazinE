import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import ArticleTile from '../../Components/ArticleTile/ArticleTile';
import './ArticlesJournalistPage.css';

function ArticlesJournalistPage(journalistId) {
    const [articles, setArticles] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [chosenArticle, setChosenArticle] = useState(0);

    const data = [
        { id: 0, title: "Analiza matematyczna 4 i Metody numeryczne 12 od nowego roku na MiNI", tags: "", isPremium: true },
        { id: 1, title: "Another interesting topic", tags: "", isPremium: false },
        { id: 2, title: "Another interesting topic2", tags: "", isPremium: false },
        { id: 3, title: "Another interesting topic3", tags: "", isPremium: false },
        { id: 4, title: "Another interesting topic4", tags: "", isPremium: false },
        { id: 5, title: "Another interesting topic5", tags: "", isPremium: false },
        { id: 6, title: "Another interesting topic6", tags: "", isPremium: false },
        { id: 7, title: "Another interesting topic7", tags: "", isPremium: true }
    ];

    const fetchData = async () => {
        setIsLoading(true);
        const articlesList = data.map(article => ({ ...article }));
        setArticles(articlesList);
        setIsLoading(false);
    }

    const content = isLoading ? (
        <h1>Loading</h1>
    ) : (
        <>
                {articles.map(article => (
                    <ArticleTile
                        title={article.title}
                        onClick={setChosenArticle}
                        sendToReview={() => fetch(`/review?id=${article.id}`) }
                        rejectArticle={() => fetch(`/reject?id=${article.id}`) }
                    />
            ))}
        </>
    );

    
    useEffect(() => {
        fetchData();
    }, []);


    return (
        <div className="ajp-container">
            <div className="ajp-articlesView">Available articles
                {content}
            </div>
        <div className="ajp-articlePreview">Article preview</div>
        </div>
    )
}

export default ArticlesJournalistPage;