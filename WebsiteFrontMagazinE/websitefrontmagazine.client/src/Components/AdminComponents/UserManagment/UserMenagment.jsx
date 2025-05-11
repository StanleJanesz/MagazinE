import React, { useState } from 'react';
import {
    Button,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    TextField,
    Box,
    Typography
} from '@mui/material';

const UserManagement = () => {
    const [searchQuery, setSearchQuery] = useState('');
    const [openDialog, setOpenDialog] = useState(false);
    const [selectedUser, setSelectedUser] = useState(null);

    const handleSearch = () => {
        if (searchQuery.trim()) {
            // In a real app, you would search for the user here
            setSelectedUser(searchQuery);
            setOpenDialog(true);
        }
    };

    const handleKeyPress = (e) => {
        if (e.key === 'Enter') {
            handleSearch();
        }
    };

    const handleCloseDialog = () => {
        setOpenDialog(false);
    };

    const handleBanAction = (action) => {
        // Handle ban/unban logic here
        console.log(`${action} user:`, selectedUser);
        handleCloseDialog();
    };

    return (
        <Box sx={{ p: 3, maxWidth: 500, margin: '0 auto' }}>
            <Typography variant="h5" gutterBottom>
                User Management
            </Typography>

            <Box sx={{ display: 'flex', gap: 2, mb: 3 }}>
                <TextField
                    fullWidth
                    variant="outlined"
                    placeholder="Search user..."
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    onKeyPress={handleKeyPress}
                />
                <Button
                    variant="contained"
                    onClick={handleSearch}
                    disabled={!searchQuery.trim()}
                >
                    Enter
                </Button>
            </Box>

            <Dialog open={openDialog} onClose={handleCloseDialog}>
                <DialogTitle>User Actions</DialogTitle>
                <DialogContent>
                    <Typography>Selected user: <strong>{selectedUser}</strong></Typography>
                </DialogContent>
                <DialogActions sx={{ justifyContent: 'center', pb: 3 }}>
                    <Button
                        variant="contained"
                        color="error"
                        onClick={() => handleBanAction('Ban')}
                        sx={{ mr: 2 }}
                    >
                        Ban
                    </Button>
                    <Button
                        variant="contained"
                        color="success"
                        onClick={() => handleBanAction('Unban')}
                    >
                        Unban
                    </Button>
                </DialogActions>
            </Dialog>
        </Box>
    );
};

export default UserManagement;