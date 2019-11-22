import axios, {AxiosInstance} from 'axios';

export const axiosInstance: AxiosInstance = axios.create({
    baseURL: process.env.VUE_APP_URL
});

export const nodejsAxiosInstance: AxiosInstance = axios.create({
    baseURL: process.env.VUE_APP_NODE_URL
});

export const esecAxiosInstance: AxiosInstance = axios.create({
    baseURL: process.env.VUE_APP_ESEC_URL
});
