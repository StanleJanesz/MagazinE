import { Dropdown, ButtonGroup } from 'react-bootstrap';

function MyDropdown({ actions, title = "Select an Option" }) {
    return (
        <Dropdown as={ButtonGroup}>
            <Dropdown.Toggle variant="primary" id="dropdown-basic">
                {title}
            </Dropdown.Toggle>

            <Dropdown.Menu>
                {actions.map((action, index) => (
                    <Dropdown.Item
                        key={index}
                        href={action.href || undefined}
                        onClick={action.onClick || undefined}
                    >
                        {action.label}
                    </Dropdown.Item>
                ))}
            </Dropdown.Menu>
        </Dropdown>
    );
}

export default MyDropdown;
