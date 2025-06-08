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
            const response = await fetch(`https://localhost:8083/articles/${articleId}`, {
                method: 'GET',
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
            });

            if (!response.ok) {
                throw new Error(response.status === 401 ?
                    "Unauthorized to retrieve article data!" :
                    "Failed to fetch article");
            }

            const articleData = await response.json();
            setData(articleData);

            // Process photos if they exist
            if (articleData.photos && articleData.photos.length > 0) {
                const photoUrls = articleData.photos.map(photoName =>
                    `https://localhost:8083/api/Photo/${photoName}`
                );
                setPhotos(photoUrls);
            }
        } catch (error) {
            console.error(error);
            throw error;
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