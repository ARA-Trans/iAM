import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove MockAdapter code when api is implemented
import {mockAdapter, mockAxiosInstance} from '@/shared/utils/axios-mock-adapter-instance';

export default class CommittedProjectsService {
    /**
     * Saves committed projects files
     * @param files List of files to save
     */
    static saveCommittedProjectsFiles(files: File[]): AxiosPromise<any> {
        // TODO: remove MockAdapter code when api is implemented
        mockAdapter
            .onPost('/api/SaveCommittedProjectsFiles')
            .reply((config: any) => {
                return [201];
            });
        // TODO: replace mockAxiosInstance with axiosInstance
        return mockAxiosInstance.post<any>('/api/SaveCommittedProjectsFiles', files);
    }
}
