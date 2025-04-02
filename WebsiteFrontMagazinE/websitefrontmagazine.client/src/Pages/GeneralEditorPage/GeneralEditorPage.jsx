import { useEffect, useState } from 'react';
import MyDropdown from '../../Components/MyDropdown/MyDropdown.jsx';
import SearchBar from '../../Components/SearchBar/SearchBar.jsx';
import Table from 'react-bootstrap/Table';
import './GeneralEditorPage.css';

function GeneralEditorPage() {
    const [users, setUsers] = useState([]);
    const [filteredUsers, setFilteredUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');

    // Simulate fetching data
    useEffect(() => {
        const fetchUsers = () => {
            setTimeout(() => {
                const fetchedUsers = [
                    { id: 1, name: 'Alice Johnson', email: 'alice@example.com', role: 'Journalist' },
                    { id: 2, name: 'Bob Smith', email: 'bob@example.com', role: 'Editor' },
                    { id: 3, name: 'Charlie Brown', email: 'charlie@example.com', role: 'Admin' },
                    { id: 4, name: 'Diana Prince', email: 'diana@example.com', role: 'General editor' },
                    { id: 5, name: 'Grzegorz Braun', email: 'braun@polska.ru', role: 'Reader' }
                ];
                setUsers(fetchedUsers);
                setFilteredUsers(fetchedUsers);
                setLoading(false);
            }, 1000);
        };

        fetchUsers();
    }, []);

    const roles = [
        { label: 'General editor', onClick: () => console.log('Role changed to General editor') },
        { label: 'Admin', onClick: () => console.log('Role changed to Admin') },
        { label: 'Editor', onClick: () => console.log('Role changed to Editor') },
        { label: 'Journalist', onClick: () => console.log('Role changed to Journalist') },
        { label: 'Reader', onClick: () => console.log('Role changed to reader') }
    ];

    const handleSearchButtonClick = (searchTerm) => {
        setSearchTerm(searchTerm);
        if (!searchTerm) {
            setFilteredUsers(users); 
            return;
        }

        const filtered = users.filter(user =>
            user.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
            user.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
            user.role.toLowerCase().includes(searchTerm.toLowerCase())
        );

        setFilteredUsers(filtered);
    };

    return (
        <div style={{
            display: 'flex', flexDirection: 'column', height: '90vh', padding: '20px',
            border: '1px solid #BBCE8A', borderRadius: '8px', boxShadow: '0 4px 8px rgba(0, 0, 0, 0.1)', marginTop: '40px', overflowY: 'auto'
        }}>
            <SearchBar
                handleSearchButtonClick={handleSearchButtonClick}
            />
            <br />
            {loading ? (
                <div>Loading users...</div>
            ) : (
                <Table striped bordered hover>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Email</th>
                            <th>Role</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {filteredUsers.map(user => (
                            <tr key={user.id}>
                                <td>{user.name}</td>
                                <td>{user.email}</td>
                                <td>{user.role}</td>
                                <td style={{ width: '120px' }}>
                                    <MyDropdown actions={roles} title="Change Role" />
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </Table>
            )}
        </div>
    );
}

export default GeneralEditorPage;
