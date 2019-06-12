import {AxiosPromise, AxiosResponse} from 'axios';
import {Scenario} from '@/shared/models/iAM/scenario';
import { axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {db} from '@/firebase';
import DataSnapshot = firebase.database.DataSnapshot;
import moment from 'moment';
import {CreateScenarioData} from '@/shared/models/modals/scenario-creation-data';
import {hasValue} from '@/shared/utils/has-value-util';

export default class ScenarioService {
    static getUserScenarios(userId: string): AxiosPromise<Scenario[]> {
        return new Promise<AxiosResponse<Scenario[]>>((resolve) => {

            nodejsAxiosInstance.get<Scenario[]>('api/scenarios')
                .then((response: AxiosResponse<Scenario[]>) => {
                    if (hasValue(response)) {
                        return resolve(response);
                    }
                })
                .catch((error: any) => {
                    return resolve(error.response);
                });
        });
    }

    /**
     * Creates a new scenario
     * @param createScenarioData Scenario create data
     * @param userId Current user's id
     */
    static createScenario(createScenarioData: CreateScenarioData, userId: string): AxiosPromise<Scenario> {
        return new Promise<AxiosResponse<Scenario>>((resolve) => {
            axiosInstance.post<Scenario>('/api/CreateNewSimulation', createScenarioData)
                .then((response: AxiosResponse<Scenario>) => {
                    if (hasValue(response)) {
                        const scenarioToTrackStatus: Scenario = {
                            ...response.data,
                            shared: false,
                            status: 'New scenario'
                        };
                        nodejsAxiosInstance.post<Scenario>('api/scenarios', scenarioToTrackStatus)
                            .then((res: AxiosResponse<Scenario>) => {
                                if (hasValue(res)) {
                                    return resolve(res);
                                }
                            })
                            .catch((error: any) => {
                                const axiosResponse: AxiosResponse<Scenario> = {
                                    data: {} as Scenario,
                                    status: 500,
                                    statusText: error.toString(),
                                    headers: {},
                                    config: {}
                                };
                                return resolve(axiosResponse);
                            });
                    }
                });
        });
    }

    /**
     * Runs a simulation for a specific scenario
     * @param selectedScenario The scenario to run the simulation on
     * @param userId Current user's id
     */
    static runScenarioSimulation(selectedScenario: Scenario, userId: string): AxiosPromise<any> {
        return new Promise<AxiosResponse<any>>((resolve) => {
            db.ref('scenarioStatus').once('value')
                .then((snapshot: DataSnapshot) => {
                    const currentDate: moment.Moment = moment();
                    const selectedScenarioPath: string =
                        `Scenario_${selectedScenario.networkId.toString()}_${selectedScenario.simulationId.toString()}`;
                    if (snapshot.hasChild(selectedScenarioPath)) {
                        selectedScenario.status = 'Started';
                        selectedScenario.lastModifiedDate = new Date(currentDate.toISOString());
                        const firebaseScenario: any = {
                            ...selectedScenario,
                            lastModifiedDate: currentDate.toISOString(),
                            // [Note]: this will be removed
                            owner: userId
                        };
                        db.ref('scenarioStatus').child(selectedScenarioPath).update(firebaseScenario)
                            .then(() => {
                                return axiosInstance.post<any>('/api/RunSimulation', selectedScenario);
                            });
                    } else {
                        const firebaseScenario: any = {
                            ...selectedScenario,
                            status: 'Started',
                            created: currentDate.toISOString(),
                            lastModified: currentDate.toISOString(),
                            // [Note]: this will be removed
                            owner: userId
                        };
                        db.ref('scenarioStatus').child(selectedScenarioPath).set(firebaseScenario)
                            .then(() => {
                                return axiosInstance.post<any>('/api/RunSimulation', selectedScenario);
                            });
                    }
                })
                .catch((error: any) => {
                    const response: AxiosResponse<any> = {
                        data: {},
                        status: 500,
                        statusText: error.toString(),
                        headers: {},
                        config: {}
                    };
                    return resolve(response);
                });
        });
        // TODO: replace the above code with the following when mongo db implemented
        // return axiosInstance.post<any>('/api/RunSimulation', selectedScenario);
    }
}