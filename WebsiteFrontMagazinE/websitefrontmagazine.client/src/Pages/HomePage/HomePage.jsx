import Article from '../../Components/Article/Article';
import { useState, useEffect } from 'react';
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
    const [searchTag, setSearchTag] = useState('');
    const [selectedTags, setSelectedTags] = useState([]);
    const [searchText, setSearchText] = useState('');
    const [page, setPage] = useState(0);

    async function sleep(msec) {
        return new Promise(resolve => setTimeout(resolve, msec));
    }

    useEffect(() => {
        fetchTags();
    }, []);

    useEffect(() => {
        fetchArticles();
    }, [page, searchText, selectedTags]);

    const fetchArticles = async () => {
        setIsLoading(true);

        const params = new URLSearchParams({
            BatchSize: 10,
            Page: page,
        });

        if (searchText) {
            params.append('Title', searchText);
        }

        if (selectedTags.length > 0) {
            selectedTags.forEach(tag => {
                params.append('Tags', tag.id); 
            });
        }

        try {
            const response = await fetch(`https://localhost:8083/articles?${params.toString()}`);
            if (!response.ok) {
                throw new Error(`Failed to fetch articles ${response.status}`);
            }
            const data = await response.json();
            setArticles(data);
        } catch (error) {
            console.error(error);
        }

        setIsLoading(false);
    };

    const fetchTags = async () => {
        await sleep(500);
        try {
            const token = getTokenFromCookie();
            const response = await fetch(`https://localhost:8083/api/Tags`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`
                }
            });
            if (response.ok) {
                const data = await response.json();
                setTags(data);
            } else {
                throw new Error(await response.text());
            }
        } catch (error) {
            console.error(error);
        }
    };

    const handleSearchButtonClick = (searchTerm) => {
        setPage(0); // Reset to first page when searching
        setSearchText(searchTerm);
    };

    const filteredTags = tags.filter(tag =>
        tag.name.toLowerCase().includes(searchTag.toLowerCase())
    );

    const addTagToSelected = (tag) => {
        if (!selectedTags.some(t => t.id === tag.id)) {
            setSelectedTags(prev => [...prev, tag]);
            setPage(0); // Reset page on filter change
        }
        setSearchTag('');
    };

    const removeTagFromSelected = (tag) => {
        setSelectedTags(prev => prev.filter(t => t.id !== tag.id));
        setPage(0); // Reset page on filter change
    };

    const handleNextPage = () => setPage(prev => prev + 1);
    const handlePrevPage = () => setPage(prev => Math.max(prev - 1, 0));

    const content = isLoading ? (
        <CircularProgress />
    ) : (
        <div>
            {articles.map(article => (
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

            <div className="article-list-wrapper">{content}</div>

            <div className="pagination-controls">
                <button onClick={handlePrevPage} disabled={page === 0}>Previous</button>
                <span>Page {page + 1}</span>
                <button onClick={handleNextPage}>Next</button>
            </div>
        </div>
    );
}

export default HomePage;
