import React from 'react';

/**
 * CommentInputBox Component
 *
 * Renders a textarea input with submit and optional cancel buttons, typically used for writing or editing a comment.
 *
 * @component
 *
 * @param {Object} props - Component properties.
 * @param {string} props.placeholder - Placeholder text displayed in the textarea.
 * @param {string} props.value - Current value of the textarea.
 * @param {Function} props.onChange - Callback function called when the textarea content changes.
 * @param {Function} props.onSubmit - Handler called when the submit button is clicked.
 * @param {Function} props.onCancel - Handler called when the cancel button is clicked (only shown if cancelLabel is provided).
 * @param {string} props.submitLabel - Text displayed on the submit button.
 * @param {string} [props.cancelLabel] - Optional text for the cancel button. If not provided, the cancel button is not rendered.
 *
 * @returns {JSX.Element} Rendered comment input box component.
 */
const CommentInputBox = ({ placeholder, value, onChange, onSubmit, onCancel, submitLabel, cancelLabel }) => (
    <div className="answerInputSection">
        <textarea
            className="answerTextarea"
            value={value}
            onChange={(e) => onChange(e.target.value)}
            placeholder={placeholder}
        />
        <button className="answerButton submit" onClick={onSubmit}>{submitLabel}</button>
        {cancelLabel ?
            <button className="answerButton reject" onClick={onCancel}>{cancelLabel}</button> : <></>}
    </div>
);

export default CommentInputBox;
