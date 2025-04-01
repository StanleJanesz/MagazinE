import { useEffect, useState } from "react";
import CircularProgress from '@mui/material/CircularProgress';
import Box from '@mui/material/Box';
import './ArticlePage.css';
import { useNavigate } from 'react-router-dom';
import Comment from '../../Components/Comment/Comment.jsx';
import { Button } from "bootstrap";
function ArticlePage({ articleId }) {
    // TODO: PARSER BALD AND ITALICS
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [comments, setComments] = useState([]);
    const navigate = useNavigate();
    const getRandomInt = (max) => Math.floor(Math.random() * max);
    async function sleep(msec) {
        return new Promise(resolve => setTimeout(resolve, msec));
    }

    const dummyComments = [
        { author: "Jeanne Barbe", date: Date.now(), content: "Great news! I believe it will improve the results from students surveys!" },
        { author: "Paul deCat", date: Date.now(), content: "I'm excited about these new courses. Looking forward to AM4!" },
        { author: "Mr. Captain", date: Date.now(), content: "Will the new subjects have practical applications? Like Turing Machine!" },
    ];

    const fetchComments = async () => {
        setComments(dummyComments.map(comment => ({ ...comment })));
    }

    const fetchArticle = async (isUserPremium) => {
        if (isUserPremium) {
            setData({
                title: "Analiza Matematyczna 4 i Metody Numeryczne 3 juz od nowego roku na MiNI",
                author: "Juan de Barbas",
                content: `
            
                Wydzial Matematyki i Nauk Informacyjnych Politechniki Warszawskiej oglosil, ze od nowego roku akademickiego wprowadza dwa nowe przedmioty obowiazkowe: *Analiza Matematyczna 4* oraz *Metody Numeryczne 3*. Decyzja ta jest odpowiedzia na wieloletnie postulaty studentow o "wieksze wyzwania akademickie" oraz "prawdziwe odczucie studiow inzynierskich".
                \n
                

                Po latach spekulacji, kiedy tylko zartowano o mozliwosci istnienia czwartego semestru analizy matematycznej, stalo sie to rzeczywistoscia. Nowy przedmiot obejmie:
                - Dowod, ze istnieje jeszcze jeden, trudniejszy dowod twierdzenia Stokesa,
                - Zastosowanie analizy zespolonej do gotowania makaronu,
                - Wplyw rachunku wariacyjnego na poziom stresu studentow,
                - Niezaleznosc hipotezy continuum od organizmu smiertelnego.

                Jak informuje jeden z wykladowcow: "Po trzecim semestrze analizy wielu studentow ma niedosyt. Czulismy, ze musimy im dac cos wiecej. Dlatego AM4 bedzie miala obowiazkowe projekty badawcze, a studenci na zaliczenie beda musieli napisac podrecznik do Analizy 5."`,

            });
        }
        else {
            setData({
                title: "Analiza Matematyczna 4 i Metody Numeryczne 3",
                author: "Juan de Barbas",
                content: "",
            });
        }
    }

    const fetchData = async () => {
        setLoading(true);
        await sleep(1000);
        if (getRandomInt(10) % 2 === 0) {
            // TODO: integrate secure checking with backend 
            fetchArticle(true);
        }
        else {
            fetchArticle(false);
        }
        fetchComments();
        setLoading(false);
    }

    useEffect(() => {
        fetchData();
    }, [articleId]);


    return (
        <>
            {loading ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%', width: '100%' }} >
                    <CircularProgress size={60} />
                </Box >
            ) :
                <div className="contentWrapper">
                    <img src="src/assets/mini.jpg" className="articleCover" alt="Article cover" />
                    <button className="galleryButton" onClick={() => navigate('/photos?articleId=')}>
                        <img src="src/assets/gallery.png" alt="Gallery Icon" className="icon" /> Show gallery
                    </button>
                    <h1>{data.title}</h1>
                    <h2>Author: {data.author}</h2>
                    {data.content.length > 0 ? (
                        <p>{data.content}</p>
                    ) : (
                        <div className="paywall">
                                <p>This article is available for premium users only. Subscribe to unlock full access!</p>
                                <div className="ap-buttonContainer">
                                    <button className="subscribeButton" onClick={() => navigate('/subscribe')}>
                                        Subscribe Now
                                    </button>
                                    <button className="subscribeButton" onClick={() => navigate(`/purchase?id=${articleId}`)}>
                                        Purchase only this article
                                    </button>
                                </div>
                        </div>
                    )}
                    <div className="commentsSection">
                        <h3>Comments:</h3>
                        {comments.length > 0 ? (
                            comments.map((comment, index) => (
                                <Comment key={index} author={comment.author} date={comment.date} content={comment.content} />
                            ))
                        ) : (
                            <p>No comments yet. Be the first to comment!</p>
                        )}
                    </div>
                </div>
            }
        </>
    );
}

export default ArticlePage;