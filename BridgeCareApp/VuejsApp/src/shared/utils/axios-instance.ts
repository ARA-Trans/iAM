import axios, {AxiosInstance} from 'axios';

export const axiosInstance: AxiosInstance = axios.create({
    baseURL: process.env.VUE_APP_URL
});

export const nodejsAxiosInstance: AxiosInstance = axios.create({
    baseURL: process.env.VUE_APP_NODE_URL
});

// The other nodejs axios instance has an interceptor
// which causes a spinner to be shown when it is being used,
// briefly blocking the page from being used. This instance does not
// have that spinner, and should be used for background tasks such
// as polling.
export const nodejsBackgroundAxiosInstance: AxiosInstance = axios.create({
    baseURL: process.env.VUE_APP_NODE_URL
});