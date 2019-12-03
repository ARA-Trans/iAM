import store from '@/store/root-store';

/**
 * Creates a header to send with all calls to the C# api
 */
export const getAuthorizationHeader = () => {
    return {'Authorization': 'Bearer ' + (store.state as any).authentication.userIdToken};
};