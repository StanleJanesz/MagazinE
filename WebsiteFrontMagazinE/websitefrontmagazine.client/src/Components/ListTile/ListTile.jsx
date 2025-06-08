import { useState } from "react";
import './ListTile.css';

/**
 * ListTile Component
 *
 * A reusable tile UI component designed to display a brief preview of an article or item,
 * including a thumbnail image, title, and action buttons.
 *
 * Props:
 * @param {string} title - The title to display on the tile.
 * @param {string} [photo] - Optional path to a thumbnail image. Defaults to a placeholder.
 * @param {function} onSelect - Callback when tile is clicked.
 * @param {number|string} id - Identifier passed to the `onSelect` function.
 * @param {function} accept - Callback triggered by clicking the accept button.
 * @param {function} reject - Callback triggered by clicking the reject button.
 * @param {boolean} isChosen - Indicates if this tile is currently selected.
 *
 * @returns {JSX.Element} A clickable tile component with action buttons.
 */
function ListTile({ title, photo, onSelect, id, accept, reject, isChosen }) {
    const [photoPath, setPhotoPath] = useState(photo || 'src/assets/mini.jpg');

    const handleTileClick = (id) => {
        onSelect(id); 
    };

    return (
        <div className={`at-container ${isChosen ? 'selected' : ''}`} onClick={() => handleTileClick(id)}>
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

export default ListTile;
