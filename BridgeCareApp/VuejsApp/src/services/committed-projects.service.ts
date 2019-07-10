import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import { Scenario } from '@/shared/models/iAM/scenario';

export default class CommittedProjectsService {
    /**
     * Saves committed projects files
     * @param files List of files to save
     */
    static saveCommittedProjectsFiles(files: File[], selectedScenarioId: string, networkId: string): AxiosPromise {
        let formData = new FormData();

        // Add the form data      
        for (var i = 0; i < files.length; i++) {
            let file = files[i];
            formData.append('files[' + i + ']', file);
        }
        formData.append('selectedScenarioId', selectedScenarioId);
        formData.append('networkId', networkId);
        
        // Make the request to the API      
        return axiosInstance.post('/api/SaveCommittedProjectsFiles', formData,
            {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });
    }

    /**
     * Saves committed projects files
     * @param scenarioData
     */
    static ExportCommittedProjects(scenarioData: Scenario): AxiosPromise {
        return axiosInstance.post('/api/ExportCommittedProjects', scenarioData, { responseType: 'blob' });
    }
}
    

