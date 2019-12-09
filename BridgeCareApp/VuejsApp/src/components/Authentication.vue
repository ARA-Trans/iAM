<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center>
                    <v-card>
                        <v-card-title>
                            <h3>Authenticating...</h3>
                        </v-card-title>
                    </v-card>
                </v-layout>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import AuthenticationService from '@/services/authentication.service';
    import {AxiosResponse} from 'axios';
    import {http2XX} from '@/shared/utils/http-utils';

    @Component
    export default class Authentication extends Vue {
        @State(state => state.authentication.authenticated) authenticated: boolean;

        @Action('setSuccessMessage') setSuccessMessageAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('getUserTokens') getUserTokensAction: any;
        @Action('getUserInfo') getUserInfoAction: any;
        @Action('getNetworks') getNetworksAction: any;
        @Action('getAttributes') getAttributesAction: any;

        mounted() {
            const code: string = this.$route.query.code as string;
            const state: string = this.$route.query.state as string;
            
            // The ESEC login will always redirect the browser to the iam-deploy site.
            // If the state is set, we know the authentication was started by a local client,
            // and so we should send the browser back to that client.
            if (state === 'localhost8080') {
                window.location.href = `http://localhost:8080/Authentication/?code=${code}`;
                return;
            }

            this.getUserTokensAction(code).then(() => {
                if (!this.authenticated) {
                    this.onAuthenticationFailure();
                } else {
                    this.getUserInfoAction();
                    this.onAuthenticationSuccess();
                }
            });
        }
        
        onAuthenticationSuccess() {
            this.setSuccessMessageAction({message: 'Authentication successful.'});
            this.$router.push('/Inventory/');
        }

        onAuthenticationFailure() {
            this.setErrorMessageAction({message: 'Authentication failed.'});
            this.$router.push('/AuthenticationFailure/');
        }
    }
</script>
