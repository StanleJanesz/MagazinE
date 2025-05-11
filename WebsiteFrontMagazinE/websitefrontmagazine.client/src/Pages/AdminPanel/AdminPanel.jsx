import { useEffect, useState } from "react";
import CircularProgress from '@mui/material/CircularProgress';
import Box from '@mui/material/Box';
import './AdminPanel.css';
import { useNavigate } from 'react-router-dom';
import Comment from '../../Components/Comment/Comment.jsx';
import { Button } from "bootstrap";
import mini from "/src/assets/mini.jpg";
import gallery from "/src/assets/gallery.png";
import UserManagement from "./../../Components/AdminComponents/UserManagment/UserMenagment.jsx"
import BadgedButton from "./../../Components/AdminComponents/BadgedButton.jsx"


function AdminPanel({ articleId }) {
    // TODO: PARSER BALD AND ITALICS
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [comments, setComments] = useState([]);
    const navigate = useNavigate();
    const getRandomInt = (max) => Math.floor(Math.random() * max);
    async function sleep(msec) {
        return new Promise(resolve => setTimeout(resolve, msec));
    }

    
    const fetchData = async () => {
        setLoading(true);
        await sleep(1000);
        if (getRandomInt(10) % 2 === 0) {
            // TODO: integrate secure checking with backend 
        }
        else {

        }
        
        setLoading(false);
    }

    useEffect(() => {
        fetchData();
    }, [articleId]);


    return (
        <>
            {loading ? (
                <div className="loading-container">
                    <CircularProgress size={60} />
                </div>
            ) : (
                <div className="content-wrapper">
                    <UserManagement />

                    <div className="button-container">
                            <BadgedButton
                                icon={gallery}
                                text="View Reports"
                                count={6777}
                                onClick={() => navigate('/admin-panel/reports')}
                            />
                        <button
                            className="gallery-button"
                            onClick={() => navigate('/photos?articleId=')}
                        >
                            <img src={gallery} alt="Unban Icon" className="icon" />
                            Unban Requests
                        </button>

                        <button
                            className="gallery-button"
                            onClick={() => navigate('/photos?articleId=')}
                        >
                            <img src={gallery} alt="Articles Icon" className="icon" />
                            Browse Articles
                        </button>

                    </div>
                </div>
            )}
        </>
    );
};
export default AdminPanel;