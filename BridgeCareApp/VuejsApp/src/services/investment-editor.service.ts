import Vue from 'vue';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {AxiosPromise, AxiosResponse} from 'axios';
import {InvestmentLibrary} from '@/shared/models/iAM/investment';
import {db} from '@/firebase';
import {isNil} from 'ramda';
import DataSnapshot = firebase.database.DataSnapshot;

export default class InvestmentEditorService extends Vue {
    /**
     * Gets all investment libraries
     */
    static getInvestmentLibraries(): AxiosPromise<InvestmentLibrary[]> {
        return new Promise<AxiosResponse<InvestmentLibrary[]>>((resolve) => {
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
                    const response: AxiosResponse<InvestmentLibrary[]> = {
                        data: investmentLibraries,
                        status: 200,
                        statusText: 'Success',
                        headers: {},
                        config: {}
                    };
                    return resolve(response);
                })
                .catch((error: any) => {
                    const response: AxiosResponse<InvestmentLibrary[]> = {
                        data: [] as InvestmentLibrary[],
                        status: 500,
                        statusText: error.toString(),
                        headers: {},
                        config: {}
                    };
                    resolve(response);
                });
        });
        // TODO: replace the above code with the following when mongo db implemented
        // return axiosInstance.get<InvestmentLibrary[]>('/api/GetInvestmentLibraries');
    }

    /**
     * Creates an investment library
     * @param createInvestmentLibraryData The investment library create data
     */
    static createInvestmentLibrary(createInvestmentLibraryData: InvestmentLibrary): AxiosPromise<InvestmentLibrary> {
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
        });
        // TODO: replace the above code with the following when mongo db implemented
        // return axiosInstance.post<InvestmentLibrary>('/api/CreateInvestmentLibrary', createInvestmentLibraryData);
    }

    /**
     * Updates an investment library
     * @param updateInvestmentLibraryData The investment library updated data
     */
    updateInvestmentLibrary(updateInvestmentLibraryData: InvestmentLibrary): AxiosPromise<InvestmentLibrary> {
        return new Promise<AxiosResponse<InvestmentLibrary>>((resolve) => {
            db.ref('investmentLibraries')
                .child('Investment_' + updateInvestmentLibraryData.id)
                .update(updateInvestmentLibraryData)
                .then(() => {
                    const response: AxiosResponse<InvestmentLibrary> = {
                        data: updateInvestmentLibraryData,
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
        });
        // TODO: replace the above with the following when mongo db implemented
        // return axiosInstance.post<InvestmentLibrary>('/api/UpdateInvestmentLibrary', updateInvestmentLibraryData);
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