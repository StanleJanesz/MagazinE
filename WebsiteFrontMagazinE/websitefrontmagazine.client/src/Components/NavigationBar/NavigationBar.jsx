import { useState, useEffect } from 'react';
import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';

function NavigationBar() {
    const [isAuthenticated, setIsAuthenticated] = useState(false);

    useEffect(() => {
        const hasJWT = document.cookie.split(';').some(cookie => cookie.trim().startsWith('jwt='));
        setIsAuthenticated(hasJWT);
    }, []);

    const handleLogout = () => {
        document.cookie = "jwt=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT";
        setIsAuthenticated(false);
        window.location.reload(); 
    };

    return (
        <Navbar data-bs-theme="dark" fixed="top" expand={true} style={{ backgroundColor: '#313715' }}>
            <Container>
                <Navbar.Brand href="/">MagazinE</Navbar.Brand>
                <Nav className="ms-auto">
                    {isAuthenticated ? (
                        <Nav.Link onClick={handleLogout}>Logout</Nav.Link>
                    ) : (
                        <>
                            <Nav.Link href="login">Login</Nav.Link>
                            <Nav.Link href="register">Register</Nav.Link>
                        </>
                    )}
                </Nav>
            </Container>
        </Navbar>
    );
}

export default NavigationBar;
