import Article from '../Components/Article/Article';
import { useState, useEffect } from 'react';
import SearchBar from '../Components/SearchBar/SearchBar';

function HomePage() {
    const [content, setContent] = useState(null);

    const data = [
        { title: "Analiza matematyczna 4 i Metody numeryczne 12 od nowego roku na MiNI" },
        { title: "Another interesting topic" },
        { title: "Another interesting topic2" },
        { title: "Another interesting topic3" },
        { title: "Another interesting topic4" },
        { title: "Another interesting topic5" },
        { title: "Another interesting topic6" },
        { title: "Another interesting topic7" }
    ];

    useEffect(() => {
        setContent(
            data.map((article, index) => (
                <Article articleData={article} key={index} />
            ))
        );
    }, []);
    
    return (
        <div style={{ margin: '0, auto' }}>
            <SearchBar />
            {content}
        </div>
    );
}

export default HomePage;
