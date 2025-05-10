import Button from "react-bootstrap/Button";
import Form from 'react-bootstrap/Form';
import './RegisterPage.css';
import { useState } from "react";

function RegisterPage() {
    const [name, setName] = useState('');
    const [login, setLogin] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [surname, setSurname] = useState('');
    const [errors, setErrors] = useState({});

    const validate = () => {
        const newErrors = {};
        const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$/;

        if (!name.trim()) newErrors.name = "Name is required";
        if (!surname.trim()) newErrors.surname = "Surname is required";
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
            newErrors.password = 'Password must contain at least: one uppercase letter, one lowercase letter, one number, one special character';
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async () => {
        if (validate()) {
            const request = {
                FirstName: name,
                LastName: surname,
                Email: email,
                Password: password,
                ConfirmPassword: password
            }
            try {
                const response = await fetch('https://localhost:7054/register', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(request)
                });
                if (response.ok) {
                    const result = await response.json();
                    console.log('User registered successfully:', result);
                } else {
                    const error = await response.text();
                    console.error('Registration failed:', error);
                }
            } catch (error) {
                console.error('Error during fetch:', error);
            }
        }
    };

    return (
        <div className="container">
            <h1 className="titlePage">Register</h1>

            <div className="inputContainer">
                <input
                    className="inputElement"
                    placeholder="Name"
                    type="text"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                />
                {errors.name && <div className="errorText">{errors.name}</div>}
            </div>
            <div className="inputContainer">
                <input
                    name="surname"
                    placeholder="Surname"
                    className="inputElement"
                    type="text"
                    value={surname}
                    onChange={(e) => setSurname(e.target.value)}
                />
                {errors.surname && <div className="errorText">{errors.surname}</div>}
            </div>
            <div className="inputContainer">
                <input
                    name="login"
                    placeholder="Login"
                    className="inputElement"
                    value={login}
                    onChange={(e) => setLogin(e.target.value)}
                />
                {errors.login && <div className="errorText">{errors.login}</div>}
            </div>
            <div className="inputContainer">
                <input
                    name="password"
                    type="password"
                    placeholder="Password"
                    className="inputElement"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                />
                {errors.password && <div className="errorText">{errors.password}</div>}
            </div>
            <div className="inputContainer">
                <input
                    name="email"
                    placeholder="Email"
                    className="inputElement"
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                />
                {errors.email && <div className="errorText">{errors.email}</div>}
            </div>
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
                <Button className="button" onClick={handleSubmit}>Register</Button>
            </div>
        </div>
    );
}

export default RegisterPage;
