<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center>
                    <v-card>
                        <v-card-title>
                            <h3>Beginning Authentication</h3>
                        </v-card-title>
                        <v-card-text>
                            You should be redirected to the PennDOT login page shortly. If you are not redirected within
                            5 seconds, press the button below.
                        </v-card-text>
                        <v-btn @click="onRedirect" class="v-btn theme--light ara-blue-bg white--text">
                            Go to login page
                        </v-btn>
                    </v-card>
                </v-layout>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {State} from 'vuex-class';
    import oidcConfig from '@/oidc-config';

    @Component
    export default class AuthenticationStart extends Vue {
        @State(state => state.authentication.authenticated) authenticated: boolean;
        @State(state => state.authentication.hasRole) hasRole: boolean;
        @State(state => state.authentication.checkedForRole) checkedForRole: boolean;

        onRedirect() {
            if (!this.authenticated) {
                var href: string = `${oidcConfig.authorizationEndpoint}?response_type=code&scope=openid&scope=BAMS`;
                href += `&client_id=${oidcConfig.clientId}`;
                href += `&redirect_uri=${oidcConfig.redirectUri}`;

                // The 'state' query parameter that is sent to ESEC will be sent back to
                // the /Authentication page of the iam-deploy app.
                if (process.env.VUE_APP_IS_PRODUCTION !== 'true') {
                    href += '&state=localhost8080';
                }

                window.location.href = href;
            }
        }

        @Watch('checkedForRole')
        onCheckedRole() {
            if (this.hasRole) {
                this.$router.push('/News/');
            } else {
                this.$router.push('/NoRole/');
            }
        }

        mounted() {
            this.onRedirect();
        }
    }
</script>
