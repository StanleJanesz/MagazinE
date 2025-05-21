import React, { useState } from 'react';
import './Comment.css';
import like from "/src/assets/like.webp";
import dislike from "/src/assets/dislike.png";
import answer from "/src/assets/answer.png";
import report from "/src/assets/report.png";


function Comment({ author, date, content }) {
    // TODO:
    // -handle answer, likes, report
    // -view of the answers
    const [likes, setLikes] = useState(0);
    const [dislikes, setDislikes] = useState(0);
    const [isAnswered, setIsAnswered] = useState(false);
    const [isReported, setIsReported] = useState(false);

    const handleLike = () => setLikes(likes + 1);
    const handleDislike = () => setDislikes(dislikes + 1);

    const handleAnswer = () => {
        setIsAnswered(true);
    };

    const handleReport = () => setIsReported(true);

    return (
        <div className="comment">
            <div className="commentHeader">
                <h4 className="commentAuthor">{author}</h4>
                <p className="commentDate">{new Date(date).toLocaleDateString()}</p>
            </div>
            <p className="commentContent">{content}</p>

            <div className="commentActions">
                <button className="actionButton" onClick={handleLike}>
                    <img src={like} className="actionImage"/> {likes}
                </button>
                <button className="actionButton" onClick={handleDislike}>
                    <img src={dislike} className="actionImage" /> {dislikes}
                </button>
                <button className="actionButton" onClick={handleAnswer} disabled={isAnswered}>
                    <img src={answer} className="actionImage" />{isAnswered ? 'Answered' : 'Answer'}
                </button>
                <button className="actionButton" onClick={handleReport} disabled={isReported}>
                    <img src={report} className="actionImage" />{isReported ? 'Reported' : 'Report'}
                </button>
            </div>
        </div>
    );
}

export default Comment;
