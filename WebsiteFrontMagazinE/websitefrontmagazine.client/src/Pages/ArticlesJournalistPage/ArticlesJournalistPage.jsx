import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from "motion/react";
import ArticleTile from '../../Components/ListTile/ArticleTile/ArticleTile';
import Button from "react-bootstrap/Button";
import './ArticlesJournalistPage.css';

function ArticlesJournalistPage(journalistId) {
    const [articles, setArticles] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [chosenArticleId, setChosenArticleId] = useState(Number.MAX_SAFE_INTEGER);
    const [chosenArticle, setChosenArticle] = useState('');
    const navigate = useNavigate();
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
    const articleContent = `
            
                Wydzial Matematyki i Nauk Informacyjnych Politechniki Warszawskiej oglosil, ze od nowego roku akademickiego wprowadza dwa nowe przedmioty obowiazkowe: *Analiza Matematyczna 4* oraz *Metody Numeryczne 3*. Decyzja ta jest odpowiedzia na wieloletnie postulaty studentow o "wieksze wyzwania akademickie" oraz "prawdziwe odczucie studiow inzynierskich".
                \n
                

                Po latach spekulacji, kiedy tylko zartowano o mozliwosci istnienia czwartego semestru analizy matematycznej, stalo sie to rzeczywistoscia. Nowy przedmiot obejmie:
                - Dowod, ze istnieje jeszcze jeden, trudniejszy dowod twierdzenia Stokesa,
                - Zastosowanie analizy zespolonej do gotowania makaronu,
                - Wplyw rachunku wariacyjnego na poziom stresu studentow,
                - Niezaleznosc hipotezy continuum od organizmu smiertelnego.

                Jak informuje jeden z wykladowcow: "Po trzecim semestrze analizy wielu studentow ma niedosyt. Czulismy, ze musimy im dac cos wiecej. Dlatego AM4 bedzie miala obowiazkowe projekty badawcze, a studenci na zaliczenie beda musieli napisac podrecznik do Analizy 5."`;

    const fetchData = async () => {
        setIsLoading(true);
        const articlesList = data.map(article => ({ ...article }));
        setArticles(articlesList);
        setIsLoading(false);
    }

    const handleRemove = async (id) => {
        setArticles(articles.filter(article => article.id !== id));
    }

    const content = isLoading ? (
        <h1>Loading</h1>
    ) : (   
            <AnimatePresence mode="popLayout">
                {articles.map(article => (
                    <motion.div
                        key={article.id}
                        layout
                        layoutTransition={{type: "spring", stiffness: 900, damping: 40} }
                        initial={{ opacity: 0, x: -50 }}
                        animate={{ opacity: 1, x: 0 }}
                        exit={{ opacity: 0, scale: 0 }}
                    >
                        <ArticleTile
                            id={article.id}
                            title={article.title}
                            onSelect={(id) => {
                                setChosenArticleId(id);
                                setChosenArticle(data.find(article => article.id === id));
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
            <>
                <h1>{chosenArticle.title}</h1>
                <p>{articleContent}</p>
            </>
                <Button
                    style={{
                        background: '#E2F9B8',
                        color: '#313715',
                        border: '2px #313715 solid',
                        position: 'absolute',
                        bottom: '5%',
                        right: '5%'
                        
                    }}
                    onClick={() => navigate('/edit-article')}
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