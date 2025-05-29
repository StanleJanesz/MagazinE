import Article from '../../Components/Article/Article';
import { useState, useEffect} from 'react';
import SearchBar from '../../Components/SearchBar/SearchBar';
import CircularProgress from '@mui/material/CircularProgress';
import './HomePage.css'; 
import { getTokenFromCookie } from '../../utils';

/**
 * HomePage Component
 * Renders the main landing page of the application, displaying a list of articles.
 */
function HomePage() {

    const [articles, setArticles] = useState([]); 
    const [tags, setTags] = useState([]); 
    const [isLoading, setIsLoading] = useState(true);
    const [searchTag, setSearchTag] = useState("");  
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

        const params = new URLSearchParams({
            BatchSize: 10,
            Page: 0
        });

        try {
            const response = await fetch(`https://localhost:5001/articles?${params.toString()}`);

            if (!response.ok) {
                throw new Error(`Failed to fetch articles ${response.status}`);
            }

            const data = await response.json();
            console.log(data);
            const articlesList = data.map(article => ({ ...article }));

            setArticles(articlesList);
            setSearchedArticles(articlesList);

        }
        catch (error) {
            console.log(error);
        }
    };

    const fetchTags = async () => {
        await sleep(500);

        try {
            const token = getTokenFromCookie();
            const response = await fetch(`https://localhost:5001/api/Tags`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json', Authorization: `Bearer ${token}`
                }
            });
            if (response.ok) {
                const data = await response.json();
                console.log(data);
                const tagsList = data.map(tag => ({ ...tag }));
                setTags(tagsList);
            }
            else {
                throw new Error(response.text);
            }
        }
        catch (error) {
            console.log(error);
        }
        

        

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
            selectedTags.every(tag => article.tagsIds.includes(tag.id))
        )
        : searchedArticles; 



    const filteredTags = tags.filter(tag =>
        tag.name.toLowerCase().includes(searchTag.toLowerCase())
    );



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
                    <Article data={article} key={article.id} />
            ))}
        </div>
    );

    return (
        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', marginTop: '40px' }}>
            <SearchBar handleSearchButtonClick={handleSearchButtonClick} />

            <div className="large-tag-container">
                <div className="tag-search-wrapper">
                    <input
                        type="text"
                        placeholder="Wpisz tag..."
                        value={searchTag}
                        onChange={(e) => setSearchTag(e.target.value)}
                        className="tag-input"
                    />

                    <div className={filteredTags.length > 0 && searchTag.length > 0 ? 'tag-list' : 'tag-list hidden'}>
                        {filteredTags.map(tag => (
                            <span key={tag.id} className="tag-item" onClick={() => addTagToSelected(tag)}>
                                {tag.name}
                            </span>
                        ))}
                    </div>
                </div>

                {selectedTags.length > 0 && (
                    <div className="tag-container-full">
                        <h4 className="selected-tags-title">Wybrane tagi</h4>
                        <div className="tag-container-inner">
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
                    </div>
                )}
            </div>


            <div className="article-list-wrapper">
                {content}
            </div>
        </div>
    );

}

export default HomePage;
