import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5017', // URL вашего бэкенда
});

export default api;