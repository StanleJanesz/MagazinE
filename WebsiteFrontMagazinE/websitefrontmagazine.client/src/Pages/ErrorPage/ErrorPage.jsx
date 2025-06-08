import React from 'react';
import error from '/src/assets/not_found.jpg';  

const ErrorPage = ({ statusCode = 404, message = "Page not found" }) => {
    return (
        <div style={styles.container}>
            <img src={error} alt={`Error ${statusCode}`} style={styles.image} />
            <h2 style={styles.message}>{message}</h2>
            <p>The page you are looking for does not exist or an error occurred.</p>
            <a href="/" style={styles.homeLink}>Go back to Home</a>
        </div>
    );
};

const styles = {
    container: {
        textAlign: 'center',
        marginTop: '10vh',
        fontFamily: 'Arial, sans-serif',
        padding: '20px',
    },
    image: {
        width: '500px',    
        height: '500px',
        marginBottom: '20px',
    },
    message: {
        fontSize: '2rem',
        margin: '10px 0 20px',
    },
    homeLink: {
        fontSize: '1.2rem',
        color: '#007bff',
        textDecoration: 'none',
    }
};

export default ErrorPage;
