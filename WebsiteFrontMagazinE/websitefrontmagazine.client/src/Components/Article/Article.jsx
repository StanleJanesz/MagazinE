import './Article.css';
import { useNavigate } from 'react-router-dom';
import { useState, useEffect } from 'react';
import CircularProgress from '@mui/material/CircularProgress';


function Article({ data }) {
  
    const navigate = useNavigate();

    return (
        <div className = "articleContainer" onClick = { () => navigate(`/article?id=${data.id}`)}>
            <img
                src={"src/assets/mini.jpg"}
                alt={"TODO"}
                className="articleImage"
                onError={(e) => {
                    e.target.onerror = null; 
                    e.target.src = "/vite.svg"; // TODO: change to some default
                }}
            />
            <h2 className="articleTitle">{data.title}</h2>
            <h3 className="articleDescription"> 
                {data.isPremium ? "Premium" : ''} {/* TODO: TAGS */}
            </h3>  
        </div>
    );
}

export default Article;
