import {axiosInstance} from './axios-instance';
import MockAdapter from 'axios-mock-adapter';

export const mockAdapterInstance: any = new MockAdapter(axiosInstance);
