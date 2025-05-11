import { useEffect, useState } from "react";
import CircularProgress from '@mui/material/CircularProgress';
import Box from '@mui/material/Box';
import './ReportsView.css';
import { useNavigate } from 'react-router-dom';
import gallery from "/src/assets/gallery.png";


function ReportsView({ articleId }) {
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
                <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%', width: '100%' }} >
                    <CircularProgress size={60} />
                </Box >
            ) :
                <div className="contentWrapper">
                   
                    <button className="galleryButton" onClick={() => navigate('/')}>
                        <img src={gallery} alt="Gallery Icon" className="icon" /> View Reports
                    </button>

                    
                </div>
            }
        </>
    );
}

export default ReportsView;