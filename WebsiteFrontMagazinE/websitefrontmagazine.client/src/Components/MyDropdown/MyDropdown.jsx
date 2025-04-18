import { Dropdown, ButtonGroup } from 'react-bootstrap';
import { useState, useEffect, useRef } from 'react';

function MyDropdown({ actions, title = "Select an Option" }) {
    const [menuWidth, setMenuWidth] = useState(null);
    const toggleRef = useRef(null);
    const [checkedItems, setCheckedItems] = useState({});

    useEffect(() => {
        if (toggleRef.current) {
            setMenuWidth(toggleRef.current.offsetWidth);
        }
    }, [toggleRef.current]);

    const handleCheckboxChange = (label, event) => {
        event.stopPropagation(); // Prevent the dropdown from closing
        setCheckedItems(prevState => ({
            ...prevState,
            [label]: !prevState[label]
        }));
    };

    return (
        <Dropdown as={ButtonGroup}>
            <Dropdown.Toggle variant="primary" id="dropdown-basic" ref={toggleRef}>
                {title}
            </Dropdown.Toggle>

            <Dropdown.Menu style={{ width: menuWidth || 'auto' }}>
                {actions.map((action, index) => (
                    <div
                        key={index}
                        className="dropdown-item"
                        onClick={(e) => e.stopPropagation()} // Prevent closing when clicking on the item
                    >
                        <div className="form-check">
                            <input
                                className="form-check-input"
                                type="checkbox"
                                id={`check-${index}`}
                                checked={!!checkedItems[action.label]}
                                onChange={(e) => handleCheckboxChange(action.label, e)}
                            />
                            <label className="form-check-label" htmlFor={`check-${index}`}>
                                {action.label}
                            </label>
                        </div>
                    </div>
                ))}
            </Dropdown.Menu>
        </Dropdown>
    );
}

export default MyDropdown;
