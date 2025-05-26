import React, { useState, useEffect } from 'react';
import './Comment.css';
import like from "/src/assets/like.webp";
import dislike from "/src/assets/dislike.png";
import answer from "/src/assets/answer.png";
import report from "/src/assets/report.png";
import { getTokenFromCookie } from '../../utils';

function Comment({ author, date, content, answerIds, commentId, likesCount, dislikesCount, articleId }) {
    const [likes, setLikes] = useState(likesCount);
    const [dislikes, setDislikes] = useState(dislikesCount);
    const [isAnswered, setIsAnswered] = useState(false);
    const [isReported, setIsReported] = useState(false);
    const [isLiked, setIsLiked] = useState(false);
    const [isDisliked, setIsDisliked] = useState(false);
    const [answers, setAnswers] = useState([]);
    const [showAnswers, setShowAnswers] = useState(false);
    const [loadingAnswers, setLoadingAnswers] = useState(false);
    const [isAnswering, setIsAnswering] = useState(false);
    const [replyContent, setReplyContent] = useState('');
    const [isReporting, setIsReporting] = useState(false);
    const [reportContent, setReportContent] = useState('');

    useEffect(() => {
        setLikes(likesCount);
    }, [likesCount]);

    useEffect(() => {
        setDislikes(dislikesCount);
    }, [dislikesCount]);

    useEffect(() => {
        checkLikeStatus();
        checkDislikeStatus();
    }, []);

    const checkLikeStatus = async () => {
        const token = getTokenFromCookie();
        try {
            const response = await fetch(
                `https://localhost:7054/api/Comments/${commentId}/isLiked`,
                {
                    method: "GET",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`,
                    },
                }
            );

            if (response.ok) {
                const isLikedResponse = await response.json();
                setIsLiked(isLikedResponse);
            }
        } catch (error) {
            console.error("Error checking like status:", error);
        }
    };

    const checkDislikeStatus = async () => {
        const token = getTokenFromCookie();
        try {
            const response = await fetch(
                `https://localhost:7054/api/Comments/${commentId}/isDisliked`,
                {
                    method: "GET",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`,
                    },
                }
            );

            if (response.ok) {
                const isDislikedResponse = await response.json();
                setIsDisliked(isDislikedResponse);
            }
        } catch (error) {
            console.error("Error checking dislike status:", error);
        }
    };


    const handleLike = async () => {
        const token = getTokenFromCookie();
        try {
            const response = await fetch(`https://localhost:7054/api/Comments/${commentId}/likes`, {
                method:  isLiked ? 'DELETE' : 'POST' ,
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
            });
            if (!response.ok) {
                if (response.status === 401) {
                    throw new Error('Unauthorized request');
                } else {
                    throw new Error('Failed to like');
                }
            }

            const likesCount = await response.json();
            setLikes(likesCount);
            setIsLiked(!isLiked);
            //if (isLiked) setIsDisliked(false);
        }
        catch (error) {
            console.log(error);
        }
    }

    const handleDislike = async () => {
        const token = getTokenFromCookie();
        try {
            const response = await fetch(
                `https://localhost:7054/api/Comments/${commentId}/dislikes`,
                {
                    method: isDisliked ? "DELETE" : "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`,
                    },
                }
            );

            if (response.ok) {
                const dislikesCount = await response.json();
                setDislikes(dislikesCount);
                setIsDisliked(!isDisliked);
                //if (isDisliked) setIsLiked(false); // Ensure opposite action resets
            }
        } catch (error) {
            console.error("Error toggling dislike:", error);
        }
    };


    const submitAnswer = async () => {
        try {
            const token = getTokenFromCookie();
            const response = await fetch(`https://localhost:7054/api/Comments`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
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

            if (!response.ok) {
                const errorMsg = await response.text();
                throw new Error(errorMsg || "Failed to post comment");
            }

            const newComment = await response.json();
            const updatedAnswers = [...answers, newComment];
            const uniqueAnswers = [...new Map(updatedAnswers.map((c) => [c.id, c])).values()];
            setAnswers(uniqueAnswers);
            setReplyContent('');
            setIsAnswering(false);
            setIsAnswered(true);
        } catch (error) {
            console.error("Error posting reply:", error);
            alert(error.message || "Something went wrong while replying.");
        }
    };

    const handleReport = () => setIsReported(true);
    const rejectAnswer = () => setIsAnswering(false); 

    const fetchAnswers = async () => {
        setLoadingAnswers(true);
        try {
            const token = getTokenFromCookie();
            const fetchedAnswers = await Promise.all(
                answerIds.map(async (answerId) => {
                    const response = await fetch(`https://localhost:7054/api/Comments/${answerId}`, {
                        method: 'GET',
                        headers: {
                            "Content-Type": "application/json",
                            Authorization: `Bearer ${token}`,
                        },
                    });

                    if (!response.ok) {
                        throw new Error('Failed to fetch answer');
                    }
                    return response.json();
                })
            );
            setAnswers(fetchedAnswers);
        } catch (error) {
            console.error('Error fetching answers:', error);
        } finally {
            setLoadingAnswers(false);
        }
    };

    const toggleAnswers = () => {
        if (!showAnswers) {
            fetchAnswers();
        }
        setShowAnswers((prev) => !prev);
    };

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

    return (
        <div className="commentWrapper">
            <div className="commentHeader">
                <h4 className="commentAuthor">{author}</h4>
                <p className="commentDate">{new Date(date).toLocaleDateString()}</p>
            </div>
            <p className="commentContent">{content}</p>

            <div className="commentActions">
                <button
                    className="actionButton"
                    onClick={handleLike}
                   //disabled={isLiked || isDisliked}
                >
                    <img src={like} className="actionImage" /> {likes}
                </button>
                <button
                    className="actionButton"
                    onClick={handleDislike}
                    //disabled={isLiked || isDisliked}
                >
                    <img src={dislike} className="actionImage" /> {dislikes}
                </button>
                {isReporting ? (
                    <div className="answerInputSection">
                        <textarea
                            className="answerTextarea"
                            value={reportContent}
                            onChange={(e) => setReportContent(e.target.value)}
                            placeholder="Write report justification..."
                        />
                        <button
                            className="answerButton submit"
                            onClick={() => {
                                setIsReporting(false);
                                setIsReported(true);
                            }}
                        >
                            Submit
                        </button>
                        <button
                            className="answerButton reject"
                            onClick={() => {
                                setIsReporting(false);
                                setIsReported(false);
                            }}>
                            Reject
                        </button>
                    </div>
                ) : (
                    <button
                            className="actionButton"
                            onClick={() => setIsReporting(true)}
                            disabled={isReported}
                    >
                            <img
                                src={report}
                                className="actionImage" />
                        {isReported ? "Reported" : "Report"}
                    </button>
                )}
                <button className="actionButton" onClick={toggleAnswers}>
                    {showAnswers ? "Hide Answers" : "Show Answers"}
                </button>
                {isAnswering ? (
                    <div className="answerInputSection">
                        <textarea
                            className="answerTextarea"
                            value={replyContent}
                            onChange={(e) => setReplyContent(e.target.value)}
                            placeholder="Write your answer..."
                        />
                        <button className="answerButton submit" onClick={submitAnswer}>
                            Submit
                        </button>
                        <button className="answerButton reject" onClick={rejectAnswer}>
                            Reject
                        </button>
                    </div>
                ) : (
                    <button
                        className="actionButton"
                        onClick={() => setIsAnswering(true)}
                    >
                        <img src={answer} className="actionImage" />
                        {isAnswered ? "Answered" : "Answer"}
                    </button>
                )}
            </div>

            {showAnswers && (
                <div className="answersSection">
                    {loadingAnswers ? (
                        <p>Loading answers...</p>
                    ) : answers.length > 0 ? (
                        answers.map((answer, index) => (
                            <Comment
                                author={answer.authorEmail}
                                commentId={answer.id}
                                date={answer.date}
                                content={answer.content}
                                answerIds={answer.childrenIds}
                                likesCount={answer.likesCount}
                                dislikesCount={answer.dislikesCount}
                                articleId={articleId}
                            />
                        ))
                    ) : (
                        <p>No answers available.</p>
                    )}
                </div>
            )}
        </div>
    );
}

export default Comment;