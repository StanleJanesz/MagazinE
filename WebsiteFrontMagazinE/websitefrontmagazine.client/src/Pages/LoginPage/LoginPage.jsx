import { useState } from 'react';
import Button from "react-bootstrap/Button";
import './LoginPage.css';
import GoogleButton from 'react-google-button'

function LoginPage() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [errors, setErrors] = useState({});

    const Login = async () => {
        if (validate()) {
            try {
                const response =
                    await fetch('https://localhost:7054/login/login', {
                        method: 'POST',
                        headers: {
                            'Accept': 'application/json',
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({
                            Email: email, Password: password
                        })
                    });
                const content = await response.json();

                console.log(content);

            }
            catch (error) {
                console.log(error);
            }
        }
    } 

    const validate = () => {
        const newErrors = {};

        if (!email.trim()) newErrors.email = "Email is required";
        if (!password.trim()) newErrors.password = "Surname is required";
        
        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const GoogleLogin = async () => {

        try {
            const response =
                await fetch('/login/google');
            const content = await response.json();

            console.log(content);

        }
        catch (error) {
            console.log(error);
        }

    }

    return (
        <div className="container">
            <h1 className="titlePage">Login</h1>
            <div className="inputContainer">
                <input
                    name="email"
                    placeholder="Email"
                    className="inputElement"
                    value={email}
                    onChange={(event) => {
                        setEmail(event.target.value);
                    }}
                />
                {errors.email && <div className="errorText">{errors.email}</div>}
            </div>
            <div className="inputContainer">
                <input
                    name="password"
                    type="password"
                    placeholder="Password"
                    className="inputElement"
                    value={password}
                    onChange={(event) => {
                        setPassword(event.target.value);
                        console.log(`${password}`);
                    }}
                />
                {errors.password && <div className="errorText">{errors.password}</div>}
            </div>
            <div className="buttonContainer">
                <Button
                    className="forgotButton"
                >
                    Forgot my password
                </Button>
                <Button
                    className="button"
                    onClick={async () => await Login() }
                >
                    Sign in
                </Button>
            </div>
            <GoogleButton onClick={async () => await GoogleLogin()} style={{ width: '100%' }}/>
            
        </div>
    );
};

export default LoginPage;