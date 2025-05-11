import React, { useState } from 'react';
import './BadgedButton.css';
const BadgedButton = ({ icon, text, count = 0, onClick }) => {
    return (
        <button className="gallery-button" onClick={onClick}>
            <img src={icon} alt={`${text} Icon`} className="icon" />
            {text}
            {count > 0 && (
                <span className={`badge ${count > 9 ? 'large-number' : ''}`}>
                    {count > 99 ? '99+' : count}
                </span>
            )}
        </button>
    );
};
export default BadgedButton;