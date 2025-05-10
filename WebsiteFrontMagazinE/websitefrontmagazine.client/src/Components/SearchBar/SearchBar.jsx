import { React, useState } from "react";
import TextField from "@mui/material/TextField";
import './SearchBar.css';
import loupeIcon from '../../assets/loupe.png';


function SearchBar({ handleSearchButtonClick, handleKeyDown }) {
    const [searchTerm, setSearchTerm] = useState("");

    const handleInputChange = (e) => {
        setSearchTerm(e.target.value);
    };

    const handleIconClick = () => {

        handleSearchButtonClick(searchTerm);
    };

    const handleKey = (e) => {
        handleKeyDown(searchTerm, e);
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
                onKeyPress={handleKey}
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
