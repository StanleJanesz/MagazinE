import { useState, useEffect } from "react";
import { useNavigate } from 'react-router-dom';
import Button from "react-bootstrap/Button";
import Form from 'react-bootstrap/Form';
import './RegisterPage.css';
import { saveTokenToCookie } from "../../utils";

/** 
 * RegisterPage component
 * Render an input form for the users
 */
function RegisterPage() {
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [login, setLogin] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [errors, setErrors] = useState({});
    const [loginSuccess, setLoginSuccess] = useState(false);
    const [welcomeMessage, setWelcomeMessage] = useState('');
    const [showPassword, setShowPassword] = useState(false);

    const navigate = useNavigate();
    const validate = () => {
        const newErrors = {};
        const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$/;

        if (!firstName.trim()) newErrors.firstName = "Name is required";
        if (!lastName.trim()) newErrors.lastName = "Surname is required";
        if (!login.trim()) newErrors.login = "Login is required";

        if (!email) {
            newErrors.email = 'Email is required';
        } else if (!/\S+@\S+\.\S+/.test(email)) {
            newErrors.email = 'Email is invalid';
        }

        if (!password) {
            newErrors.password = 'Password is required';
        } else if (password.length < 6) {
            newErrors.password = 'Password must be at least 6 characters';
        } else if (!passwordRegex.test(password)) {
            newErrors.password = 'Password must contain uppercase, lowercase, number, and special character';
        }

        if (password !== confirmPassword) {
            newErrors.confirmPassword = 'Passwords do not match';
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    useEffect(() => {
        if (!loginSuccess) return;

        const fullMessage = " Welcome to MagazinE! Redirecting to home page";
        let index = 0;

        const interval = setInterval(() => {
            setWelcomeMessage((prev) => prev + fullMessage[index]);
            index++;
            if (index >= fullMessage.length - 1) {
                clearInterval(interval);

                // Redirect 1 second after message finishes
                setTimeout(() => {
                    navigate("/");
                }, 3000);
            }
        }, 50);

        return () => clearInterval(interval);
    }, [loginSuccess, navigate]);

    const handleSubmit = async () => {
        if (!validate()) return;

        const registerRequest = {
            FirstName: firstName,
            LastName: lastName,
            Email: email,
            Password: password,
            ConfirmPassword: confirmPassword
        };

        try {
            const registerResponse = await fetch('https://localhost:8083/register', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(registerRequest)
            });

            if (registerResponse.ok) {
                console.log('User registered successfully');

                const loginRequest = {
                    Email: email,
                    Password: password,
                    TwoFactorCode: '',
                    TwoFactorRecoveryCode: ''
                };

                const loginResponse = await fetch('https://localhost:8083/login/login', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(loginRequest)
                });

                if (loginResponse.ok) {
                    const loginResult = await loginResponse.json();
                    saveTokenToCookie(loginResult.token); // assumes `token` is returned
                    setLoginSuccess(true);
                    console.log('Logged in and token saved to cookie');
                } else {
                    console.error('Login failed');
                }
            } else {
                const error = await registerResponse.text();
                console.error('Registration failed:', error);
            }
        } catch (error) {
            console.error('Network error:', error);
        }
    };

    return (
        <div className="container">
            {loginSuccess ? (
                <div className="welcomeMessage" style={{ marginTop: "1rem", fontSize: "2rem" }}>
                    {welcomeMessage}
                </div>
            ) : (
                <>
                    <h1 className="titlePage">Register</h1>

                    {[
                        { label: "Name", value: firstName, setter: setFirstName, name: "firstName", errorKey: "firstName" },
                        { label: "Surname", value: lastName, setter: setLastName, name: "lastName", errorKey: "lastName" },
                        { label: "Login", value: login, setter: setLogin, name: "login", errorKey: "login" },
                        { label: "Email", value: email, setter: setEmail, name: "email", errorKey: "email", type: "email" },
                        { label: "Password", value: password, setter: setPassword, name: "password", errorKey: "password", type: showPassword ? "text" : "password" },
                        { label: "Confirm Password", value: confirmPassword, setter: setConfirmPassword, name: "confirmPassword", errorKey: "confirmPassword", type: showPassword ? "text" : "password" }
                    ].map(({ label, value, setter, name, errorKey, type = "text" }) => (
                        <div className="inputContainer" key={name}>
                            <input
                                name={name}
                                placeholder={label}
                                className="inputElement"
                                type={type}
                                value={value}
                                onChange={(e) => setter(e.target.value)}
                            />
                            {errors[errorKey] && <div className="errorText">{errors[errorKey]}</div>}
                        </div>
                    ))}

                    <Form.Check
                        type="checkbox"
                        label="Show password"
                        checked={showPassword}
                        onChange={() => setShowPassword(!showPassword)}
                        className="checkBox"
                    />

                    <Form>
                        <Form.Check
                            type="switch"
                            id="custom-switch"
                            label="I want to subscribe"
                            className="checkBox"
                        />
                    </Form>

                    <div className="buttonContainer">
                        <Button className="forgotButton">Forgot my password</Button>
                        <Button className="button" onClick={handleSubmit}>
                            Register
                        </Button>
                    </div>
                </>
            )}
        </div>
    );
}

export default RegisterPage;
