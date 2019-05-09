import axios, {AxiosResponse} from 'axios';
import {emptyInvestmentLibrary, InvestmentLibrary} from '@/shared/models/iAM/investment';
import {db} from '@/firebase';
import {isNil} from 'ramda';
import DataSnapshot = firebase.database.DataSnapshot;
import {Action} from 'vuex-class';
import Vue from 'vue';
import {clone} from 'ramda';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class InvestmentEditorService extends Vue {
    @Action('setErrorMessage') setErrorMessageAction: any;

    /**
     * Gets all investment libraries
     */
    async getInvestmentLibraries(): Promise<InvestmentLibrary[]> {
        return new Promise<InvestmentLibrary[]>((resolve) => {
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
                .catch((error: any) => this.setErrorMessageAction(`Failed to get investment libraries: ${error}`));
        });
    }

    /**
     * Creates an investment library
     * @param createdInvestmentLibrary The investment library create data
     */
    createInvestmentLibrary(createdInvestmentLibrary: InvestmentLibrary): Promise<InvestmentLibrary> {
        return new Promise<InvestmentLibrary>((resolve) => {
            db.ref('investmentLibraries')
                .child('Investment_' + createdInvestmentLibrary.id)
                .set(createdInvestmentLibrary)
                .then(() => {
                    return resolve(createdInvestmentLibrary);
                })
                .catch((error: any) => this.setErrorMessageAction(`Failed to create investment library: ${error}`));
        });
    }

    /**
     * Updates an investment library
     * @param updatedInvestmentLibrary The investment library updated data
     */
    updateInvestmentLibrary(updatedInvestmentLibrary: InvestmentLibrary): Promise<InvestmentLibrary> {
        return new Promise<InvestmentLibrary>((resolve) => {
            db.ref('investmentLibraries')
                .child('Investment_' + updatedInvestmentLibrary.id)
                .update(updatedInvestmentLibrary)
                .then(() => {
                    return resolve(updatedInvestmentLibrary);
                })
                .catch((error: any) => this.setErrorMessageAction(`Failed to update investment library: ${error}`));
        });
    }

    /**
     * Gets a scenario's investment library data
     * @param selectedScenarioId
     */
    getScenarioInvestmentLibrary(selectedScenarioId: number): Promise<InvestmentLibrary> {
        return axios.get<InvestmentLibrary>(`/api/GetInvestmentStrategies/${selectedScenarioId}`)
            .then((response: AxiosResponse) => {
                if (!isNil(response)) {
                    
                    // TODO: uncomment the following line when service has been updated
                    // return response.data;
                }
            });
    }

    /**
     * Upserts a scenario's investment library data
     * @param upsertedScenarioInvestmentLibrary The scenario investment library upsert data
     */
    upsertScenarioInvestmentLibrary(upsertedScenarioInvestmentLibrary: InvestmentLibrary): Promise<InvestmentLibrary> {
        return axios.post<InvestmentLibrary>('/api/SaveInvestmentStrategy', upsertedScenarioInvestmentLibrary)
            .then((response: AxiosResponse) => {
                if (!isNil(response)) {
                    return response.data;
                }
                return clone(emptyInvestmentLibrary);
            });
    }
}