import Vue from 'vue';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {AxiosPromise, AxiosResponse} from 'axios';
import {InvestmentLibrary} from '@/shared/models/iAM/investment';
import {db} from '@/firebase';
import {isNil} from 'ramda';
import DataSnapshot = firebase.database.DataSnapshot;
import forEach from 'ramda/es/forEach';

export default class InvestmentEditorService extends Vue {
    /**
     * Gets all investment libraries
     */
    getInvestmentLibraries(): AxiosPromise<InvestmentLibrary[]> {

        return axiosInstance.get<InvestmentLibrary[]>(`http://localhost:4000/api/investmentLibraries/`);
    }

    /**
     * Creates an investment library
     * @param createInvestmentLibraryData The investment library create data
     */
    createInvestmentLibrary(createdInvestmentLibrary: InvestmentLibrary): AxiosPromise<InvestmentLibrary> {

        return axiosInstance.post<InvestmentLibrary[]>(`http://localhost:4000/api/investmentLibraries/`, createdInvestmentLibrary)
            .then((response: any) => {
                if (!isNil(response)) {
                    return response;
                }
                return Promise.reject('Failed to get investment library');
            });
    }

    /**
     * Updates an investment library
     * @param updateInvestmentLibraryData The investment library updated data
     */
    updateInvestmentLibrary(updatedInvestmentLibrary: InvestmentLibrary): AxiosPromise<InvestmentLibrary> {
        return axiosInstance.put<InvestmentLibrary[]>(`http://localhost:4000/api/investmentLibraries/${updatedInvestmentLibrary.id}`, updatedInvestmentLibrary)
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