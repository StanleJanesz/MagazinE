import './Article.css';
import { useNavigate } from 'react-router-dom';
import { useState, useEffect } from 'react';
import defaultImage from '../../assets/mini.jpg'; // Adjusted path

function Article({ data }) {
    const navigate = useNavigate();
    const [imageSrc, setImageSrc] = useState(defaultImage);

    useEffect(() => {
        if (!data?.photos || data.photos.length === 0) return;

        const loadPhoto = async () => {
            try {
                console.log('Loading article photo:', data.photos[0]);
                const photoUrl = `https://localhost:8083/api/Photo/${data.photos[0]}`;
                // Test if the image loads successfully
                const img = new Image();
                img.src = photoUrl;

                await new Promise((resolve, reject) => {
                    img.onload = resolve;
                    img.onerror = reject;
                });

                setImageSrc(photoUrl);
            } catch (error) {
                console.error('Error loading article photo:', error);
                // Stay with default image if loading fails
            }
        };

        loadPhoto();
    }, [data?.photos]);

    return (
        <div className="articleContainer" onClick={() => navigate(`/article?id=${data.id}`)}>
            <img
                src={imageSrc}
                alt={data.title}
                className="articleImage"
                onError={(e) => {
                    e.target.onerror = null;
                    e.target.src = defaultImage;
                }}
            />
            <h2 className="articleTitle">{data.title}</h2>
            <h3 className="articleDescription">
                {data.isPremium ? "Premium" : ''}
            </h3>
        </div>
    );
}

export default Article;