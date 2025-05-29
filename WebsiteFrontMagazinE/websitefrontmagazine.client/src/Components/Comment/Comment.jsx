// Comment.jsx
import React, { useState, useEffect } from 'react';
import './Comment.css';
import CommentActions from './CommentActions';
import CommentReplies from './CommentReplies';
import CommentInputBox from './CommentInputBox';
import { getTokenFromCookie } from '../../utils';


/**
 * Comment Component
 *
 * Responsible for rendering an individual comment, including its content, author information,
 * metadata (like date and likes/dislikes), and nested replies. Also handles comment interaction
 * features such as displaying replies and updating the comment state when necessary.
 *
 * @param {Object} props - The component props.
 * @param {string} props.author - The email of the comment's author.
 * @param {string} props.authorId - The ID of the comment's author.
 * @param {string} props.date - The ISO string representing when the comment was posted.
 * @param {string} props.content - The main text content of the comment.
 * @param {string[]} props.answerIds - Array of IDs representing replies to this comment.
 * @param {string} props.commentId - Unique identifier for the comment.
 * @param {number} props.likesCount - Number of likes the comment has received.
 * @param {number} props.dislikesCount - Number of dislikes the comment has received.
 * @param {string} props.articleId - The ID of the article to which this comment belongs.
 * @param {Array<Object>} props.comments - The full list of comments related to the article.
 * @param {Function} props.setComments - Function to update the comments state in the parent component.
 *
 * @returns {JSX.Element} A rendered comment element, possibly including nested replies.
 */
function Comment({
    author,
    authorId,
    date,
    content,
    answerIds,
    commentId,
    likesCount,
    dislikesCount,
    articleId,
    comments,
    setComments
}) {
    const [likes, setLikes] = useState(likesCount);
    const [dislikes, setDislikes] = useState(dislikesCount);
    const [isReported, setIsReported] = useState(false);
    const [isLiked, setIsLiked] = useState(false);
    const [isDisliked, setIsDisliked] = useState(false);
    const [showAnswers, setShowAnswers] = useState(false);
    const [isAnswering, setIsAnswering] = useState(false);
    const [replyContent, setReplyContent] = useState('');
    const [isReporting, setIsReporting] = useState(false);
    const [reportContent, setReportContent] = useState('');

    useEffect(() => {
        setLikes(likesCount);
        setDislikes(dislikesCount);
        checkLikeStatus();
        checkDislikeStatus();
    }, [likesCount, dislikesCount]);

    const checkLikeStatus = async () => {
        const token = getTokenFromCookie();
        try {
            const res = await fetch(`https://localhost:5001/api/Comments/${commentId}/isLiked`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (res.ok) setIsLiked(await res.json());
        } catch (e) { console.error("Like status error", e); }
    };

    const checkDislikeStatus = async () => {
        const token = getTokenFromCookie();
        try {
            const res = await fetch(`https://localhost:5001/api/Comments/${commentId}/isDisliked`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (res.ok) setIsDisliked(await res.json());
        } catch (e) { console.error("Dislike status error", e); }
    };

    const handleLike = async () => {
        const token = getTokenFromCookie();
        try {
            const res = await fetch(`https://localhost:5001/api/Comments/${commentId}/likes`, {
                method: isLiked ? 'DELETE' : 'POST',
                headers: { Authorization: `Bearer ${token}` },
            });
            if (res.ok) {
                setLikes(await res.json());
                setIsLiked(!isLiked);
            }
        } catch (e) { console.error(e); }
    };

    const handleDislike = async () => {
        const token = getTokenFromCookie();
        try {
            const res = await fetch(`https://localhost:5001/api/Comments/${commentId}/dislikes`, {
                method: isDisliked ? 'DELETE' : 'POST',
                headers: { Authorization: `Bearer ${token}` },
            });
            if (res.ok) {
                setDislikes(await res.json());
                setIsDisliked(!isDisliked);
            }
        } catch (e) { console.error(e); }
    };

    const handleReport = async () => {
        const token = getTokenFromCookie();
        try {
            const res = await fetch(`https://localhost:5001/reports`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
                body: JSON.stringify({ reason: reportContent, commentId }),
            });
            if (res.ok) {
                alert("Comment reported successfully.");
                setIsReported(true);
                setIsReporting(false);
            }
        } catch (e) {
            console.error("Report error", e);
            alert("Failed to report comment.");
        }
    };

    const handleReplySubmit = async () => {
        const token = getTokenFromCookie();
        try {
            const res = await fetch(`https://localhost:5001/api/Comments`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
                body: JSON.stringify({
                    Content: replyContent,
                    ArticleId: articleId,
                    ParentId: commentId,
                    Date: new Date().toISOString(),
                    ChildrenIds: [],
                    LikesCount: 0,
                    DislikesCount: 0,
                }),
            });
            if (res.ok) {
                const createdComment = await res.json(); 
                setComments(prev => [...prev, createdComment]);
                setIsAnswering(false);
                setReplyContent('');
                setShowAnswers(true);
            }
        } catch (e) {
            console.error("Reply error", e);
            alert("Failed to reply to comment.");
        }
    };

    const handleDelete = async () => {
        //const token = getTokenFromCookie();
        //try {
        //    const res = await fetch(`https://localhost:7054/api/Comments`, {
        //        method: 'POST',
        //        headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
        //        body: JSON.stringify({
        //            Content: replyContent,
        //            ArticleId: articleId,
        //            ParentId: commentId,
        //            Date: new Date().toISOString(),
        //            ChildrenIds: [],
        //            LikesCount: 0,
        //            DislikesCount: 0,
        //        }),
        //    });
        //    if (res.ok) {
        //        setIsAnswering(false);
        //        setReplyContent('');
        //        setShowAnswers(true);
        //    }
        //} catch (e) {
        //    console.error("Reply error", e);
        //    alert("Failed to reply to comment.");
        //}
    }

    return (
        <div className="commentWrapper">
            <div className="commentHeader">
                <h4 className="commentAuthor">{author}</h4>
                <p className="commentDate">{new Date(date).toLocaleDateString()}</p>
            </div>
            <p className="commentContent">{content}</p>

            <CommentActions
                likes={likes}
                dislikes={dislikes}
                isLiked={isLiked}
                isDisliked={isDisliked}
                isReported={isReported}
                onLike={handleLike}
                onDislike={handleDislike}
                onAnswerToggle={() => setIsAnswering(!isAnswering)}
                onReportToggle={() => setIsReporting(!isReporting)}
                onShowReplies={() => setShowAnswers(!showAnswers)}
                showReplies={showAnswers}
                author={author}
            />

            {isReporting && (
                <CommentInputBox
                    placeholder="Write report justification..."
                    value={reportContent}
                    onChange={setReportContent}
                    onSubmit={handleReport}
                    onCancel={() => setIsReporting(false)}
                    submitLabel="Submit"
                    cancelLabel="Cancel"
                />
            )}

            {isAnswering && (
                <CommentInputBox
                    placeholder="Write your answer..."
                    value={replyContent}
                    onChange={setReplyContent}
                    onSubmit={handleReplySubmit}
                    onCancel={() => setIsAnswering(false)}
                    submitLabel="Submit"
                    cancelLabel="Cancel"
                />
            )}

            {showAnswers && (
                <CommentReplies
                    answerIds={answerIds}
                    articleId={articleId}
                />
            )}
        </div>
    );
}

export default Comment;
