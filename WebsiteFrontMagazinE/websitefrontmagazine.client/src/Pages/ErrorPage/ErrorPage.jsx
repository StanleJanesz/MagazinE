const ErrorPage = ({ statusCode = 404, message = "Page not found" }) => {
    return (
        <div style={styles.container}>
            <h1 style={styles.statusCode}>{statusCode}</h1>
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
    statusCode: {
        fontSize: '6rem',
        margin: 0,
        color: '#ff4d4d',
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
