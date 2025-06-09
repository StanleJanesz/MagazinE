import { useEffect, useState } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import CircularProgress from '@mui/material/CircularProgress';
import Box from '@mui/material/Box';
import './PhotoGalleryPage.css';
import { getTokenFromCookie } from '../../utils';

function PhotoGalleryPage() {
    const [searchParams] = useSearchParams();
    const articleId = searchParams.get('articleId');
    const [photos, setPhotos] = useState([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchPhotos = async () => {
            try {
                const token = getTokenFromCookie();
                const response = await fetch(`https://localhost:8083/articles/${articleId}`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });

                if (!response.ok) throw new Error('Failed to fetch article');

                const data = await response.json();

                if (data.photos && data.photos.length > 0) {
                    const urls = data.photos.map(name => `https://localhost:8083/api/Photo/${name}`);
                    setPhotos(urls);
                }
            } catch (error) {
                console.error("Failed to load gallery:", error);
            } finally {
                setLoading(false);
            }
        };

        if (articleId) {
            fetchPhotos();
        }
    }, [articleId]);

    if (loading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                <CircularProgress size={60} />
            </Box>
        );
    }

    return (
        <div className="galleryContainer">
            <h2>Photo Gallery</h2>
            <button className="backButton" onClick={() => navigate(-1)}>← Back to Article</button>
            <div className="photoGrid">
                {photos.length > 0 ? photos.map((url, index) => (
                    <img key={index} src={url} alt={`Photo ${index + 1}`} className="galleryPhoto" />
                )) : <p>No photos available for this article.</p>}
            </div>
        </div>
    );
}

export default PhotoGalleryPage;
