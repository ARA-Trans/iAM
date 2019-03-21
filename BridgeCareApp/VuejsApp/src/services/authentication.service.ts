import axios from 'axios';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class AuthenticationService {
    getAuthentication(): Promise<any> {
        return axios.get('/auth/getuser', { withCredentials: true })
            .then(response => {
                return response.data;
            })
            .catch(error => {
                console.log(error);
            });
    }
}
