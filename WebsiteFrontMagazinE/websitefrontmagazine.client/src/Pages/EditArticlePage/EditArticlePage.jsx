import { useState } from 'react';
import { Editor } from 'primereact/editor';
import Button from "react-bootstrap/Button";
import Form from 'react-bootstrap/Form';
import './EditArticlePage.css';

const EditArticlePage = () => {
    const [text, setText] = useState("");


    // Na razie niepotrzebne, do modyfikacji opcji edycji tekstu
    const renderHeader = () => {
        return (
            <span className="ql-formats">
                <button className="ql-bold" aria-label="Bold"></button>
                <button className="ql-italic" aria-label="Italic"></button>
                <button className="ql-underline" aria-label="Underline"></button>
                <button className="ql-image" aria-label="Insert cover"></button>
            </span>
        );
    };

    const header = renderHeader();

    return (
        <div className="pageContainer">
            <Editor value={text} onTextChange={(e) => setText(e.htmlValue)} className="editor" />
            <div className="optionsContainer">
                <Form className="checkBoxContainer">
                    <Form.Check 
                        type="switch"
                        id="custom-switch"
                        label="Mark as subscribers only"
                        className="checkBox"
                    />
                </Form>
                <Button className="saveButton">Save</Button>
                <Button className="sendButton">Send</Button>
                <Button className="deleteButton">Delete</Button>
            </div>
        </div>
    )
};

export default EditArticlePage;
