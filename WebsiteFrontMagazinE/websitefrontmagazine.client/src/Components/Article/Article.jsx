import './Article.css';
import { useNavigate } from 'react-router-dom';

function Article(articleData) {
    const navigate = useNavigate();
    
    return (
        <div className="articleContainer" onClick={() => navigate(`/article`)}>
            <img
                src={"src/assets/mini.jpg"}
                alt={"TODO"}
                className="articleImage"
                onError={(e) => {
                    e.target.onerror = null; 
                    e.target.src = "/vite.svg"; // TODO: change to some default
                }}
            />
            <h2 className="articleTitle">Analiza matematyczna 4 i Metody numeryczne 12 od nowego roku na MiNI</h2>
            <p className="carDescription">
            {/*    HERE WILL BE TAGS?*/}
                </p>  
        </div>
    );
}

export default Article;
