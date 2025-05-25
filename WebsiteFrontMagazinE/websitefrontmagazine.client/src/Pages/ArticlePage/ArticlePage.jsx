import { useEffect, useState } from "react";
import CircularProgress from '@mui/material/CircularProgress';
import Box from '@mui/material/Box';
import './ArticlePage.css';
import { useNavigate, useSearchParams } from 'react-router-dom';
import Comment from '../../Components/Comment/Comment.jsx';
import { Button } from "bootstrap";
import mini from "/src/assets/mini.jpg";
import gallery from "/src/assets/gallery.png";


function ArticlePage() {
    // TODO: PARSER BALD AND ITALICS
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [comments, setComments] = useState([]);
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const articleId = searchParams.get("id");

    async function sleep(msec) {
        return new Promise(resolve => setTimeout(resolve, msec));
    }

    const dummyComments = [
        { author: "Jeanne Barbe", date: Date.now(), content: "Great news! I believe it will improve the results from students surveys!" },
        { author: "Paul deCat", date: Date.now(), content: "I'm excited about these new courses. Looking forward to AM4!" },
        { author: "Mr. Captain", date: Date.now(), content: "Will the new subjects have practical applications? Like Turing Machine!" },
    ];

    const fetchComments = async () => {
        const token = getTokenFromCookie();
        setComments([]);
        try {
            console.log(data.commentsIds);
            const fetchedComments = [];
            for (const commentId of data.commentsIds) {
                const response = await fetch(`https://localhost:7054/api/Comments/${commentId}`, {
                    method: 'GET',
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`,
                    },
                });

                // Check if the response is OK
                if (response.ok) {
                    const commentData = await response.json();
                    fetchedComments.push(commentData);     
                }
            }

            const uniqueComments = [...new Map(fetchedComments.map((c) => [c.id, c])).values()];
            setComments(uniqueComments);
            console.log(comments);
        } catch (error) {
            console.error(error);
        }
    };

    const fetchArticle = async () => {
        try {
            const token = getTokenFromCookie();
            console.log(token);
            const response = await fetch(`https://localhost:7054/articles/${articleId}`,
                {
                    method: 'GET',
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`, 
                    },
                });

            // Not found or bad request
            if (!response.ok && !response.status === 401) {
                throw new Error(`Failed to fetch article`);
            }

            if (response.status === 401) {
                throw new Error("Unathorized to retrieve article data!");
                return;
            }

            const data = await response.json();
            setData(data);
        }
        catch (error) {
            console.log(error);
        }

    }

    const getTokenFromCookie = () => {
        const cookieName = "jwt="; 
        const cookies = document.cookie.split("; "); 

        for (const cookie of cookies) {
            if (cookie.startsWith(cookieName)) {
                return cookie.substring(cookieName.length);
            }
        }

        return null; // Return null if the cookie is not found
    };

    useEffect(() => {
        const fetchArticleData = async () => {
            setLoading(true);
            try {
                await fetchArticle(); // This updates `data` state
            } catch (error) {
                console.error("Error fetching article:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchArticleData();
    }, [articleId]);

    const fetchCommentsData = async () => {
        try {
            setComments([]);
            await fetchComments();
        } catch (error) {
            console.error("Error fetching comments:", error);
        }
    };

    useEffect(() => {
        if (data && data.commentsIds) {
            fetchCommentsData();
        }
    }, [data]);

    return (
        <>
            {loading ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%', width: '100%' }} >
                    <CircularProgress size={60} />
                </Box >
            ) :
                <div className="contentWrapper">
                    <img src={mini} className="articleCover" alt="Article cover" />
                    <button className="galleryButton" onClick={() => navigate('/photos?articleId=')}>
                        <img src={gallery} alt="Gallery Icon" className="icon" /> Show gallery
                    </button>
                    <h1>{data.title}</h1>
                    <h2>Author: {data.author}</h2>
                    {data !== null ? (
                        <>
                            <p>{data.content}</p>
                            <div className="commentsSection">
                                <h3>Comments:</h3>
                                {comments.length > 0 ? (
                                    comments
                                        .filter(comment => comment.parentId === null) // Top-level comments only
                                        .map((comment, index) => (
                                            <Comment
                                                commentId={comment.id} // Prefer unique IDs if available
                                                author={comment.authorId}
                                                date={comment.date}
                                                answerIds={comment.childrenIds}
                                                content={comment.content}
                                                likesCount={comment.likesCount}
                                                dislikesCount={comment.dislikesCount}
                                            />
                                        ))
                                ) : (
                                    <p>No comments yet. Be the first to comment!</p>
                                )}
                            </div>
                        </>
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
                </div>
            }
        </>
    );
}

export default ArticlePage;