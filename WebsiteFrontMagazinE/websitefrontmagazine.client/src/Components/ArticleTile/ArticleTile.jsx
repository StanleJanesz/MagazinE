import { useState } from "react";
import './ArticleTile.css';

function ArticleTile({ title, photo, onClick, id, sendToReview, rejectArticle }) {
    const [photoPath, setPhotoPath] = useState(photo || 'src/assets/mini.jpg');

    
    return (
        <div className="at-container" onClick={() => onClick(id)}>
            <img
                src={photoPath}
                alt={title}
                className="at-photo"
            />
            <h3 className="at-title">{title}</h3>
            <div className="at-buttonContainer">
                <button className="actionButton" onClick={sendToReview}>
                    <img src="src/assets/accept.png" className="actionImage" />
                </button>

                <button className="actionButton" onClick={rejectArticle}>
                    <img src="src/assets/remove.png" className="actionImage" />
                </button>
            </div>
        </div>
    );
}

export default ArticleTile;
