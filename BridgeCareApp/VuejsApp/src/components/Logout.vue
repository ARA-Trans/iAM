<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center>
                    <v-card class="logged-out-card">
                        <div class="bridgecare-logo-img-div">
                            <v-img :src="require('@/assets/images/logos/Banner-logo.jpg')"
                                   class="bridgecare-logo-img">
                            </v-img>
                        </div>
                        <v-card-title style="justify-content: center">
                            <h3>Logged Out</h3>
                        </v-card-title>
                        <v-card-text style="justify-content: center; text-align: center">
                            <p>You have been logged out. Log back in to gain full access to the site.</p>
                        </v-card-text>
                    </v-card>
                </v-layout>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';

    @Component
    export default class Logout extends Vue {
        mounted() {
            /*
             * The /iAM/ pages of the penndot deployments fail to set the cookie until they have been refreshed.
             */
            if (!window.location.hash) {
                window.location.hash = 'refreshed';
                window.location.reload(true);
            }

            if (this.$route.query.host === undefined) {
                return;
            }
            /*
             * In order to log out properly, the browser must visit the landing page of a penndot deployment, as iam-deploy.com cannot
             * modify browser cookies for penndot.gov. So, if the browser was sent here from another host, redirect back to the landing
             * page of that host without the 'host' query string.
             */
            const host: string = this.$route.query.host as string;
            if (host !== window.location.host) {
                window.location.href = 'http://' + host + '/iAM';
            }
        }
    }
</script>

<style>
    .logged-out-card {
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
</style>
