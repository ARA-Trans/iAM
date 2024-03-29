<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center>
                    <v-card class="announcement-container">
                        <div class="bridgecare-logo-img-div">
                            <v-img :src="require('@/assets/images/logos/Banner-logo.jpg')"
                                   class="bridgecare-logo-img">
                            </v-img>
                        </div>
                        <div class="announcement" style="padding-bottom: 0px; margin-bottom: 0px" v-if="isAdmin">
                            <v-card style="margin-bottom: 0px; padding-bottom: 0px">
                                <v-card-title style="padding-top: 0px; padding-bottom: 0px">
                                    <v-icon @click="onStopEditing()" class="ara-orange"
                                            style="padding-right: 1em"
                                            title="Stop Editing"
                                            v-if="isEditingAnnouncement()">
                                        fas fa-times-circle
                                    </v-icon>
                                    <v-text-field class="announcement-title" label="Title" single-line
                                                  tabindex="1" v-model="newAnnouncementTitle"/>
                                    <v-spacer/>
                                    <v-btn @click="onSendAnnouncement" class="ara-blue" icon
                                           tabindex="3" title="Send Announcement">
                                        <v-icon>fas fa-paper-plane</v-icon>
                                    </v-btn>
                                </v-card-title>
                                <v-card-text style="padding-top: 0px; padding-bottom: 0px">{{formatDate(Date.now())}}
                                </v-card-text>
                                <v-textarea
                                        auto-grow
                                        class="announcement-content"
                                        dense label="Announcement Text (HTML tags can be used for detailed formatting.)" rows=1 single-line
                                        style="padding-left: 1em; padding-right: 1em; padding-top: 0.2em"
                                        tabindex="2"
                                        v-model="newAnnouncementContent"/>
                            </v-card>
                        </div>
                        <div style="display: flex; align-items: center; justify-content: center">
                            <v-btn @click="seeNewerAnnouncements()" class="ara-blue-bg white--text" round
                                   style="margin-top: 10px; margin-bottom: 0px"
                                   v-if="announcementListOffset > 0">
                                See Newer Announcements
                            </v-btn>
                        </div>
                        <div class="announcement" v-for="announcement in visibleAnnouncements()">
                            <v-card>
                                <v-card-title class="announcement-title">
                                    <v-icon @click="onStopEditing()" class="ara-orange"
                                            style="padding-right: 1em"
                                            title="Stop Editing"
                                            v-if="isEditingAnnouncement(announcement)">
                                        fas fa-times-circle
                                    </v-icon>
                                    {{announcement.title}}
                                    <v-spacer/>
                                    <v-btn @click="onSetEditAnnouncement(announcement)" class="ara-blue"
                                           icon
                                           title="Edit Announcement" v-if="isAdmin">
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                    <v-btn @click="onDeleteAnnouncement(announcement)" class="ara-orange"
                                           icon
                                           title="Delete Announcement" v-if="isAdmin">
                                        <v-icon>fas fa-trash</v-icon>
                                    </v-btn>
                                </v-card-title>
                                <v-card-text class="announcement-date">{{formatDate(announcement.creationDate)}}
                                </v-card-text>
                                <v-card-text class="announcement-content"
                                             v-html="announcement.content.replace(/(\r)*\n/g, '<br/>')"></v-card-text>
                            </v-card>
                        </div>
                        <div style="display: flex; align-items: center; justify-content: center;">
                            <v-btn @click="seeOlderAnnouncements()" class="ara-blue-bg white--text" round
                                   style="margin-top: 0px; margin-bottom: 10px"
                                   v-if="announcementListOffset < announcements.length - (isAdmin ? 2 : 3)">
                                See Older Announcements
                            </v-btn>
                        </div>
                    </v-card>
                </v-layout>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Action, State} from 'vuex-class';
    import {Component} from 'vue-property-decorator';
    import {Announcement, emptyAnnouncement} from '@/shared/models/iAM/announcement';
    import {clone} from 'ramda';
    import moment from 'moment';

    const ObjectID = require('bson-objectid');

    @Component
    export default class Home extends Vue {
        @State(state => state.announcement.announcements) announcements: Announcement[];
        @State(state => state.authentication.isAdmin) isAdmin: boolean;

        @Action('createAnnouncement') createAnnouncementAction: any;
        @Action('getAnnouncements') getAnnouncementsAction: any;
        @Action('updateAnnouncement') updateAnnouncementAction: any;
        @Action('deleteAnnouncement') deleteAnnouncementAction: any;

        announcementListOffset: number = 0;

        newAnnouncementTitle: string = '';
        newAnnouncementContent: string = '';

        editing?: Announcement = undefined;

        visibleAnnouncements() {
            return this.announcements.slice(this.announcementListOffset, this.announcementListOffset + (this.isAdmin ? 2 : 3));
        }

        formatDate(announcementDate: Date) {
            const date = new Date(announcementDate);
            return `${moment(date).format('dddd, MMMM Do')}`;
        }

        mounted() {
            this.getAnnouncementsAction();
        }

        onDeleteAnnouncement(announcement: Announcement) {
            this.deleteAnnouncementAction({deletedAnnouncement: announcement});
        }

        onSendAnnouncement() {
            if (this.editing === undefined) {
                this.onCreateAnnouncement();
            } else {
                this.onEditAnnouncement();
            }
        }

        onCreateAnnouncement() {
            var announcement = clone(emptyAnnouncement);
            announcement.title = this.newAnnouncementTitle;
            announcement.content = this.newAnnouncementContent;
            announcement.creationDate = Date.now();
            announcement.id = ObjectID.generate();
            this.createAnnouncementAction({createdAnnouncement: announcement});
            this.newAnnouncementContent = this.newAnnouncementTitle = '';
        }

        onEditAnnouncement() {
            if (this.editing === undefined) {
                return;
            }
            this.editing.title = this.newAnnouncementTitle;
            this.editing.content = this.newAnnouncementContent;
            this.updateAnnouncementAction({updatedAnnouncement: this.editing});
            this.newAnnouncementContent = this.newAnnouncementTitle = '';
            this.editing = undefined;
        }

        onSetEditAnnouncement(announcement: Announcement) {
            this.editing = announcement;
            this.newAnnouncementTitle = announcement.title;
            this.newAnnouncementContent = announcement.content;
        }

        onStopEditing() {
            this.editing = undefined;
            this.newAnnouncementTitle = this.newAnnouncementContent = '';
        }

        isEditingAnnouncement(announcement?: Announcement) {
            if (announcement === undefined) {
                return this.editing !== undefined;
            }
            return this.editing !== undefined && this.editing.id === announcement.id;
        }

        seeNewerAnnouncements() {
            // Admins see the announcement creation card, so they're shown one less announcement at a time to save space
            var decrement = this.isAdmin ? 2 : 3;
            if (this.announcementListOffset > decrement) {
                this.announcementListOffset -= decrement;
            } else {
                this.announcementListOffset = 0;
            }
        }

        seeOlderAnnouncements() {
            var increment = this.isAdmin ? 2 : 3;
            if (this.announcementListOffset < this.announcements.length - increment) {
                this.announcementListOffset += increment;
            } else {
                this.announcementListOffset = this.announcementListOffset - increment;
            }
        }
    }
</script>

<style>
    .announcement-container {
        width: 45%;
    }

    .bridgecare-logo-img-div {
        width: 100%;
    }

    .bridgecare-logo-img {
        width: 100%;
        border-bottom-style: solid;
        border-color: #008FCA;
    }

    .announcement {
        margin-left: 20px;
        margin-right: 20px;
        margin-top: 10px;
        margin-bottom: 10px;
    }

    .announcement-title {
        font-size: 2em;
        font-weight: bold;
        padding-bottom: 0px;
    }

    .announcement-date {
        font-size: 0.9em;
        padding-top: 0px;
        padding-bottom: 0px;
    }

    .announcement-content {
        font-size: 1.2em;
        padding-top: 0.75em;
        padding-bottom: 1em;
    }
</style>
