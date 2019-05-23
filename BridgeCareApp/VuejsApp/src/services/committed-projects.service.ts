import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove mockAdapter code when api is implemented
import {mockAdapterInstance} from '@/shared/utils/mock-adapter-instance';

export default class CommittedProjectsService {
    /**
     * Saves committed projects files
     * @param files List of files to save
     */
    static saveCommittedProjectsFiles(files: File[]): AxiosPromise<any> {
        // TODO: remove mockAdapter code when api is implemented
        mockAdapterInstance.onPost(`${axiosInstance.defaults.baseURL}/api/SaveCommittedProjectsFiles`)
            .reply(201);
        return axiosInstance.post<any>('/api/SaveCommittedProjectsFiles', files);
    }
}
