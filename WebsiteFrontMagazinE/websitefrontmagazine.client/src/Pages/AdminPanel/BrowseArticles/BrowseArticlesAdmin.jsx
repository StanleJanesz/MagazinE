import React, { useState } from 'react';
import './BrowseArticlesAdmin.css';

const Comment = ({ comment, onDelete, onBan }) => {
    return (
        <div className="comment-container">
            <div className="comment-header">
                <h4 className="comment-author">{comment.author}</h4>
                <span className="comment-date">{comment.date}</span>
            </div>
            <p className="comment-text">{comment.text}</p>
            <div className="comment-actions">
                <button
                    className="action-button ban-button"
                    onClick={() => onBan(comment.userId)}
                >
                    Ban User
                </button>
                <button
                    className="action-button delete-button"
                    onClick={() => onDelete(comment.id)}
                >
                    Delete Comment
                </button>
            </div>
        </div>
    );
};

const BrowseArticlesAdmin = () => {
    const [articles] = useState([
        { id: 1, title: 'Post about React' },
        { id: 2, title: 'JavaScript best practices' },
        { id: 3, title: 'CSS tricks 2023' },
        { id: 4, title: 'Web development trends' },
        { id: 5, title: 'Node.js performance' },
        { id: 6, title: 'Frontend frameworks comparison' },
        { id: 7, title: 'State management solutions' },
        { id: 8, title: 'React hooks guide' },
        { id: 9, title: 'TypeScript introduction' },
        { id: 10, title: 'UI/UX principles' },
        { id: 10, title: 'UI/UX principles' },
        { id: 11, title: 'UI/UX principles' },
        { id: 12, title: 'UI/UX principles' },
        { id: 13, title: 'UI/UX principles' },
        { id: 14, title: 'UI/UX principles' },
        { id: 15, title: 'UI/UX principles' },
        { id: 16, title: 'UI/UX principles' },
        { id: 17, title: 'UI/UX principles' },
    ]);

    const [comments, setComments] = useState([
        {
            id: 1,
            postId: 1,
            userId: 101,
            author: 'John Doe',
            text: 'Great post about React! Very informative.',
            date: '2023-05-15 10:30'
        },
        {
            id: 2,
            postId: 1,
            userId: 102,
            author: 'Jane Smith',
            text: 'I disagree with some points made here.',
            date: '2023-05-15 11:45'
        },
        {
            id: 3,
            postId: 2,
            userId: 103,
            author: 'Bob Johnson',
            text: 'These best practices saved me hours of work!',
            date: '2023-05-16 09:15'
        },
        {
            id: 4,
            postId: 3,
            userId: 104,
            author: 'Alice Williams',
            text: 'The CSS tricks mentioned here are outdated.',
            date: '2023-05-16 14:20'
        },
    ]);

    const [selectedArticleId, setSelectedArticleId] = useState(1);

    const filteredComments = comments.filter(comment => comment.postId === selectedArticleId);

    const handleDeleteComment = (commentId) => {
        setComments(comments.filter(comment => comment.id !== commentId));
    };

    const handleBanUser = (userId) => {
        setComments(comments.filter(comment => comment.userId !== userId));
        alert(`User ${userId} has been banned. Their comments have been removed.`);
    };

    return (
        <div className="comment-management-container">
            {/* Articles List - Left Column */}
            <div className="articles-column">
                <h2 className="articles-title">Articles</h2>
                <ul className="articles-list">
                    {articles.map(article => (
                        <li
                            key={article.id}
                            className={`article-item ${selectedArticleId === article.id ? 'selected' : ''}`}
                            onClick={() => setSelectedArticleId(article.id)}
                        >
                            <span className="article-id">{article.id}.</span>
                            <span className="article-title">{article.title}</span>
                        </li>
                    ))}
                </ul>
            </div>

            {/* Comments - Right Column */}
            <div className="comments-column">
                <h2 className="comments-title">
                    Comments for Article #{selectedArticleId}
                </h2>

                {filteredComments.length === 0 ? (
                    <p className="no-comments">No comments for this article.</p>
                ) : (
                    filteredComments.map(comment => (
                        <Comment
                            key={comment.id}
                            comment={comment}
                            onDelete={handleDeleteComment}
                            onBan={handleBanUser}
                        />
                    ))
                )}
            </div>
        </div>
    );
};

export default BrowseArticlesAdmin;