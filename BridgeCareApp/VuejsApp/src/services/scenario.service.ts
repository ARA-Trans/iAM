import axios from 'axios';
import {Analysis, ScenarioAnalysisUpsertData} from '@/shared/models/iAM/scenario';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class ScenarioService {
    applyAnalysisToScenario(scenarioAnalysisUpsertData: ScenarioAnalysisUpsertData): Promise<Analysis> {
        return Promise.resolve<Analysis>(scenarioAnalysisUpsertData.analysis);
        // TODO: add axios web service call for upserting analysis data for a scenario
    }
}