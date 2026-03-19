import axios from 'axios';

// axios calls will use this base URL
const apiClient = axios.create({
    baseURL: 'http://localhost:5202/api',
    headers: {
        'Content-Type': 'application/json',
    },
});

// interceptor for outgoing requests to the API - jwt token attached in header
apiClient.interceptors.request.use((config => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
}));

export default apiClient;