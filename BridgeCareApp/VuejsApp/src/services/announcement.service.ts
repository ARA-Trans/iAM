import {AxiosPromise} from 'axios';
import {nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {Announcement} from '@/shared/models/iAM/announcement';
import {getAuthorizationHeader} from '@/shared/utils/authorization-header';

const modifyDataForMongoDB = (announcement: Announcement): any => {
    const announcementData: any = {
        ...announcement,
        _id: announcement.id
    };
    delete announcementData.id;
    return announcementData;
};

export default class AnnouncementService {
    /**
     * Gets Announcements
     */
    static getAnnouncements(): AxiosPromise {
        return nodejsAxiosInstance.get('/api/GetAnnouncements', {headers: getAuthorizationHeader()});
    }

    /**
     * Creates an Announcement
     * @param createdAnnouncement The Announcement create data
     */
    static createAnnouncement(createdAnnouncement: Announcement): AxiosPromise {
        return nodejsAxiosInstance.post('/api/CreateAnnouncement', modifyDataForMongoDB(createdAnnouncement), {headers: getAuthorizationHeader()});
    }

    /**
     * Updates an Announcement
     * @param updatedAnnouncement The Announcement update data
     */
    static updateAnnouncement(updatedAnnouncement: Announcement): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdateAnnouncement', modifyDataForMongoDB(updatedAnnouncement), {headers: getAuthorizationHeader()});
    }

    /**
     * Deletes an Announcement
     * @param deletedAnnouncement The Announcement to delete
     */
    static deleteAnnouncement(deletedAnnouncement: Announcement): AxiosPromise {
        return nodejsAxiosInstance.delete(`/api/DeleteAnnouncement/${deletedAnnouncement.id}`, {headers: getAuthorizationHeader()});
    }
}