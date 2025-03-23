import Button from "react-bootstrap/Button";
import './LoginPage.css';

function LoginPage() {
    return (
        <div className="container">
            <h1 className="titlePage">Login</h1>
            <input name="login" placeholder="Login" className="inputElement" />
            <input name="password" type="password" placeholder="Password" className="inputElement" />
            <div className="buttonContainer">
                <Button className="forgotButton">Forgot my password</Button>
                <Button className="button">Login</Button>
            </div>
        </div>
    );
};

export default LoginPage;