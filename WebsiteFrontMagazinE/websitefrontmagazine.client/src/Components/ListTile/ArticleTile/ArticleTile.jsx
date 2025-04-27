import { useState } from "react";
import './ArticleTile.css';

function ArticleTile({ title, photo, onSelect, id, accept, reject, isChosen }) {
    const [photoPath, setPhotoPath] = useState(photo || 'src/assets/mini.jpg');

    const handleTileClick = (id) => {
        onSelect(id); 
    };

    return (
        <div className={isChosen ? 'at-selected' : 'at-container'} onClick={() => handleTileClick(id)}>
            <img
                src={photoPath}
                alt={title}
                className="at-photo"
            />
            <h3 className="at-title">{title}</h3>
            <div className="at-buttonContainer">
                <button className="actionButton" onClick={accept}>
                    <img src="src/assets/accept.png" className="actionImage" />
                </button>

                <button className="actionButton" onClick={reject}>
                    <img src="src/assets/remove.png" className="actionImage" />
                </button>
            </div>
        </div>
    );
}

export default ArticleTile;
