import { React, useState } from "react";
import TextField from "@mui/material/TextField";
import './SearchBar.css';
import loupeIcon from '../../assets/loupe.png';

function SearchBar({ handleSearchButtonClick }) {
    const [searchTerm, setSearchTerm] = useState(""); 

    const handleInputChange = (e) => {
        setSearchTerm(e.target.value);
    };

    const handleIconClick = () => {
        handleSearchButtonClick(searchTerm); 
    };

    return (
        <div className="search">
            <TextField
                id="outlined-basic"
                variant="outlined"
                label="Search"
                className="inputField"
                value={searchTerm}  
                onChange={handleInputChange}  
            />
            <img
                src={loupeIcon}
                className="loupeIcon"
                onClick={handleIconClick}  
                alt="Search"
            />
        </div>
    );
}

export default SearchBar;
