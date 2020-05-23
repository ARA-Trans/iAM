import {UserTokens} from '@/shared/models/iAM/authentication';

/**
 * Creates a header to send with all calls to the C# api
 */
export const getAuthorizationHeader = () => {
    if (localStorage.getItem('UserTokens')) {
        const userTokens: UserTokens = JSON.parse(localStorage.getItem('UserTokens') as string) as UserTokens;
        return {'Authorization': `Bearer ${userTokens.access_token}`};
    }
};
