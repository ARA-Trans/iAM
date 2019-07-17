import {AxiosPromise, AxiosResponse} from 'axios';
import {Scenario} from '@/shared/models/iAM/scenario';
import { axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {CreateScenarioData} from '@/shared/models/modals/scenario-creation-data';
import {hasValue} from '@/shared/utils/has-value-util';
import { Simulation } from '@/shared/models/iAM/simulation';

export default class ScenarioService {
    static getUserScenarios(userId: string): AxiosPromise {
        return new Promise<AxiosResponse<Scenario[]>>((resolve) => {

            nodejsAxiosInstance.get('api/scenarios')
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
    static createScenario(createScenarioData: CreateScenarioData, userId: string): AxiosPromise {
        return new Promise<AxiosResponse<Scenario>>((resolve) => {
            axiosInstance.post('/api/CreateRunnableSimulation', createScenarioData)
                .then((response: AxiosResponse<Scenario>) => {
                    if (hasValue(response)) {
                        const scenarioToTrackStatus: Scenario = {
                            ...response.data,
                            shared: false,
                            status: 'New scenario'
                        };
                        nodejsAxiosInstance.post('api/scenarios', scenarioToTrackStatus)
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

    static updateScenario(updateScenarioData: Simulation, scenarioId: string): AxiosPromise {
        return new Promise<AxiosResponse<Scenario>>((resolve) => {
            axiosInstance.post('/api/UpdateSimulationName', updateScenarioData)
                .then((response: AxiosResponse<Scenario>) => {
                    if (hasValue(response)) {

                        nodejsAxiosInstance.put(`api/updateScenarios/${scenarioId}`, updateScenarioData)
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

    static deleteScenario(simulationId: number, scenarioId: string): AxiosPromise {
        return new Promise<AxiosResponse<number>>((resolve) => {
            axiosInstance.delete(`/api/DeleteSimulation/${simulationId}`)
                .then((response: AxiosResponse<number>) => {
                    if (hasValue(response)) {
                        if (response.status == 200) {
                            nodejsAxiosInstance.delete(`api/scenarios/${scenarioId}`)
                                .then((res: AxiosResponse<number>) => {
                                    if (hasValue(res)) {
                                        return resolve(res);
                                    }
                                })
                                .catch((error: any) => {
                                    return resolve(error.response);
                                });
                        }
                        if (response.status == 404) {
                            return resolve(response);
                        }
                    }
                })
                .catch((error: any) => {
                    return resolve(error.response);
                });
        });
    }

    /**
     * Runs a simulation for a specific scenario
     * @param selectedScenario The scenario to run the simulation on
     * @param userId Current user's id
     */
    static runScenarioSimulation(selectedScenario: Scenario, userId: string): AxiosPromise {
        return new Promise<AxiosResponse<any>>((resolve) => {
            return axiosInstance.post('/api/RunSimulation', selectedScenario);

        });
    }
}