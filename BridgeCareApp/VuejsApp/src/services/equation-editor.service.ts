import axios from 'axios';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class EquationEditorService {
    /**
     * Checks an equation's validity
     * @param equation The equation to check
     */
    checkEquationValidity(equation: string): Promise<boolean> {
        return Promise.resolve<boolean>(true);
        // TODO: add axios web service call to perform actual validation check on equation
    }
}