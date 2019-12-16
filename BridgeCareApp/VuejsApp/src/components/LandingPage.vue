<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center>
                    <v-card class="logged-out-card">
                        <v-card-title>
                            <h3>Logged Out</h3>
                        </v-card-title>
                        <v-card-text>
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
    export default class LandingPage extends Vue {
        mounted() {
            if (this.$route.query.host == undefined) {
                return;
            }
            /*
             * In order to log out properly, the browser must visit a penndot deployment, as iam-deploy.com cannot
             * modify tokens for penndot.gov. So, if the browser was sent here from another host, redirect back to
             * that host without the 'host' query string.
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
        width: 800px;
    }
</style>