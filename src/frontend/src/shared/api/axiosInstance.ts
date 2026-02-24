import axios from 'axios';
import { useAuthStore } from '@/shared/hooks/useAuth';

const axiosInstance = axios.create({
    baseURL: '/api',
    headers: { 'Content-Type': 'application/json' },
});

// Request interceptor: attach JWT token
axiosInstance.interceptors.request.use((config) => {
    const token = useAuthStore.getState().token;
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// Response interceptor: handle 401 → refresh or logout
axiosInstance.interceptors.response.use(
    (response) => response,
    async (error) => {
        if (error.response?.status === 401) {
            useAuthStore.getState().logout();
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

export default axiosInstance;
