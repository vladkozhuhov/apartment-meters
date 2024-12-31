import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:44383/api', // URL вашего бэкенда
});

export default api;