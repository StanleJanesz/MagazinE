import React, { useState } from 'react';
import likeIcon from '/src/assets/like.webp';
import dislikeIcon from '/src/assets/dislike.png';
import answerIcon from '/src/assets/answer.png';
import reportIcon from '/src/assets/report.png';
import deleteIcon from '/src/assets/delete.png';
import { getTokenFromCookie } from '../../utils';
import { useEffect } from 'react';

/**
 * CommentActions Component
 *
 * Renders interactive buttons for a comment, such as like, dislike, report, answer, and show/hide replies.
 *
 * @component
 *
 * @param {Object} props - Component properties.
 * @param {number} props.likes - Number of likes on the comment.
 * @param {number} props.dislikes - Number of dislikes on the comment.
 * @param {boolean} props.isLiked - Whether the current user has liked the comment.
 * @param {boolean} props.isDisliked - Whether the current user has disliked the comment.
 * @param {boolean} props.isReported - Whether the comment has been reported by the user.
 * @param {Function} props.onLike - Handler called when the like button is clicked.
 * @param {Function} props.onDislike - Handler called when the dislike button is clicked.
 * @param {Function} props.onAnswerToggle - Handler called to toggle the answer input visibility.
 * @param {Function} props.onReportToggle - Handler called when the report button is clicked.
 * @param {Function} props.onShowReplies - Handler called to toggle showing/hiding replies.
 * @param {boolean} props.showReplies - Whether replies are currently being shown.
 * @param {string} props.author - Email or identifier of the comment's author.
 * @param {Function} props.onDelete - Handler called to delete the comment (shown only if user is the author).
 *
 * @returns {JSX.Element} Rendered comment actions component.
 */
function CommentActions({
    likes,
    dislikes,
    isLiked,
    isDisliked,
    isReported,
    onLike,
    onDislike,
    onAnswerToggle,
    onReportToggle,
    onShowReplies,
    showReplies,
    author,
    onDelete
}) {
    const [isAuthor, setIsAuthor] = useState(false);

    const checkAuthor = async () => {
        const token = getTokenFromCookie();
        try {
            const res = await fetch(`https://localhost:5001/PersonalInfo`, {
                headers: { Authorization: `Bearer ${token}` },
            });

            if (res.ok) {
                const data = await res.json();
                if (data.email === author) {
                    setIsAuthor(true);
                }
            }

        } catch (e) { console.error("Like status error", e); }
    }

    useEffect(() => {
        checkAuthor();
    }, [author]);

    return(
        <div className="commentActions">
            <button className="actionButton" onClick={onLike} style={{ backgroundColor: isLiked ? '#D0E8FF' : '#F5F5F5' }}>
                <img src={likeIcon} className="actionImage" /> {likes}
            </button>
            <button className="actionButton" onClick={onDislike} style={{ backgroundColor: isDisliked ? '#FFE4E1' : '#F5F5F5' }}>
                <img src={dislikeIcon} className="actionImage" /> {dislikes}
            </button>
            <button
                className="actionButton"
                onClick={onReportToggle}
                disabled={isReported}
            >
                <img src={reportIcon} className="actionImage" />
                {isReported ? "Reported" : "Report"}
            </button>
            <button className="actionButton" onClick={onShowReplies}>
                {showReplies ? "Hide Answers" : "Show Answers"}
            </button>
            <button className="actionButton" onClick={onAnswerToggle}>
                <img src={answerIcon} className="actionImage" />
                Answer
            </button>
            {/*{isAuthor && (*/}
            {/*    <button className="actionButton" onClick={onDelete}>*/}
            {/*        <img src={deleteIcon} className="actionImage" />*/}
            {/*        Delete*/}
            {/*    </button>*/}
            {/*)}*/}
        </div>
    );
}

export default CommentActions;
