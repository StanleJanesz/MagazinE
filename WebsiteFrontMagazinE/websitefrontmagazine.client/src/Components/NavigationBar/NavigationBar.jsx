import { useState } from 'react';
import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';

function NavigationBar() {
    const [mail, setMail] = useState("");

    return (
        <Navbar data-bs-theme="dark" fixed="top" expand={true} style={{ backgroundColor: '#939F5C' }} >
            <Container>
                <Navbar.Brand href="/">MagazinE</Navbar.Brand>
                <Nav className="ms-auto">
                    <Nav.Link href="login">Login</Nav.Link>
                    <Nav.Link href="register">Register</Nav.Link>
                </Nav>
            </Container>
        </Navbar>
    );
}

export default NavigationBar;