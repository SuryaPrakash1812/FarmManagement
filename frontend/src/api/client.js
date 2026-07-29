import axios from 'axios';
export const api = axios.create({ baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api' });
api.interceptors.request.use((config) => { const token = localStorage.getItem('farm_token'); if (token) config.headers.Authorization = `Bearer ${token}`; return config; });
export async function login(email, password) { const { data } = await api.post('/auth/login', { email, password }); localStorage.setItem('farm_token', data.token); localStorage.setItem('farm_user', JSON.stringify(data.user)); return data; }
