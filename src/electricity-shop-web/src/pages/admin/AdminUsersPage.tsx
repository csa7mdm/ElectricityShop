import React, { useEffect, useState } from 'react';
import {
  Container,
  Typography,
  Box,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TablePagination,
  TextField,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  MenuItem,
  Chip,
  IconButton,
  Grid,
  Alert,
  Avatar,
  FormControl,
  InputLabel,
  Select,
  SelectChangeEvent,
} from '@mui/material';
import {
  Search as SearchIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Person as PersonIcon,
  FilterList as FilterListIcon,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';

import { User, PaginationParams } from '../../types';
import { formatDate } from '../../utils/formatters';
import { profileSchema } from '../../utils/validation';
import LoadingSpinner from '../../components/common/LoadingSpinner';

// Mock data
const mockUsers: User[] = [
  {
    id: '1',
    email: 'john.doe@example.com',
    firstName: 'John',
    lastName: 'Doe',
    role: 'user',
    createdAt: new Date(2023, 0, 15).toISOString(),
    updatedAt: new Date(2023, 2, 10).toISOString(),
  },
  {
    id: '2',
    email: 'jane.smith@example.com',
    firstName: 'Jane',
    lastName: 'Smith',
    role: 'user',
    createdAt: new Date(2023, 1, 20).toISOString(),
    updatedAt: new Date(2023, 3, 5).toISOString(),
  },
  {
    id: '3',
    email: 'admin@electricityshop.com',
    firstName: 'Admin',
    lastName: 'User',
    role: 'admin',
    createdAt: new Date(2022, 11, 1).toISOString(),
    updatedAt: new Date(2023, 2, 15).toISOString(),
  },
];

const AdminUsersPage: React.FC = () => {
  const [users, setUsers] = useState<User[]>(mockUsers);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [userToDelete, setUserToDelete] = useState<User | null>(null);
  
  // State for pagination and filtering
  const [paginationParams, setPaginationParams] = useState<PaginationParams>({
    page: 1,
    limit: 10,
  });
  const [filters, setFilters] = useState({
    search: '',
    role: '',
  });
  
  // Form setup
  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm({
    resolver: yupResolver(profileSchema),
  });
  
  // Handle search
  const handleSearch = (event: React.ChangeEvent<HTMLInputElement>) => {
    const value = event.target.value;
    setFilters({
      ...filters,
      search: value,
    });
  };
  
  // Handle role filter
  const handleRoleFilter = (event: SelectChangeEvent<string>) => {
    const value = event.target.value;
    setFilters({
      ...filters,
      role: value,
    });
  };
  
  // Handle page change
  const handlePageChange = (_: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    setPaginationParams({
      ...paginationParams,
      page: newPage + 1, // MUI pagination is 0-based, but our API is 1-based
    });
  };
  
  // Handle rows per page change
  const handleRowsPerPageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setPaginationParams({
      limit: parseInt(event.target.value, 10),
      page: 1, // Reset to first page
    });
  };
  
  // Handle edit click
  const handleEditClick = (user: User) => {
    setSelectedUser(user);
    reset({
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
    });
    setEditDialogOpen(true);
  };
  
  // Handle delete click
  const handleDeleteClick = (user: User) => {
    setUserToDelete(user);
    setDeleteDialogOpen(true);
  };
  
  // Handle form submit
  const onSubmit = (data: any) => {
    console.log('Form data:', data);
    
    if (selectedUser) {
      // Update user
      const updatedUsers = users.map((user) =>
        user.id === selectedUser.id
          ? { ...user, ...data }
          : user
      );
      setUsers(updatedUsers);
      setEditDialogOpen(false);
    }
  };
  
  // Handle delete confirm
  const handleDeleteConfirm = () => {
    if (userToDelete) {
      const updatedUsers = users.filter((user) => user.id !== userToDelete.id);
      setUsers(updatedUsers);
      setDeleteDialogOpen(false);
    }
  };
  
  // Filter users based on search and role
  const filteredUsers = users.filter((user) => {
    const matchesSearch =
      filters.search === '' ||
      user.firstName.toLowerCase().includes(filters.search.toLowerCase()) ||
      user.lastName.toLowerCase().includes(filters.search.toLowerCase()) ||
      user.email.toLowerCase().includes(filters.search.toLowerCase()) ||
      user.id.toLowerCase().includes(filters.search.toLowerCase());
    
    const matchesRole = filters.role === '' || user.role === filters.role;
    
    return matchesSearch && matchesRole;
  });
  
  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <LoadingSpinner isOpen={isLoading} message="Loading users..." />
      
      <Typography variant="h4" component="h1" gutterBottom>
        Manage Users
      </Typography>
      
      {error && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}
      
      <Paper sx={{ p: 2, mb: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
          <Box sx={{ display: 'flex', gap: 2, flexGrow: 1 }}>
            <TextField
              placeholder="Search users..."
              variant="outlined"
              size="small"
              sx={{ minWidth: 300 }}
              value={filters.search}
              onChange={handleSearch}
              InputProps={{
                startAdornment: <SearchIcon fontSize="small" sx={{ mr: 1 }} />,
              }}
            />
            
            <FormControl size="small" sx={{ minWidth: 150 }}>
              <InputLabel id="role-filter-label">Role</InputLabel>
              <Select
                labelId="role-filter-label"
                id="role-filter"
                value={filters.role}
                label="Role"
                onChange={handleRoleFilter}
              >
                <MenuItem value="">All Roles</MenuItem>
                <MenuItem value="admin">Admin</MenuItem>
                <MenuItem value="user">User</MenuItem>
              </Select>
            </FormControl>
          </Box>
        </Box>
        
        <TableContainer>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>User ID</TableCell>
                <TableCell>Name</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>Role</TableCell>
                <TableCell>Registered</TableCell>
                <TableCell align="center">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {filteredUsers.map((user) => (
                <TableRow key={user.id} hover>
                  <TableCell>{user.id.substring(0, 8)}...</TableCell>
                  <TableCell>
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                      <Avatar sx={{ mr: 1, width: 24, height: 24, bgcolor: 'primary.main' }}>
                        {user.firstName.charAt(0)}
                      </Avatar>
                      {user.firstName} {user.lastName}
                    </Box>
                  </TableCell>
                  <TableCell>{user.email}</TableCell>
                  <TableCell>
                    <Chip
                      label={user.role.toUpperCase()}
                      size="small"
                      color={user.role === 'admin' ? 'secondary' : 'primary'}
                    />
                  </TableCell>
                  <TableCell>{formatDate(user.createdAt)}</TableCell>
                  <TableCell align="center">
                    <IconButton
                      size="small"
                      onClick={() => handleEditClick(user)}
                    >
                      <EditIcon fontSize="small" />
                    </IconButton>
                    <IconButton
                      size="small"
                      color="error"
                      onClick={() => handleDeleteClick(user)}
                      disabled={user.role === 'admin'} // Prevent deleting admins
                    >
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
              
              {filteredUsers.length === 0 && (
                <TableRow>
                  <TableCell colSpan={6} align="center">
                    No users found
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
        
        <TablePagination
          component="div"
          count={filteredUsers.length} // Total count from API
          page={paginationParams.page - 1} // MUI pagination is 0-based, but our API is 1-based
          rowsPerPage={paginationParams.limit}
          onPageChange={handlePageChange}
          onRowsPerPageChange={handleRowsPerPageChange}
          rowsPerPageOptions={[5, 10, 25, 50]}
        />
      </Paper>
      
      {/* Edit User Dialog */}
      <Dialog open={editDialogOpen} onClose={() => setEditDialogOpen(false)}>
        <DialogTitle>Edit User</DialogTitle>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <Controller
                  name="firstName"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      label="First Name"
                      fullWidth
                      required
                      error={!!errors.firstName}
                      helperText={errors.firstName?.message as string}
                    />
                  )}
                />
              </Grid>
              
              <Grid item xs={12} sm={6}>
                <Controller
                  name="lastName"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      label="Last Name"
                      fullWidth
                      required
                      error={!!errors.lastName}
                      helperText={errors.lastName?.message as string}
                    />
                  )}
                />
              </Grid>
              
              <Grid item xs={12}>
                <Controller
                  name="email"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      label="Email Address"
                      fullWidth
                      required
                      error={!!errors.email}
                      helperText={errors.email?.message as string}
                    />
                  )}
                />
              </Grid>
              
              <Grid item xs={12}>
                <FormControl fullWidth>
                  <InputLabel id="role-label">Role</InputLabel>
                  <Select
                    labelId="role-label"
                    id="role"
                    value={selectedUser?.role || 'user'}
                    label="Role"
                    onChange={(e) => {
                      if (selectedUser) {
                        setSelectedUser({
                          ...selectedUser,
                          role: e.target.value as 'admin' | 'user',
                        });
                      }
                    }}
                  >
                    <MenuItem value="admin">Admin</MenuItem>
                    <MenuItem value="user">User</MenuItem>
                  </Select>
                </FormControl>
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setEditDialogOpen(false)}>Cancel</Button>
            <Button type="submit" variant="contained" color="primary">
              Save Changes
            </Button>
          </DialogActions>
        </form>
      </Dialog>
      
      {/* Delete Confirmation Dialog */}
      <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
        <DialogTitle>Delete User</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete the user "
            {userToDelete?.firstName} {userToDelete?.lastName}"? This action cannot be undone.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialogOpen(false)}>Cancel</Button>
          <Button
            variant="contained"
            color="error"
            onClick={handleDeleteConfirm}
          >
            Delete
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default AdminUsersPage;
