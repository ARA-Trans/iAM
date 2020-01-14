import {AxiosPromise} from 'axios';
import {nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {Announcement} from '@/shared/models/iAM/announcement';
import {convertFromVueToMongo} from '@/shared/utils/mongo-model-conversion-utils';

export default class AnnouncementService {
    /**
     * Gets Announcements
     */
    static getAnnouncements(): AxiosPromise {
        return nodejsAxiosInstance.get('/api/GetAnnouncements');
    }

    /**
     * Creates an Announcement
     * @param createdAnnouncement The Announcement create data
     */
    static createAnnouncement(createdAnnouncement: Announcement): AxiosPromise {
        return nodejsAxiosInstance.post('/api/CreateAnnouncement', convertFromVueToMongo(createdAnnouncement));
    }

    /**
     * Updates an Announcement
     * @param updatedAnnouncement The Announcement update data
     */
    static updateAnnouncement(updatedAnnouncement: Announcement): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdateAnnouncement', convertFromVueToMongo(updatedAnnouncement));
    }

    /**
     * Deletes an Announcement
     * @param deletedAnnouncement The Announcement to delete
     */
    static deleteAnnouncement(deletedAnnouncement: Announcement): AxiosPromise {
        return nodejsAxiosInstance.delete(`/api/DeleteAnnouncement/${deletedAnnouncement.id}`);
    }
}