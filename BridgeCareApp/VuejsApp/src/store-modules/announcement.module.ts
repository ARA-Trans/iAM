import {Announcement} from '@/shared/models/iAM/announcement';
import {any, append, clone, findIndex, propEq, reject, update} from 'ramda';
import AnnouncementService from '@/services/announcement.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {convertFromMongoToVue} from '@/shared/utils/mongo-model-conversion-utils';

const state = {
    announcements: [] as Announcement[],
    packageVersion: process.env.PACKAGE_VERSION || '0'
};

const mutations = {
    announcementsMutator(state: any, announcements: Announcement[]) {
        state.announcements = clone(announcements);
    },
    createdAnnouncementMutator(state: any, createdAnnouncement: Announcement) {
        if (!any(propEq('id', createdAnnouncement.id), state.announcements)) {
            state.announcements = append(createdAnnouncement, state.announcements);
        }
    },
    updatedAnnouncementMutator(state: any, updatedAnnouncement: Announcement) {
        if (any(propEq('id', updatedAnnouncement.id), state.announcements)) {
            state.announcements = update(
                findIndex(propEq('id', updatedAnnouncement.id), state.announcements),
                updatedAnnouncement,
                state.announcements
            );
        }
    },
    deletedAnnouncementMutator(state: any, deletedAnnouncement: Announcement) {
        state.announcements = reject(propEq('id', deletedAnnouncement.id), state.announcements);
    },
    sortAnnouncementsMutator(state: any) {
        state.announcements.sort((a: Announcement, b: Announcement) => b.creationDate - a.creationDate);
    }
};

const actions = {
    async getAnnouncements({commit}: any) {
        await AnnouncementService.getAnnouncements().then((response: AxiosResponse<any[]>) => {
            if (hasValue(response, 'data')) {
                const announcements: Announcement[] = response.data
                    .map((data: any) => convertFromMongoToVue(data));
                commit('announcementsMutator', announcements);
                commit('sortAnnouncementsMutator');
            }
        });
    },
    async createAnnouncement({dispatch, commit}: any, payload: any) {
        await AnnouncementService.createAnnouncement(payload.createdAnnouncement)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const createdAnnouncement: Announcement = convertFromMongoToVue(response.data);
                    commit('createdAnnouncementMutator', createdAnnouncement);
                    commit('sortAnnouncementsMutator');
                    dispatch('setSuccessMessage', {message: 'Successfully created announcement'});
                }
            });
    },
    async updateAnnouncement({dispatch, commit}: any, payload: any) {
        await AnnouncementService.updateAnnouncement(payload.updatedAnnouncement)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const updatedAnnouncement: Announcement = convertFromMongoToVue(response.data);
                    commit('updatedAnnouncementMutator', updatedAnnouncement);
                    commit('sortAnnouncementsMutator');
                    dispatch('setSuccessMessage', {message: 'Successfully updated announcement'});
                }
            });
    },
    async deleteAnnouncement({dispatch, commit}: any, payload: any) {
        await AnnouncementService.deleteAnnouncement(payload.deletedAnnouncement)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const deletedAnnouncement: Announcement = convertFromMongoToVue(response.data);
                    commit('deletedAnnouncementMutator', deletedAnnouncement);
                    dispatch('setSuccessMessage', {message: 'Successfully deleted announcement'});
                }
            });
    },
    async socket_announcement({dispatch, state, commit}: any, payload: any) {
        if (hasValue(payload, 'operationType') && hasValue(payload, 'fullDocument')) {
            const announcement: Announcement = convertFromMongoToVue(payload.fullDocument);
            switch (payload.operationType) {
                case 'update':
                case 'replace':
                    commit('updatedAnnouncementMutator', announcement);
                    commit('sortAnnouncementsMutator');
                    break;
                case 'insert':
                    if (!any(propEq('id', announcement.id), state.announcements)) {
                        commit('createdAnnouncementMutator', announcement);
                        commit('sortAnnouncementsMutator');
                        dispatch('setInfoMessage',
                            {message: `New Announcement: ${announcement.title}`}
                        );
                    }
            }
        }
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
