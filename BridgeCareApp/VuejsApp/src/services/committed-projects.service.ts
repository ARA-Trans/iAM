import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove MockAdapter code when api is implemented
import MockAdapter from 'axios-mock-adapter';

export default class CommittedProjectsService {
    /**
     * Saves committed projects files
     * @param files List of files to save
     */
    static saveCommittedProjectsFiles(files: File[]): AxiosPromise<any> {
        // TODO: remove MockAdapter code when api is implemented
        const mockAdapterInstance = new MockAdapter(axiosInstance)
            .onPost(`${axiosInstance.defaults.baseURL}/api/SaveCommittedProjectsFiles`)
            .reply((config: any) => {
                mockAdapterInstance.restore();
                return [201];
            });
        return axiosInstance.post<any>('/api/SaveCommittedProjectsFiles', files);
    }
}
