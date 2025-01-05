import axios from 'axios';

const api = axios.create({
  baseURL: `http://localhost:5017`,
});

export default api;