import { AxiosPromise, AxiosResponse } from 'axios';
import { Scenario, ScenarioUser } from '@/shared/models/iAM/scenario';
import { axiosInstance, nodejsAxiosInstance } from '@/shared/utils/axios-instance';
import { ScenarioCreationData } from '@/shared/models/modals/scenario-creation-data';
import { hasValue } from '@/shared/utils/has-value-util';
import { Simulation } from '@/shared/models/iAM/simulation';
import { any, propEq } from 'ramda';
import { http2XX } from '@/shared/utils/http-utils';
import { convertFromVueToMongo } from '@/shared/utils/mongo-model-conversion-utils';

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

    /**
     * Updates mongodb to match the scenarios from the legacy database
     */
    static getLegacyScenarios(): AxiosPromise {
        return new Promise<AxiosResponse<Scenario[]>>((resolve) => {
            axiosInstance.get('api/GetScenarios')
                .then((responseLegacy: AxiosResponse<Scenario[]>) => {
                    if (hasValue(responseLegacy)) {
                        this.getMongoScenarios()
                            .then((responseMongo: AxiosResponse<Scenario[]>) => {
                                if (hasValue(responseMongo)) {
                                    var scenariosToAdd = this.getMissingScenarios(responseLegacy.data, responseMongo.data);
                                    var scenariosToRemove = this.getMissingScenarios(responseMongo.data, responseLegacy.data);
                                    if (scenariosToAdd.length > 0) {
                                        nodejsAxiosInstance.post('api/AddMultipleScenarios', scenariosToAdd)
                                            .then((res: AxiosResponse<Scenario[]>) => {
                                                if (hasValue(res)) {
                                                    return resolve(res);
                                                }
                                            })
                                            .catch((error: any) => resolve(error.response));
                                    }
                                    if (scenariosToRemove.length > 0) {
                                        this.removeMongoScenarios(scenariosToRemove).then((res: AxiosResponse) => {
                                            if (hasValue(res)) {
                                                return resolve(res);
                                            }
                                        }).catch((error: any) => resolve(error.response));
                                    }
                                    return resolve(responseLegacy);
                                }
                            })
                            .catch((error: any) => resolve(error.response));
                    }
                })
                .catch((error: any) => resolve(error.response));
        });
    }

    /**
     * Creates a new scenario
     * @param createScenarioData Scenario create data
     * @param userId Current user's id
     */
    static createScenario(createScenarioData: ScenarioCreationData, userId: string): AxiosPromise {
        return new Promise<AxiosResponse<Scenario>>((resolve) => {
            axiosInstance.post('/api/CreateScenario', createScenarioData)
                .then((response: AxiosResponse<Scenario>) => {
                    if (hasValue(response)) {
                        const scenarioToTrackStatus: Scenario = {
                            ...response.data,
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

    static updateScenarioUsers(scenario: Scenario): AxiosPromise {
        return new Promise<AxiosResponse<Scenario>>((resolve) => {
            axiosInstance.post(`/api/SetScenarioUsers/${scenario.simulationId}`, scenario.users)
                .then((response: AxiosResponse<Scenario>) => {
                    if (hasValue(response)) {
                        nodejsAxiosInstance.put(`api/UpdateMongoScenario/${scenario.id}`, convertFromVueToMongo({ users: response.data }));
                    }
                });
        });
    }

    static updateScenarioStatus(scenarioStatus: String, scenarioId: number): AxiosPromise {
        return new Promise<AxiosResponse<Scenario>>((resolve) => {
            nodejsAxiosInstance.put(`api/UpdateMongoScenarioStatus/${scenarioId}`, { status: scenarioStatus })
                .then((response: AxiosResponse<Scenario>) => {
                    if (hasValue(response)) {
                        return resolve(response);
                    }
                });
        });
    }

    static cloneScenario(scenarioId: number): AxiosPromise {
        return new Promise<AxiosResponse>((resolve) => {
            axiosInstance.post(`/api/CloneScenario/${scenarioId}`)
                .then((response: AxiosResponse) => {
                    if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                        const newScenario: Scenario = {
                            ...response.data,
                            status: 'New scenario'
                        };
                        nodejsAxiosInstance.post('api/GetMongoScenarios', newScenario)
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
        return new Promise<AxiosResponse>((resolve) => {
            axiosInstance.delete(`/api/DeleteScenario/${scenarioId}`)
                .then((serverResponse: AxiosResponse) => {
                    if (hasValue(serverResponse, 'status') && http2XX.test(serverResponse.status.toString())) {
                        nodejsAxiosInstance.delete(`api/DeleteMongoScenario/${scenarioMongoId}`)
                            .then((mongoResponse: AxiosResponse) => {
                                return resolve(mongoResponse);
                            })
                            .catch((error: any) => {
                                return resolve(error.response);
                            });
                    } else {
                        return resolve(serverResponse);
                    }
                })
                .catch((error: any) => {
                    return resolve(error.response);
                });
        });
    }

    static deleteDuplicateMongoScenario(scenarios: Scenario[]): AxiosPromise {
        return new Promise<AxiosResponse<Scenario[]>>((resolve) => {
            var removeDuplicateMongoScenarios = this.getDuplicateScenarios(scenarios);
            var convertedForMongoDB: Scenario[] = []; 
            removeDuplicateMongoScenarios.forEach(item => {
                var convertedData = convertFromVueToMongo(item);
                convertedForMongoDB.push(convertedData);
            });
            if (removeDuplicateMongoScenarios.length > 0) {
                this.removeMongoScenarios(convertedForMongoDB)
                    .then((res: AxiosResponse) => {
                        this.getMongoScenarios()
                            .then((responseMongo: AxiosResponse<Scenario[]>) => {
                                if (hasValue(responseMongo)) {
                                    return resolve(responseMongo);
                                }
                            });
                    }).catch((error: any) => resolve(error.response));
            }
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

    static setScenarioUsers(scenarioId: number, scenarioUsers: ScenarioUser[]): AxiosPromise {
        return axiosInstance.post(`/api/SetScenarioUsers/${scenarioId}`, scenarioUsers);
    }

    /**
     * Given two arrays of scenarios, returns an array of
     * scenarios that are in the first array but not the second.
     * @param arrayA The first array
     * @param arrayB The second array
     */
    private static getMissingScenarios(arrayA: Scenario[], arrayB: Scenario[]): Scenario[] {
        var missingScenarios: Scenario[] = [];
        arrayA.forEach(simulation => {
            if (!any(propEq('simulationId', simulation.simulationId), arrayB)) {
                missingScenarios.push(simulation);
            }
        });
        return missingScenarios;
    }

    private static getDuplicateScenarios(mongoScenarios: Scenario[]): Scenario[] {
        var duplicateScenarios: Scenario[] = [];
        // This will sort the array in ascending order of their simulation Ids
        let sortedScenarios = mongoScenarios.slice().sort((a, b) => a.simulationId - b.simulationId);
        for (var i = 0; i < sortedScenarios.length - 1; i++) {
            if (sortedScenarios[i + 1].simulationId == sortedScenarios[i].simulationId) {
                duplicateScenarios.push(sortedScenarios[i]);
            }
        }
        return duplicateScenarios;
    }

    /**
     * Removes several scenarios from mongoDB
     * @param scenarios Array of scenarios to remove
     */
    private static removeMongoScenarios(scenarios: Scenario[]): AxiosPromise {
        return new Promise<AxiosResponse<Scenario[]>>((resolve) => {
            scenarios.forEach(simulation => {
                nodejsAxiosInstance.delete(`api/DeleteMongoScenario/${(simulation as any)._id}`)
                    .then((res: AxiosResponse) => {
                        if (hasValue(res)) {
                            return resolve(res);
                        }
                    })
                    .catch((error: any) => {
                        return resolve(error.response);
                    });
            });
        });
    }
}
