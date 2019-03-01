import Vue from 'vue';
import axios from 'axios'
import {Scenario} from '@/models/scenario';
import {sharedScenarios, userScenarios} from '@/shared/utils/mock-data';
import * as moment from 'moment';


axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class ScenarioService extends Vue {
    getUserScenarios(userId: number): Promise<Scenario[]> {
        // @ts-ignore
        const dateMinus2Days = new Date(moment().subtract(2, 'days').format('YYYY-MM-DDTHH:mm:ss.SSS'));
        const scenarios: Scenario[] = [
            ...userScenarios.map((s: Scenario) => {
                return {
                    ...s,
                    createdDate: dateMinus2Days
                }
            }),
            ...sharedScenarios.map((s: Scenario) => {
                return {
                    ...s,
                    createdDate: dateMinus2Days
                }
            })
        ];
        return Promise.resolve<Scenario[]>(scenarios);
        // TODO: integrate axios web service call for user scenarios
    }
}
