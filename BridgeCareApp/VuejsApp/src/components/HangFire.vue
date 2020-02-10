<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center>
                    <v-card class="announcement-container">
                        
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
    export default class HangFire extends Vue { 
       mounted() {
            
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
        border-color:#008FCA;
    }

    .announcement {
        margin-left: 20px;
        margin-right: 20px;
        margin-top: 10px;
        margin-bottom: 10px;
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