import Vue from 'vue';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {AxiosPromise, AxiosResponse} from 'axios';
import {InvestmentLibrary} from '@/shared/models/iAM/investment';
import {db} from '@/firebase';
import {isNil} from 'ramda';
import DataSnapshot = firebase.database.DataSnapshot;
import forEach from 'ramda/es/forEach';

axios.defaults.baseURL = process.env.VUE_APP_URL;

const nodeAPIInstance = axios.create({
    baseURL: process.env.VUE_APP_NODE_URL
});

Vue.prototype.$nodeApi = nodeAPIInstance;

export default class InvestmentEditorService extends Vue {
    /**
     * Gets all investment libraries
     */
    async getInvestmentLibraries(): Promise<InvestmentLibrary[]> {

        return axios.get<InvestmentLibrary[]>(`http://localhost:4000/api/investmentLibraries/`)
            //@ts-ignore
            .then((response: AxiosResponse) => {
                if (!isNil(response)) {
                    return response.data;
                }
                return Promise.reject('Failed to get investment library');
            });
    }

    /**
     * Creates an investment library
     * @param createInvestmentLibraryData The investment library create data
     */
    createInvestmentLibrary(createdInvestmentLibrary: InvestmentLibrary): Promise<InvestmentLibrary> {

        return axios.post<InvestmentLibrary[]>(`http://localhost:4000/api/investmentLibraries/`, createdInvestmentLibrary)
            .then((response: AxiosResponse) => {
                if (!isNil(response)) {
                    return response.data;
                }
                return Promise.reject('Failed to get investment library');
            });
    }

    /**
     * Updates an investment library
     * @param updateInvestmentLibraryData The investment library updated data
     */
    updateInvestmentLibrary(updatedInvestmentLibrary: InvestmentLibrary): Promise<InvestmentLibrary> {
        return axios.put<InvestmentLibrary[]>(`http://localhost:4000/api/investmentLibraries/${updatedInvestmentLibrary.id}`, updatedInvestmentLibrary)
            .then((response: AxiosResponse) => {
                if (!isNil(response)) {
                    return response.data;
                }
                return Promise.reject('Failed to get investment library');
            });
    }

    /**
     * Gets a scenario's investment library data
     * @param selectedScenarioId Scenario id to use in finding a scenario's investment library data
     */
    static getScenarioInvestmentLibrary(selectedScenarioId: number): AxiosPromise<InvestmentLibrary> {
        return axiosInstance.get<InvestmentLibrary>(`/api/GetScenarioInvestmentLibrary/${selectedScenarioId}`);
    }

    /**
     * Upserts a scenario's investment library data
     * @param saveScenarioInvestmentLibraryData The scenario investment library upsert data
     */
    static saveScenarioInvestmentLibrary(saveScenarioInvestmentLibraryData: InvestmentLibrary): AxiosPromise<InvestmentLibrary> {
        return axiosInstance.post<InvestmentLibrary>('/api/SaveScenarioInvestmentLibrary', saveScenarioInvestmentLibraryData);
    }
}