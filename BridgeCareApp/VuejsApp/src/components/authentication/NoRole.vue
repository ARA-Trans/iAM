<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center>
                    <v-card>
                        <v-card-title>
                            <h3>Service Not Available</h3>
                        </v-card-title>
                        <v-card-text>
                            You do not have access to the iAM BridgeCare App.
                        </v-card-text>
                        <!-- We don't have a link to redirect to for requesting access
                        <v-btn class="v-btn theme--light ara-blue-bg white--text" @click="onRequestAccess">
                            Request Access
                        </v-btn>
                        -->
                    </v-card>
                </v-layout>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';

    @Component
    export default class NoRole extends Vue {
        onRequestAccess() {
            console.error('onRequestAccess() has not been implemented!');
        }
    }
</script>
