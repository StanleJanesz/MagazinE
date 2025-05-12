import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import Button from "react-bootstrap/Button";
import './LoginPage.css';
import GoogleButton from 'react-google-button';
import Form from 'react-bootstrap/Form';


function LoginPage() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [errors, setErrors] = useState({});
    const [loginSuccess, setLoginSuccess] = useState(false);
    const [welcomeMessage, setWelcomeMessage] = useState('');
    const [showPassword, setShowPassword] = useState(false);

    const navigate = useNavigate();
    useEffect(() => {
        if (!loginSuccess) return;

        const fullMessage = " Welcome! Redirecting to home page";
        let index = 0;

        const interval = setInterval(() => {
            setWelcomeMessage((prev) => prev + fullMessage[index]);
            index++;
            if (index >= fullMessage.length - 1) {
                clearInterval(interval);

                // Redirect 1 second after message finishes
                setTimeout(() => {
                    navigate("/");
                }, 2000);
            }
        }, 50);

        return () => clearInterval(interval);
    }, [loginSuccess, navigate]);

    const saveTokenToCookie = (token) => {
        const expirationDays = 7;
        const expires = new Date(Date.now() + expirationDays * 24 * 60 * 60 * 1000).toUTCString();
        document.cookie = `jwt=${token}; path=/; expires=${expires}; secure`;
    };

    const Login = async () => {
        if (validate()) {
            try {
                const loginRequest = {
                    Email: email,
                    Password: password,
                    TwoFactorCode: '',
                    TwoFactorRecoveryCode: ''
                };

                const loginResponse = await fetch('https://localhost:7054/login/login', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(loginRequest)
                });

                if (loginResponse.ok) {
                    const loginResult = await loginResponse.json();
                    saveTokenToCookie(loginResult.token);
                    setLoginSuccess(true);
                    console.log('Logged in and token saved to cookie');
                } else {
                    console.error('Login failed');
                }

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
            {loginSuccess ? (
                <div className="welcomeMessage" style={{ marginTop: "1rem", fontSize: "2rem" }}>
                    {welcomeMessage}
                </div>
            ) : (
                <>
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
                            type={showPassword ? "text" : "password"}
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
                    <Form.Check
                        type="checkbox"
                        label="Show password"
                        checked={showPassword}
                        onChange={() => setShowPassword(!showPassword)}
                        className="checkBox"
                    />
                    <div className="buttonContainer">
                        <Button className="forgotButton">Forgot my password</Button>
                        <Button className="button" onClick={async () => await Login()}>
                            Sign in
                        </Button>
                    </div>
                    <GoogleButton
                        onClick={async () => await GoogleLogin()}
                        style={{ width: '100%' }}
                    />
                </>
            )}
        </div>
    );
}

export default LoginPage;