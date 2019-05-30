import Vue from 'vue';
import axios, {AxiosResponse} from 'axios';
import {InvestmentLibrary, InvestmentLibraryBudgetYear} from '@/shared/models/iAM/investment';
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

        //return new Promise<InvestmentLibrary[]>((resolve, reject) => {
        //    db.ref('investmentLibraries').once('value')
        //        .then((snapshot: DataSnapshot) => {
        //            const investmentLibraries: InvestmentLibrary[] = [];
        //            const results = snapshot.val();
        //            for (let key in results) {
        //                investmentLibraries.push({
        //                    id: results[key].id,
        //                    name: results[key].name,
        //                    inflationRate: results[key].inflationRate,
        //                    discountRate: results[key].discountRate,
        //                    description: results[key].description,
        //                    budgetOrder: isNil(results[key].budgetOrder) ? [] : results[key].budgetOrder,
        //                    budgetYears: isNil(results[key].budgetYears) ? [] : results[key].budgetYears
        //                });
        //            }
        //            return resolve(investmentLibraries);
        //        })
        //        .catch((error: any) => reject(`Failed to get investment libraries: ${error.toString()}`));
        //});
    }

    /**
     * Creates an investment library
     * @param createdInvestmentLibrary The investment library create data
     */
    createInvestmentLibrary(createdInvestmentLibrary: InvestmentLibrary): Promise<InvestmentLibrary> {

        return axios.post<InvestmentLibrary[]>(`http://localhost:4000/api/investmentLibraries/`, createdInvestmentLibrary)
            .then((response: AxiosResponse) => {
                if (!isNil(response)) {
                    return response.data;
                }
                return Promise.reject('Failed to get investment library');
            });
        //return new Promise<InvestmentLibrary>((resolve, reject) => {
        //    db.ref('investmentLibraries')
        //        .child('Investment_' + createdInvestmentLibrary.id)
        //        .set(createdInvestmentLibrary)
        //        .then(() => {
        //            return resolve(createdInvestmentLibrary);
        //        })
        //        .catch((error: any) => reject(`Failed to create investment library: ${error.toString()}`));
        //});
    }

    /**
     * Updates an investment library
     * @param updatedInvestmentLibrary The investment library updated data
     */
    updateInvestmentLibrary(updatedInvestmentLibrary: InvestmentLibrary): Promise<InvestmentLibrary> {
        return axios.put<InvestmentLibrary[]>(`http://localhost:4000/api/investmentLibraries/${updatedInvestmentLibrary.id}`, updatedInvestmentLibrary)
            .then((response: AxiosResponse) => {
                if (!isNil(response)) {
                    return response.data;
                }
                return Promise.reject('Failed to get investment library');
            });
        //return new Promise<InvestmentLibrary>((resolve, reject) => {
        //    db.ref('investmentLibraries')
        //        .child('Investment_' + updatedInvestmentLibrary.id)
        //        .update(updatedInvestmentLibrary)
        //        .then(() => {
        //            return resolve(updatedInvestmentLibrary);
        //        })
        //        .catch((error: any) => reject(`Failed to update investment library: ${error.toString()}`));
        //});
    }

    /**
     * Gets a scenario's investment library data
     * @param selectedScenarioId Scenario id to use in finding a scenario's investment library data
     */
    getScenarioInvestmentLibrary(selectedScenarioId: number): Promise<InvestmentLibrary> {
        return axios.get<InvestmentLibrary>(`/api/GetScenarioInvestmentLibrary/${selectedScenarioId}`)
            .then((response: AxiosResponse) => {
                if (!isNil(response)) {
                    return response.data;
                }
                return Promise.reject('Failed to get scenario investment library');
            });
    }

    /**
     * Upserts a scenario's investment library data
     * @param upsertedScenarioInvestmentLibrary The scenario investment library upsert data
     */
    upsertScenarioInvestmentLibrary(upsertedScenarioInvestmentLibrary: InvestmentLibrary): Promise<InvestmentLibrary> {
        return axios.post<InvestmentLibrary>('/api/SaveScenarioInvestmentLibrary', upsertedScenarioInvestmentLibrary)
            .then((response: AxiosResponse) => {
                if (!isNil(response)) {
                    return response.data;
                }
                return Promise.reject('Failed to apply investment library');
            });
    }
}