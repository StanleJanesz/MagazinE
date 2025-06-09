import { useEffect, useState } from "react";
import CircularProgress from '@mui/material/CircularProgress';
import Box from '@mui/material/Box';
import './ArticlePage.css';
import { useNavigate, useSearchParams } from 'react-router-dom';
import Comment from '../../Components/Comment/Comment.jsx';
import mini from "/src/assets/mini.jpg";
import gallery from "/src/assets/gallery.png";
import { getTokenFromCookie } from "../../utils";
import CommentInputBox from "../../Components/Comment/CommentInputBox";

/**
 * ArticlePage Component
 *
 * This page displays the full content of a selected article, including its metadata,
 * cover image, and associated user comments. It also provides access to related photos
 * via a gallery button and includes conditional logic for premium content access.
 *
 * TODO:
 * - Add content parsing for bold and italic styles.
 * - Improve error handling and authorization flow.
 * - dynamic displaying comments state change
 *
 * @returns {JSX.Element} Article view component.
 */

function ArticlePage() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [commentsLoading, setCommentsLoading] = useState(true);
    const [comments, setComments] = useState([]);
    const [newComment, setNewComment] = useState("");
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const articleId = searchParams.get("id");

    const fetchComments = async () => {
        const token = getTokenFromCookie();
        setComments([]);
        try {
            console.log(data.commentsIds);
            const fetchedComments = [];
            for (const commentId of data.commentsIds) {
                const response = await fetch(`https://localhost:8083/api/Comments/${commentId}`, {
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
            const response = await fetch(`https://localhost:8083/articles/${articleId}`,
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
        setCommentsLoading(true);
        try {
            setComments([]);
            await fetchComments();
        } catch (error) {
            console.error("Error fetching comments:", error);
        }
        setCommentsLoading(false);
    };

    useEffect(() => {
        if (data && data.commentsIds) {
            fetchCommentsData();
        }
    }, [data]);

    const handleCommentSubmit = async () => {
        if (!newComment.trim()) {
            alert("Comment cannot be empty");
            return;
        }
        const token = getTokenFromCookie();
        if (!token) {
            alert("You must be logged in to comment.");
            navigate('/login');
            return;
        }
        try {
            const response = await fetch(`https://localhost:8083/api/Comments`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
                body: JSON.stringify({
                    Content: newComment,
                    ArticleId: articleId,
                    ParentId: null,
                    Date: new Date().toISOString(),
                    ChildrenIds: [],
                    LikesCount: 0,
                    DislikesCount: 0,
                }),
            });
            
            console.log(response);
            if (response.ok) {
                const newCommentData = await response.json(); 
                setComments(prev => [newCommentData, ...prev]);
                setData(prevData => ({
                    ...prevData,
                    commentsIds: [newCommentData.id, ...prevData.commentsIds],
                }));
                setNewComment("");  // clear textarea on success
            }
            await fetchCommentsData();
        } catch (error) {
            console.error(error);
            alert("Failed to submit comment. Please try again.");
        }
    };


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
                                {!commentsLoading ? (
                                    comments
                                        .filter(comment => comment.parentId === null) 
                                        .map((comment, index) => (
                                            <Comment
                                                key={comment.id}
                                                commentId={comment.id} 
                                                author={comment.authorEmail}
                                                authorId={comment.authorId}
                                                date={comment.date}
                                                answerIds={comment.childrenIds}
                                                content={comment.content}
                                                likesCount={comment.likesCount}
                                                dislikesCount={comment.dislikesCount}
                                                articleId={articleId}
                                                comments={comments}
                                                setComments={setComments}
                                            />
                                        ))
                                ) : (
                                        <Box
                                            sx={{
                                                display: 'flex',
                                                justifyContent: 'center',
                                                alignItems: 'center',
                                            }}
                                        >
                                            <CircularProgress />
                                        </Box>
                                )}
                                {/* Always show input box for adding new comment */}
                                <CommentInputBox
                                    placeholder="Write your comment!"
                                    value={newComment}
                                    onChange={setNewComment}
                                    onSubmit={handleCommentSubmit}
                                    submitLabel="Submit"
                                />
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