import axios from 'axios';
import {CriteriaEditorAttribute} from '../models/criteria';
import {attributes} from '../shared/utils/mock-data';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class CriteriaEditorService {
    getCriteriaEditorAttributes(): Promise<CriteriaEditorAttribute[]> {
        const criteriaEditorAttributes = attributes.map((attr: string) => ({
            name: attr,
            values: []
        }));
        return Promise.resolve<CriteriaEditorAttribute[]>(criteriaEditorAttributes);
        // TODO: integrate axios web service call for criteria attributes
    }
}
