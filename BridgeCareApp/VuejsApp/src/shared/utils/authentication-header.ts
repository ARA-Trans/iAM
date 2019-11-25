import store from '@/store/root-store';

/**
 * Creates a header to send with all calls to the C# api
 */
export const getAuthHeader = () => {
    return {'Authorization': 'Bearer ' + (store.state as any).authentication.userAccessToken};
};