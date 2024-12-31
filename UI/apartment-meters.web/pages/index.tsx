import React, { useEffect, useState } from 'react';
import api from '../services/api';

interface UserData {
  name: string;
  apartmentNumber: string;
}

const UserDashboard = () => {
  const [userData, setUserData] = useState<UserData | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchData = async () => {
      const token = localStorage.getItem('token');
      if (!token) {
        window.location.href = '/login'; // Перенаправить на страницу входа
        return;
      }

      try {
        const response = await api.get<UserData>('/users/me', {
          headers: { Authorization: `Bearer ${token}` },
        });
        setUserData(response.data);
      } catch (fetchError) {
        console.error('Error fetching user data:', fetchError);
        setError('Failed to fetch user data. Please try again.');
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, []);

  if (isLoading) {
    return <p>Loading...</p>;
  }

  if (error) {
    return <p className="error">{error}</p>;
  }

  if (!userData) {
    return <p>No user data available.</p>;
  }

  return (
    <div>
      <h1>Welcome, {userData.name}</h1>
      <p>Your apartment number: {userData.apartmentNumber}</p>
    </div>
  );
};

export default UserDashboard;