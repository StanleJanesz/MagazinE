import React, { useState, useEffect } from 'react';
import Comment from './Comment';
import { getTokenFromCookie } from '../../utils';

/**
 * CommentReplies Component
 *
 * Fetches and displays a list of replies (child comments) based on provided answer IDs.
 * Each reply is rendered using the `Comment` component.
 *
 * @component
 *
 * @param {Object} props - Component properties.
 * @param {string[]} props.answerIds - Array of comment IDs representing the replies to fetch and display.
 * @param {string} props.articleId - ID of the article to which the comments belong.
 *
 * @returns {JSX.Element} Rendered list of comment replies.
 */

const CommentReplies = ({ answerIds, articleId }) => {
    const [answers, setAnswers] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (answerIds.length) {
            fetchReplies();
        }
    }, [answerIds]);

    const fetchReplies = async () => {
        setLoading(true);
        const token = getTokenFromCookie();
        try {
            const fetched = await Promise.all(
                answerIds.map(async (id) => {
                    const res = await fetch(`https://localhost:8083/api/Comments/${id}`, {
                        headers: { Authorization: `Bearer ${token}` },
                    });
                    return res.ok ? res.json() : null;
                })
            );
            setAnswers(fetched.filter(Boolean));
        } catch (e) {
            console.error("Error fetching replies", e);
        } finally {
            setLoading(false);
        }
    };

    if (loading) return <p>Loading answers...</p>;

    return (
        <div className="answersSection">
            {answers.length ? answers.map((ans) => (
                <Comment
                    key={ans.id}
                    author={ans.authorEmail}
                    commentId={ans.id}
                    date={ans.date}
                    content={ans.content}
                    answerIds={ans.childrenIds}
                    likesCount={ans.likesCount}
                    dislikesCount={ans.dislikesCount}
                    articleId={articleId}
                />
            )) : <p>No answers available.</p>}
        </div>
    );
};

export default CommentReplies;
