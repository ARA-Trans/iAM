import {AxiosPromise} from 'axios';
import {nodejsBackgroundAxiosInstance} from '@/shared/utils/axios-instance';

export default class PollingService {
    /**
     * Gets the list of emitted events
     */
    static pollEvents(sessionId: string): AxiosPromise {
        return nodejsBackgroundAxiosInstance.get(`/api/Polling/${sessionId}`);
    }
}