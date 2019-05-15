import axios from 'axios';
import {Analysis, emptyAnalysis, Scenario} from '@/shared/models/iAM/scenario';
import {clone} from 'ramda';
import {mockBenefitAttributes} from '@/shared/utils/mock-data';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class ScenarioService {
    getScenarioAnalysisData(scenarioId: number): Promise<Analysis> {
        return Promise.resolve<Analysis>(clone(emptyAnalysis));
        // TODO: add axios web service call to get a scenario's analysis data
    }

    getBenefitAttributes(): Promise<string[]> {
        return Promise.resolve<string[]>(mockBenefitAttributes);
        // TODO: add axios web service call to get benefit attributes
    }

    applyAnalysisDataToScenario(analysis: Analysis): Promise<boolean> {
        return Promise.resolve<boolean>(true);
        // TODO: add axios web service call to upsert analysis data for a scenario
    }

    getScenarios(): Promise<Scenario[]> {
        return axios.get('/api/Simulations')
            .then((response: any) => {
                return response.data as Promise<Scenario[]>;
            });
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

    uploadCommittedProjectsFiles(files: File[]): Promise<any> {
        return Promise.resolve<any>({} as any);
        // TODO: add axios web service call to upload excel file
    }
}