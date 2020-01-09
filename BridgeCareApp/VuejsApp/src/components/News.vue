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
                        <div style="display: flex; align-items: center; justify-content: center;">
                            <v-btn round class="ara-blue-bg white--text" v-if="announcementListOffset > 0" @click="announcementListOffset = announcementListOffset > 3 ? announcementListOffset - 3 : 0">
                                See Newer Announcements
                            </v-btn>
                        </div>
                        <div class="announcement" v-for="announcement in visibleAnnouncements()">
                            <v-card>
                                <v-card-title class="announcement-title">{{announcement.title}}</v-card-title>
                                <v-card-text class="announcement-date">{{formatDate(announcement.creationDate)}}</v-card-text>
                                <v-card-text class="announcement-content">{{announcement.content}}</v-card-text>
                            </v-card>
                        </div>
                        <div style="display: flex; align-items: center; justify-content: center;">
                            <v-btn round class="ara-blue-bg white--text" v-if="announcementListOffset < announcements.length - 3" @click="announcementListOffset+=3">
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
    export default class News extends Vue { 
        @State(state => state.announcement.announcements) announcements: Announcement[];

        @Action('createAnnouncement') createAnnouncementAction: any;
        @Action('getAnnouncements') getAnnouncementsAction: any;
        @Action('updateAnnouncement') updateAnnouncementAction: any;
        @Action('deleteAnnouncement') deleteAnnouncementAction: any;

        announcementListOffset: number = 0;

        visibleAnnouncements() {
            return this.announcements.slice(this.announcementListOffset,3+this.announcementListOffset);
        }

        formatDate(announcementDate: Date) {
            const date = new Date(announcementDate);
            return `${moment(date).format('dddd, MMMM Do')}`;
        }

        created() {
            this.getAnnouncementsAction();
        }
    }
</script>

<style>
    .announcement-container {
        width: 50%;
    }

    .bridgecare-logo-img-div {
        width: 100%;
    }

    .bridgecare-logo-img {
        width: 100%;
        border-bottom-style: solid;
        border-color:#008FCA;
    }

    .announcement {
        margin: 20px;
    }

    .announcement-title {
        font-size: 2em;
        font-weight:bold;
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