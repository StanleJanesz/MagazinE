import { React, useState } from "react";
import TextField from "@mui/material/TextField";
import './SearchBar.css';
import loupeIcon from '/loupe.png';

function SearchBar() {

    return (
        <div className="search">
            <TextField
                id="outlined-basic"
                variant="outlined"
                label="Search"
                className="inputField"
            />
            <img
                src={loupeIcon} className="loupeIcon"/>
        </div>
    );
}
export default SearchBar;
