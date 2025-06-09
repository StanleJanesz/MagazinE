import { useState, useEffect } from 'react';
import { Editor } from 'primereact/editor';
import Button from "react-bootstrap/Button";
import Form from 'react-bootstrap/Form';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { getTokenFromCookie } from '../../utils';
import './EditArticlePage.css';

const EditArticlePage = () => {
    const [searchParams] = useSearchParams();
    const articleId = searchParams.get('id');
    const navigate = useNavigate();
    const [validationErrors, setValidationErrors] = useState({});

    const [article, setArticle] = useState({
        id: articleId ? parseInt(articleId) : 0,
        title: 'Your Tit',
        introduction: '',
        content: '',
        isPremium: false,
        isPublished: false,
        author: '',
        authorId: 0,
        photos: [],
        tagsIds: [],
        commentsIds: [],
        timeOfPublication: null
    });

    // Load article if editing existing one
    useEffect(() => {
        if (!articleId) return;

        const fetchArticle = async () => {
            try {
                const token = getTokenFromCookie();
                const response = await fetch(`https://localhost:8083/articles/${articleId}`, {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });

                if (response.ok) {
                    const data = await response.json();
                    setArticle({
                        ...data,
                        photos: data.photos || [],
                        tagsIds: data.tagsIds || [],
                        commentsIds: data.commentsIds || []
                    });
                }
            } catch (error) {
                console.error('Error fetching article:', error);
            }
        };

        fetchArticle();
    }, [articleId]);

    const validateForm = () => {
        const errors = {};
        if (!article.title.trim()) errors.Title = 'Title is required';
        if (!article.introduction.trim()) errors.Introduction = 'Introduction is required';
        if (!article.content.trim()) errors.Content = 'Content is required';
        setValidationErrors(errors);
        return Object.keys(errors).length === 0;
    };

    const handleSave = async (publish = false) => {
        if (!validateForm()) {
            return;
        }

        try {
            const token = getTokenFromCookie();
            if (!token) {
                navigate('/login');
                return;
            }

            // Prepare the article DTO with properly formatted values
            const articleDto = {
                ...article,
                title: article.title.trim(),
                introduction: article.introduction.trim(),
                content: article.content.trim(),
                isPublished: publish,
                timeOfPublication: publish ? new Date().toISOString() : null,
                // Ensure empty arrays are not sent as null
                photos: article.photos || [],
                tagsIds: article.tagsIds || [],
                commentsIds: article.commentsIds || []
            };

            const endpoint = articleId
                ? `https://localhost:8083/articles/${articleId}`
                : 'https://localhost:8083/articles';

            const method = articleId ? 'PUT' : 'POST';

            const response = await fetch(endpoint, {
                method,
                headers: {
                    'accept': '*/*',
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(articleDto)
            });

            if (!response.ok) {
                const errorData = await response.json();
                if (errorData.errors) {
                    // Convert server errors to match our validation format
                    const serverErrors = {};
                    Object.keys(errorData.errors).forEach(key => {
                        serverErrors[key] = errorData.errors[key][0];
                    });
                    setValidationErrors(serverErrors);
                }
                throw new Error(errorData.title || 'Failed to save article');
            }

           
            alert(publish ? 'Article published successfully!' : 'Article saved as draft!');
        } catch (error) {
            console.error('Error saving article:', error);
            alert(`Error: ${error.message}`);
        }
    };

    const handleDelete = async () => {
        if (!articleId || !window.confirm('Are you sure you want to delete this article?')) {
            return;
        }

        try {
            const token = getTokenFromCookie();
            const response = await fetch(`https://localhost:8083/articles/${articleId}`, {
                method: 'DELETE',
                headers: {
                    'accept': '*/*',
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.ok) {
                alert('Article deleted successfully');
                navigate('/journalist-articles');
            } else {
                throw new Error('Failed to delete article');
            }
        } catch (error) {
            console.error('Error deleting article:', error);
            alert(`Error: ${error.message}`);
        }
    };

    return (
        <div className="pageContainer">
            <Form.Group className="mb-3">
                <Form.Label>Title*</Form.Label>
                <Form.Control
                    type="text"
                    value={article.title}
                    onChange={(e) => setArticle({ ...article, title: e.target.value })}
                    placeholder="Article title"
                    isInvalid={!!validationErrors.Title}
                />
                <Form.Control.Feedback type="invalid">
                    {validationErrors.Title}
                </Form.Control.Feedback>
            </Form.Group>

            <Form.Group className="mb-3">
                <Form.Label>Introduction*</Form.Label>
                <Form.Control
                    as="textarea"
                    rows={3}
                    value={article.introduction}
                    onChange={(e) => setArticle({ ...article, introduction: e.target.value })}
                    placeholder="Brief article introduction"
                    isInvalid={!!validationErrors.Introduction}
                />
                <Form.Control.Feedback type="invalid">
                    {validationErrors.Introduction}
                </Form.Control.Feedback>
            </Form.Group>

            <Form.Group className="mb-3">
                <Form.Label>Content*</Form.Label>
                <Editor
                    value={article.content}
                    onTextChange={(e) => setArticle({ ...article, content: e.htmlValue })}
                    className="editor"
                    style={{ height: '400px' }}
                />
                {validationErrors.Content && (
                    <div className="invalid-feedback d-block">
                        {validationErrors.Content}
                    </div>
                )}
            </Form.Group>

            <div className="optionsContainer">
                <Form.Check
                    type="switch"
                    id="premium-switch"
                    label="Premium content"
                    checked={article.isPremium}
                    onChange={(e) => setArticle({ ...article, isPremium: e.target.checked })}
                    className="checkBox"
                />

                <div className="buttonGroup">
                    <Button
                        variant="primary"
                        className="saveButton"
                        onClick={() => handleSave(false)}
                    >
                        Save Draft
                    </Button>
                    <Button
                        variant="success"
                        className="sendButton"
                        onClick={() => handleSave(true)}
                    >
                        {articleId ? 'Publish Updates' : 'Publish Article'}
                    </Button>
                    {articleId && (
                        <Button
                            variant="danger"
                            className="deleteButton"
                            onClick={handleDelete}
                        >
                            Delete
                        </Button>
                    )}
                </div>
            </div>
        </div>
    );
};

export default EditArticlePage;