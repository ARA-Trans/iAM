import Vue from 'vue';
import axios, {AxiosPromise, AxiosResponse} from 'axios';
import {InvestmentLibrary} from '@/shared/models/iAM/investment';
import {db} from '@/firebase';
import {isNil} from 'ramda';
import DataSnapshot = firebase.database.DataSnapshot;

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class InvestmentEditorService extends Vue {
    /**
     * Gets all investment libraries
     */
    async getInvestmentLibraries(): Promise<InvestmentLibrary[]> {
        return new Promise<InvestmentLibrary[]>((resolve, reject) => {
            db.ref('investmentLibraries').once('value')
                .then((snapshot: DataSnapshot) => {
                    const investmentLibraries: InvestmentLibrary[] = [];
                    const results = snapshot.val();
                    for (let key in results) {
                        investmentLibraries.push({
                            id: results[key].id,
                            name: results[key].name,
                            inflationRate: results[key].inflationRate,
                            discountRate: results[key].discountRate,
                            description: results[key].description,
                            budgetOrder: isNil(results[key].budgetOrder) ? [] : results[key].budgetOrder,
                            budgetYears: isNil(results[key].budgetYears) ? [] : results[key].budgetYears
                        });
                    }
                    return resolve(investmentLibraries);
                })
                .catch((error: any) => reject(`Failed to get investment libraries: ${error.toString()}`));
        });
    }

    /**
     * Creates an investment library
     * @param createInvestmentLibraryData The investment library create data
     */
    static createInvestmentLibrary(createInvestmentLibraryData: InvestmentLibrary): AxiosPromise<InvestmentLibrary> {
        /*return new Promise<InvestmentLibrary>((resolve, reject) => {
            db.ref('investmentLibraries')
                .child('Investment_' + createInvestmentLibraryData.id)
                .set(createInvestmentLibraryData)
                .then(() => {
                    return resolve(createInvestmentLibraryData);
                })
                .catch((error: any) => reject(`Failed to create investment library: ${error.toString()}`));
        });*/
        return new Promise<AxiosResponse<InvestmentLibrary>>((resolve) => {
            db.ref('investmentLibraries')
                .child('Investment_' + createInvestmentLibraryData.id)
                .set(createInvestmentLibraryData)
                .then(() => {
                    const response: AxiosResponse<InvestmentLibrary> = {
                        data: createInvestmentLibraryData,
                        status: 200,
                        statusText: 'Success',
                        headers: {},
                        config: {}
                    };
                    return resolve(response);
                })
                .catch((error: any) => {
                    const response: AxiosResponse<InvestmentLibrary> = {
                        data: {} as InvestmentLibrary,
                        status: 500,
                        statusText: error.toString(),
                        headers: {},
                        config: {}
                    };
                    resolve(response);
                });
        }) as AxiosPromise<InvestmentLibrary>;
    }

    /**
     * Updates an investment library
     * @param updatedInvestmentLibrary The investment library updated data
     */
    updateInvestmentLibrary(updatedInvestmentLibrary: InvestmentLibrary): Promise<InvestmentLibrary> {
        return new Promise<InvestmentLibrary>((resolve, reject) => {
            db.ref('investmentLibraries')
                .child('Investment_' + updatedInvestmentLibrary.id)
                .update(updatedInvestmentLibrary)
                .then(() => {
                    return resolve(updatedInvestmentLibrary);
                })
                .catch((error: any) => reject(`Failed to update investment library: ${error.toString()}`));
        });
    }

    /**
     * Gets a scenario's investment library data
     * @param selectedScenarioId Scenario id to use in finding a scenario's investment library data
     */
    static getScenarioInvestmentLibrary(selectedScenarioId: number): AxiosPromise<InvestmentLibrary> {
        return axios.get<InvestmentLibrary>(`/api/GetInvestmentStrategies/${selectedScenarioId}`);
    }

    /**
     * Upserts a scenario's investment library data
     * @param saveScenarioInvestmentLibraryData The scenario investment library upsert data
     */
    static saveScenarioInvestmentLibrary(saveScenarioInvestmentLibraryData: InvestmentLibrary): AxiosPromise<InvestmentLibrary> {
        return axios.post<InvestmentLibrary>('/api/SaveInvestmentStrategy', saveScenarioInvestmentLibraryData);
    }
}