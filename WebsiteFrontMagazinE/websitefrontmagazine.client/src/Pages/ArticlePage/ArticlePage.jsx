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
import { loadStripe } from '@stripe/stripe-js';

function ArticlePage() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [commentsLoading, setCommentsLoading] = useState(true);
    const [comments, setComments] = useState([]);
    const [newComment, setNewComment] = useState("");
    const [photos, setPhotos] = useState([]);
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const articleId = searchParams.get("id");
    const [author, setAuthor] = useState('');
    const [title, setTitle] = useState('');
    const stripePromise = loadStripe('pk_test_51R6V6oQTT0aReMtnxE5kA3KKoow1v9t4WmNt6CCDvSRudXXs9XjqZ4PHiPmtDeC6Gp8bD41g3D7bW9sebb2HqwRw00Vc5GAlSd'); 

    const fetchComments = async () => {
        const token = getTokenFromCookie();
        setComments([]);
        try {
            const fetchedComments = [];
            for (const commentId of data.commentsIds) {
                const response = await fetch(`https://localhost:8083/api/Comments/${commentId}`, {
                    method: 'GET',
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`,
                    },
                });

                if (response.ok) {
                    const commentData = await response.json();
                    fetchedComments.push(commentData);
                }
            }
            setComments(fetchedComments);
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

            const data = await response.json();
            setAuthor(data.Author);
            setTitle(data.Title);  

            // Handle 401 Unauthorized specifically
            if (response.status === 401) {
                const limitedData = await response.json();
                setAuthor(limitedData.Author);
                setTitle(limitedData.Title);
                setData(null); 
                return; 
            }

            // Handle other errors
            if (!response.ok) {
                throw new Error(`Failed to fetch article: ${response.status}`);
            }
            
          // Process photos if they exist
            if (data.photos && data.photos.length > 0) {
                const photoUrls = data.photos.map(photoName =>
                    `https://localhost:8083/api/Photo/${photoName}`
                );
              setPhotos(photoUrls);
            }
            setData(data);
        }
        catch (error) {
            console.log(error);
        }
    };

    useEffect(() => {
        const loadData = async () => {
            setLoading(true);
            try {
                await fetchArticle();
                if (data?.commentsIds) {
                    await fetchComments();
                }
            } catch (error) {
                console.error("Article load error:", error);
            } finally {
                setLoading(false);
                setCommentsLoading(false);
            }
        };
        loadData();
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

    async function startCheckout() {
        try {
            const token = getTokenFromCookie();
            const currentDate = new Date(Date.now());
            const endDate = new Date(currentDate);
            endDate.setMonth(endDate.getMonth() + 6);

            const requestBody = {
                subscriptionDTO: {
                    startDate: currentDate.toISOString(),
                    endDate: endDate.toISOString(),
                    state: 'Active'
                }
            };
            console.log(token);
            console.log(requestBody);
            const res = await fetch(`https://localhost:8083/subscriptions/subscribe`, {
                method: 'POST',
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestBody)
            });

            if (!res.ok) {
                const errorText = await res.text();
                console.error('Server error:', errorText);
                throw new Error(`HTTP error! status: ${res.status}, message: ${res.body}`);
            }

            const data = await res.json();
            const { sessionId } = data;
            const stripe = await stripePromise;
            await stripe?.redirectToCheckout({ sessionId });
        } catch (error) {
            console.error('Checkout error:', error);
            navigate('/subscription-cancel');
        }
    }

    const handleCommentSubmit = async () => {
        if (!newComment.trim()) return;

        const token = getTokenFromCookie();
        if (!token) {
            navigate('/login');
            return;
        }

        try {
            const response = await fetch(`https://localhost:8083/api/Comments`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`
                },
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

            if (response.ok) {
                const newCommentData = await response.json();
                setComments(prev => [newCommentData, ...prev]);
                setNewComment("");
            }
        } catch (error) {
            console.error("Comment submission error:", error);
        }
    };

    const openPhotoGallery = () => {
        if (photos.length > 0) {
            navigate(`/photos?articleId=${articleId}`);
        }
    };

    if (loading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%', width: '100%' }}>
                <CircularProgress size={60} />
            </Box>
        );
    }

    if (!data) {
        return (
            <div className="paywall">
                <p>This article is available for premium users only. Subscribe to unlock full access!</p>
                <div className="ap-buttonContainer">
                    <button className="subscribeButton" onClick={() => navigate('/subscribe')}>
                        Subscribe Now
                    </button>
                    <button className="subscribeButton" onClick={() => navigate(`/purchase?id=${articleId}`)}>
                        Purchase only this article
                    </button>
                    <h1>{title}</h1>
                    <h2>Author: {author}</h2>
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
                                    <button className="subscribeButton" onClick={async () => await startCheckout()}>
                                    Subscribe Now
                                    </button>
                                    <button className="subscribeButton" onClick={async () => await startCheckout()}>
                                    Purchase only this article
                                </button>
                            </div>
                        </div>
                    )}
                </div>
            </div>
        );
    }

    return (
        <div className="contentWrapper">
            {/* Use first photo as cover if available */}
            {photos.length > 0 ? (
                <img
                    src={photos[0]}
                    className="articleCover"
                    alt="Article cover"
                    onClick={openPhotoGallery}
                    style={{ cursor: 'pointer' }}
                />
            ) : (
                <img src={mini} className="articleCover" alt="Default article cover" />
            )}

            {/* Gallery button - only shown if there are photos */}
            {photos.length > 0 && (
                <button className="galleryButton" onClick={openPhotoGallery}>
                    <img src={gallery} alt="Gallery Icon" className="icon" />
                    Show gallery ({photos.length})
                </button>
            )}

            <h1>{data.title}</h1>
            <h2>Author: {data.author}</h2>

            {/* Simple photo preview - only added element */}
            {photos.length > 1 && (
                <div style={{ margin: '20px 0' }}>
                    <p>This article contains {photos.length} photos</p>
                </div>
            )}

            <p>{data.content}</p>

            <div className="commentsSection">
                <h3>Comments:</h3>

                <CommentInputBox
                    value={newComment}
                    onChange={setNewComment}
                    onSubmit={handleCommentSubmit}
                    placeholder="Write your comment!"
                    submitLabel="Submit"
                />

                {commentsLoading ? (
                    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                        <CircularProgress />
                    </Box>
                ) : (
                    comments.filter(c => c.parentId === null).map(comment => (
                        <Comment
                            key={comment.id}
                            commentId={comment.id}
                            author={comment.authorEmail}
                            date={comment.date}
                            content={comment.content}
                            likesCount={comment.likesCount}
                            dislikesCount={comment.dislikesCount}
                            articleId={articleId}
                        />
                    ))
                )}
            </div>
        </div>
    );
}

export default ArticlePage;