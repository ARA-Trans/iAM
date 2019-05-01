import axios from 'axios';
import {Analysis, emptyAnalysis} from '@/shared/models/iAM/scenario';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class ScenarioService {
    getScenarioAnalysisData(scenarioId: number): Promise<Analysis> {
        return Promise.resolve<Analysis>({...emptyAnalysis});
        // TODO: add axios web service call to get a scenario's analysis data
    }
    applyAnalysisDataToScenario(analysis: Analysis): Promise<boolean> {
        return Promise.resolve<boolean>(true);
        // TODO: add axios web service call to upsert analysis data for a scenario
    }

    createNewScenario(networkId: number, simulationName: string): Promise<any> {
        return axios({
            method: 'post',
            url: `/api/CreateNewSimulation/${networkId}/${simulationName}`,
            data: {}
        }).then((response: any) => {
            return response;
        });
    }
}