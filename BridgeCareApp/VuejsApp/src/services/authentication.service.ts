import axios, {AxiosResponse} from 'axios';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class AuthenticationService {
    authenticateUser(): Promise<any> {
        return axios.get('/auth/getuser', {withCredentials: true})
            .then((response: AxiosResponse) => {
                return response;
            });
    }
}
