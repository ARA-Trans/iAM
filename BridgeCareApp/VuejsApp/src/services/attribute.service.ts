import axios from 'axios';
import {mockAttributes} from '@/shared/utils/mock-data';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class AttributeService {
    getAttributes(): Promise<string[]> {
        return Promise.resolve<string[]>(mockAttributes);
        // TODO: integrate axios web service call for attributes
    }
    /*getCriteriaEditorAttributes(): Promise<CriteriaEditorAttribute[]> {
        const criteriaEditorAttributes = attributes.map((attr: string) => ({
            name: attr,
            values: []
        }));
        return Promise.resolve<CriteriaEditorAttribute[]>(criteriaEditorAttributes);
        // TODO: integrate axios web service call for criteria attributes
    }*/
}
