import MockAdapter from 'axios-mock-adapter';
import axios, {AxiosInstance} from 'axios';

export const mockAxiosInstance: AxiosInstance = axios.create({
    baseURL: ''
});

export const mockAdapter = new MockAdapter(mockAxiosInstance);
