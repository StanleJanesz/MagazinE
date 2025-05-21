import Article from '../../Components/Article/Article';
import { useState, useEffect } from 'react';
import CircularProgress from '@mui/material/CircularProgress';
import './UserArticlesPage.css';

function UserArticlesPage() {

    const [articles, setArticles] = useState([]);
    const [tags, setTags] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [selectedTags, setSelectedTags] = useState([]);
    const [searchedArticles, setSearchedArticles] = useState([]);
    const [currentUser, setCurrentUser] = useState({});

    async function sleep(msec) {
        return new Promise(resolve => setTimeout(resolve, msec));
    }

    useEffect(() => {
        fetchArticle();
        fetchTags();
        fetchUser();
    }, []);

    const fetchArticle = async () => {
        setIsLoading(true);
        await sleep(1100);

        const data = [
            { id: 0, title: "Analiza matematyczna 4", tags: [1, 2, 3], isPremium: true, category: 'favorite' },
            { id: 1, title: "Another topic", tags: [10, 12], isPremium: false, category: 'toread' },
            { id: 2, title: "Numerical Methods", tags: [4, 7], isPremium: false, category: 'toread' },
            { id: 3, title: "React tips", tags: [21, 22], isPremium: true, category: 'favorite' },
            { id: 4, title: "Fizyka dla ka¿dego", tags: [19], isPremium: false, category: 'toread' },
            { id: 5, title: "Moda i kosmos", tags: [12, 13], isPremium: false, category: 'favorite' }
        ];

        setArticles(data);
        setSearchedArticles(data);
    };

    const fetchTags = async () => {
        await sleep(500);
        const data = [
            { id: 1, name: "MINI" },
            { id: 2, name: "PW" },
            { id: 3, name: "Analiza" },
            { id: 4, name: "Numeryczne" },
            { id: 7, name: "Swiat" },
            { id: 10, name: "Celebryci" },
            { id: 12, name: "Moda" },
            { id: 13, name: "Kosmos" },
            { id: 19, name: "Fizyka" },
            { id: 21, name: "IT" },
            { id: 22, name: "Technologia" }
        ];

        setTags(data);
        setIsLoading(false);
    };

    const fetchUser = async () => {
        await sleep(500);
        setCurrentUser({ 'name': "Andrew" });
    };


    const filteredArticles = selectedTags.length > 0
        ? searchedArticles.filter(article =>
            selectedTags.every(tag => article.tags.includes(tag.id))
        )
        : searchedArticles;


    const renderArticlesSection = (label, category) => (
        <div className="article-section">
            <h2>{label}</h2>
            {isLoading ? (
                <CircularProgress />
            ) : (
                filteredArticles
                    .filter(article => article.category === category)
                    .map(article => <Article key={article.id} data={article} />)
            )}
        </div>
    );

    return (
        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', marginTop: '40px' }}>
            <h2 className='user-welcome'>  Welcome {currentUser.name}</h2>

           

            <div className="article-list-wrapper">
                {renderArticlesSection("Ulubione", "favorite")}
                {renderArticlesSection("Do przeczytania", "toread")}
            </div>
        </div>
    );
}

export default UserArticlesPage;
