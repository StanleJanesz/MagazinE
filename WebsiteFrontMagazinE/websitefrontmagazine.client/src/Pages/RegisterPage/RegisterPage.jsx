import Button from "react-bootstrap/Button";
import Form from 'react-bootstrap/Form';
import './RegisterPage.css';

function RegisterPage() {
    return (
        <div className="container">
            <h1 className="titlePage">Register</h1>
            <input name="name" placeholder="Name" className="inputElement" />
            <input name="surname" placeholder="Surname" className="inputElement" />
            <input name="login" placeholder="Login" className="inputElement" />
            <input name="password" type="password" placeholder="Password" className="inputElement" />
            <input name="email" placeholder="Email" className="inputElement" />
            <Form>
                <Form.Check // prettier-ignore
                    type="switch"
                    id="custom-switch"
                    label="I want to subscribe"
                    className="checkBox"
                />
            </Form>
            
            <div className="buttonContainer">
                <Button className="forgotButton">Forgot my password</Button>
                <Button className="button">Login</Button>
            </div>
        </div>
    );
};

export default RegisterPage;