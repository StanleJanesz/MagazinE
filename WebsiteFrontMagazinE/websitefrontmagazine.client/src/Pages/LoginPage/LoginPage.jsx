import Button from "react-bootstrap/Button";
import './LoginPage.css';
import GoogleButton from 'react-google-button'

function LoginPage() {
    return (
        <div className="container">
            <h1 className="titlePage">Login</h1>
            <input name="login" placeholder="Login" className="inputElement" />
            <input name="password" type="password" placeholder="Password" className="inputElement" />
            <div className="buttonContainer">
                <Button className="forgotButton">Forgot my password</Button>
                <Button className="button">Sign in</Button>
            </div>
            <GoogleButton onClick={() => alert("Google login!")} style={{ width: '100%'}}/>
            
        </div>
    );
};

export default LoginPage;