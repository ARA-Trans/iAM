import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove MockAdapter code when api is implemented
import {mockAdapter, mockAxiosInstance} from '@/shared/utils/axios-mock-adapter-instance';
import or from 'ramda/es/or';

export default class CommittedProjectsService {
    /**
     * Saves committed projects files
     * @param files List of files to save
     */
    static saveCommittedProjectsFiles(files: File[], selectedScenarioId: string, networkId: string): AxiosPromise<any> {
        // Initialize the form data        
        let formData = new FormData();

        // Add the form data we need to submit        
        let file = files[0];
        formData.append('files[0]', file);
        formData.append('selectedScenarioId', selectedScenarioId);
        formData.append('networkId', networkId);
        
        // Make the request to the POST /single-file URL        
        return axiosInstance.post<any>('/api/SaveCommittedProjectsFiles', formData,
            {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });
    }
}
    

