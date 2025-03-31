import Article from '../../Components/Article/Article';
import { useState, useEffect} from 'react';
import SearchBar from '../../Components/SearchBar/SearchBar';
import CircularProgress from '@mui/material/CircularProgress';
import './HomePage.css'; // Zaimportuj CSS

function HomePage() {

    const [articles, setArticles] = useState([]); 
    const [tags, setTags] = useState([]); 
    const [isLoading, setIsLoading] = useState(true);
    const [searchTag, setSearchTag] = useState("");  
    const [filteredTags, setFilteredTags] = useState([]); 
    const [selectedTags, setSelectedTags] = useState([]); 
    const [searchText, setSearchText] = useState(""); 
    const [searchedArticles, setSearchedArticles] = useState([]); 


    async function sleep(msec) {
        return new Promise(resolve => setTimeout(resolve, msec));
    }

    useEffect(() => {
        fetchArticle();
        fetchTags();
    }, []);

    const fetchArticle = async () => {
        setIsLoading(true);
        await sleep(1100);

        const data = [
            { id: 0, title: "Analiza matematyczna 4 i Metody numeryczne 12 od nowego roku na MiNI", tags: [1, 2, 3, 4], isPremium: true },
            { id: 1, title: "Another interesting topic", tags: [10, 12], isPremium: false },
            { id: 2, title: "Another interesting topic2", tags: [5, 7], isPremium: false },
            { id: 3, title: "Another interesting topic3", tags: [8, 13], isPremium: false },
            { id: 4, title: "Another interesting topic4", tags: [6, 14], isPremium: false },
            { id: 5, title: "Another interesting topic5", tags: [8, 9], isPremium: false },
            { id: 6, title: "Another interesting topic6", tags: [10, 12], isPremium: false },
            { id: 7, title: "Another interesting topic7", tags: [5, 6, 7], isPremium: true }
        ];

        const articlesList = data.map(article => ({ ...article }));

        setArticles(articlesList);
        setSearchedArticles(articlesList);
        // console.log(articlesList);
        //setIsLoading(false); -> teraz w next funcji
    };

    const fetchTags = async () => {
        await sleep(500);

        const data = [
            { id: 1, name: "MINI" },
            { id: 2, name: "PW" },
            { id: 3, name: "Analiza Matematyczna" },
            { id: 4, name: "Metody Numeryczne" },
            { id: 5, name: "Polityka" },
            { id: 6, name: "Polska" },
            { id: 7, name: "Swiat" },
            { id: 8, name: "Nauka" },
            { id: 9, name: "Rosliny" },
            { id: 10, name: "Celebryci" },
            { id: 11, name: "Historia" },
            { id: 12, name: "Moda" },
            { id: 13, name: "Kosmos" },
            { id: 14, name: "Sport" }
        ];

        const tagsList = data.map(tag => ({ ...tag}));
        setTags(tagsList);

        setIsLoading(false);
    };

    const handleSearchButtonClick = (searchTerm) => {
        setSearchText(searchTerm); 

        if (!searchTerm) {
            setSearchedArticles(articles);
        }

        const filteredArticles = articles.filter(article =>
            article.title.toLowerCase().includes(searchTerm.toLowerCase())
        );

        setSearchedArticles(filteredArticles);
    };

    
    const filteredArticles = selectedTags.length > 0
        ? searchedArticles.filter(article =>
            selectedTags.every(tag => article.tags.includes(tag.id))
        )
        : searchedArticles; 



    useEffect(() => {
        const results = tags.filter(tag =>
            tag.name.toLowerCase().includes(searchTag.toLowerCase())
        );
        setFilteredTags(results);
    }, [searchTag, tags]);


    const addTagToSelected = (tag) => {
        if (!selectedTags.some(t => t.id === tag.id)) {
            setSelectedTags([...selectedTags, tag]);
        }

        setSearchTag('');
    };


    const removeTagFromSelected = (tag) => {
        setSelectedTags(selectedTags.filter(t => t.id !== tag.id));
    };



    const content = isLoading ? (
        <CircularProgress />
    ) : (
            <div>
                {filteredArticles.map(article => (
                <Article data={article} />
            ))}
        </div>
    );

    return (
        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', marginTop: '40px' }}>
            <SearchBar handleSearchButtonClick={handleSearchButtonClick} />

            <div className="large-tag-container">
                <input
                    type="text"
                    placeholder="Wpisz tag..."
                    value={searchTag}
                    onChange={(e) => setSearchTag(e.target.value)}
                    className="tag-input" 
                />


                <div className={filteredTags.length > 0 && searchTag.length>0  ? 'tag-list' : 'tag-list hidden'}>
                    {filteredTags.map(tag => (
                        <span key={tag.id} className="tag-item" onClick={() => addTagToSelected(tag)}>
                            {tag.name}
                        </span>
                    ))}
                </div>


                {selectedTags.length > 0 && (
                    <div className="tag-container">
                        <h4 className="selected-tags-title">Wybrane tagi</h4>
                        {selectedTags.map(tag => (
                            <span key={tag.id} className="tag-badge">
                                {tag.name}
                                <button
                                    onClick={() => removeTagFromSelected(tag)}
                                    className="tag-remove-button"
                                >
                                    X
                                </button>
                            </span>
                        ))}
                        </div>
                
                    )}
            </div>


            <div style={{ minHeight: '30vw', display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                {content}
            </div>
        </div>
    );

}

export default HomePage;
