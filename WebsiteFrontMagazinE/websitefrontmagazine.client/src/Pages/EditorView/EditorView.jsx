import { useEffect, useState } from 'react';
import Table from 'react-bootstrap/Table';
import Button from "react-bootstrap/Button";
import { useNavigate } from 'react-router-dom';
import './EditorView.css';

function EditorView() {
    const [articles, setArticles] = useState([]);
    const [acceptedArticles, setAcceptedArticles] = useState([]);
    const [pendingArticles, setPendingArticles] = useState([]);
    const [rejectedArticles, setRejectedArticles] = useState([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchArticles = () => {
            setTimeout(() => {
                const fetchedArticles = [
                    { id: 1, title: 'Article 1', author: 'Author A', status: 'Pending' },
                    { id: 2, title: 'Article 2', author: 'Author B', status: 'Accepted' },
                    { id: 3, title: 'Article 3', author: 'Author C', status: 'Rejected' },
                    { id: 4, title: 'Article 4', author: 'Author D', status: 'Pending' },
                    { id: 5, title: 'Article 5', author: 'Author E', status: 'Accepted' }
                ];
                setArticles(fetchedArticles);
                setAcceptedArticles(fetchedArticles.filter(article => article.status === 'Accepted'));
                setPendingArticles(fetchedArticles.filter(article => article.status === 'Pending'));
                setRejectedArticles(fetchedArticles.filter(article => article.status === 'Rejected'));
                setLoading(false);
            }, 1000);
        };
        fetchArticles();
    }, []);

    const handleAccept = (id) => {
        const updated = articles.map(article =>
            article.id === id ? { ...article, status: 'Accepted' } : article
        );
        updateStateLists(updated);
    };

    const handleReject = (id) => {
        const updated = articles.map(article =>
            article.id === id ? { ...article, status: 'Rejected' } : article
        );
        updateStateLists(updated);
    };

    const handleEdit = (id) => {
        navigate(`/edit-article/`);
    };

    const updateStateLists = (updatedArticles) => {
        setArticles(updatedArticles);
        setAcceptedArticles(updatedArticles.filter(article => article.status === 'Accepted'));
        setPendingArticles(updatedArticles.filter(article => article.status === 'Pending'));
        setRejectedArticles(updatedArticles.filter(article => article.status === 'Rejected'));
    };

    if (loading) return <p>Loading articles...</p>;

    return (
        <div className="editor-view container py-4">
            <section className="mb-5">
                <h2 className="section-title">Pending Articles</h2>
                <ArticleTable
                    articles={pendingArticles}
                    onAccept={handleAccept}
                    onReject={handleReject}
                    onEdit={handleEdit}
                    showActions
                />
            </section>

            <section className="mb-5">
                <h2 className="section-title">Accepted Articles</h2>
                <ArticleTable
                    articles={acceptedArticles}
                    showActions={false}
                />
            </section>

            <section>
                <h2 className="section-title">Rejected Articles</h2>
                <ArticleTable
                    articles={rejectedArticles}
                    showActions={false}
                />
            </section>
        </div>
    );


}

function ArticleTable({ articles, onAccept, onReject, onEdit, showActions }) {
    return (
        <div className="article-table p-3 rounded shadow-sm bg-white">
            <Table hover responsive className="mb-0">
                <thead>
                    <tr className="table-header">                        
                        <th>Title</th>
                        <th>Author</th>
                        {showActions && <th>Actions</th>}
                    </tr>
                </thead>
                <tbody>
                    {articles.map((article) => (
                        <tr key={article.id}>                            
                            <td>{article.title}</td>
                            <td>{article.author}</td>
                            {showActions && (
                                <td>
                                    <Button
                                        variant="outline-success"
                                        size="sm"
                                        className="me-2"
                                        onClick={() => onAccept(article.id)}
                                    >
                                        Accept
                                    </Button>
                                    <Button
                                        variant="outline-danger"
                                        size="sm"
                                        className="me-2"
                                        onClick={() => onReject(article.id)}
                                    >
                                        Reject
                                    </Button>
                                    <Button
                                        variant="outline-primary"
                                        size="sm"
                                        onClick={() => onEdit(article.id)}
                                    >
                                        Edit
                                    </Button>
                                </td>
                            )}
                        </tr>
                    ))}
                </tbody>
            </Table>
        </div>
    );
}



export default EditorView;
