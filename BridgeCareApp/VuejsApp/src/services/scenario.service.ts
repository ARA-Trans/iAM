import {AxiosPromise, AxiosResponse} from 'axios';
import {Scenario} from '@/shared/models/iAM/scenario';
import { axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {ScenarioCreationData} from '@/shared/models/modals/scenario-creation-data';
import {hasValue} from '@/shared/utils/has-value-util';
import { Simulation } from '@/shared/models/iAM/simulation';
import { any, propEq } from 'ramda';

export default class ScenarioService {
    static getMongoScenarios(): AxiosPromise {
        return new Promise<AxiosResponse<Scenario[]>>((resolve) => {
            nodejsAxiosInstance.get('api/GetMongoScenarios')
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

    static getLegacyScenarios(scenarios: Scenario[]): AxiosPromise {
        return new Promise<AxiosResponse<Scenario[]>>((resolve) => {
            axiosInstance.get('api/GetScenarios')
                .then((responseLegacy: AxiosResponse<Scenario[]>) => {
                    if (hasValue(responseLegacy)) {
                        var resultant: Scenario[] = [];
                        responseLegacy.data.forEach(simulation => {
                            if (!any(propEq('simulationId', simulation.simulationId), scenarios)) {
                                simulation.shared = false;
                                simulation.status = 'N/A';
                                resultant.push(simulation);
                            }
                        });

                        if (resultant.length != 0) {
                            nodejsAxiosInstance.post('api/AddMultipleScenarios', resultant)
                                .then((res: AxiosResponse<Scenario[]>) => {
                                    if (hasValue(res)) {
                                        return resolve(res);
                                    }
                                })
                                .catch((error: any) => {
                                    return resolve(error.response);
                                });
                        }
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
    static createScenario(createScenarioData: ScenarioCreationData, userId: string): AxiosPromise {
        return new Promise<AxiosResponse<Scenario>>((resolve) => {
            axiosInstance.post('/api/CreateRunnableScenario', createScenarioData)
                .then((response: AxiosResponse<Scenario>) => {
                    if (hasValue(response)) {
                        const scenarioToTrackStatus: Scenario = {
                            ...response.data,
                            shared: false,
                            status: 'New scenario'
                        };
                        nodejsAxiosInstance.post('api/GetMongoScenarios', scenarioToTrackStatus)
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

    static updateScenario(scenarioUpdateData: Simulation, scenarioId: string): AxiosPromise {
        return new Promise<AxiosResponse<Scenario>>((resolve) => {
            axiosInstance.post('/api/UpdateScenario', scenarioUpdateData)
                .then((response: AxiosResponse<Scenario>) => {
                    if (hasValue(response)) {

                        nodejsAxiosInstance.put(`api/UpdateMongoScenario/${scenarioId}`, scenarioUpdateData)
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

    static deleteScenario(scenarioId: number, scenarioMongoId: string): AxiosPromise {
        return new Promise<AxiosResponse<number>>((resolve) => {
            axiosInstance.delete(`/api/DeleteScenario/${scenarioId}`)
                .then((response: AxiosResponse<number>) => {
                    if (hasValue(response)) {
                        if (response.status == 200) {
                            nodejsAxiosInstance.delete(`api/DeleteMongoScenario/${scenarioMongoId}`)
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
        return axiosInstance.post('/api/RunSimulation', selectedScenario);
    }
}