import Article from '../../Components/Article/Article';
import { useState, useEffect } from 'react';
import SearchBar from '../../Components/SearchBar/SearchBar';
import CircularProgress from '@mui/material/CircularProgress';

function HomePage() {
    const [articles, setArticles] = useState([]); 
    const [isLoading, setIsLoading] = useState(true);

    async function sleep(msec) {
        return new Promise(resolve => setTimeout(resolve, msec));
    }

    useEffect(() => {
        fetchData();
    }, []);

    const fetchData = async () => {
        setIsLoading(true);
        await sleep(1100);

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

        const articlesList = data.map(article => ({ ...article }));

        setArticles(articlesList);
        // console.log(articlesList);
        setIsLoading(false);
    };

    const content = isLoading ? (
        <CircularProgress />
    ) : (
            <div>
                {articles.map(article => (
                <Article data={article} />
            ))}
        </div>
    );

    return (
        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
            <SearchBar />
            <div style={{ minHeight: '70vh', display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                {content}
            </div>
        </div>
    );

}

export default HomePage;
