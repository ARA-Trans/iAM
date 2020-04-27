import {UserInfo} from '@/shared/models/iAM/authentication';
import {parseLDAP} from './parse-ldap';

export const getUserInfo = () => {
    return JSON.parse(localStorage.getItem('UserInfo') as string) as UserInfo;
};

export const getUserName = () => {
    return parseLDAP(getUserInfo().sub)[0];
};

export const getUserRoles = () => {
    return parseLDAP(getUserInfo().roles);
};