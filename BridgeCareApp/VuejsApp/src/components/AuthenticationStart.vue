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
                            You should be redirected to the PennDOT login page shortly. If you are not, press the button below.
                        </v-card-text>
                        <v-btn class="v-btn theme--light ara-blue-bg white--text" @click="onRedirect">
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
    import {State, Action} from 'vuex-class';
    import oidcConfig from '@/oidc-config';

    @Component
    export default class AuthenticationStart extends Vue {
        @State(state => state.authentication.loginFailed) loginFailed: boolean;

        onRedirect() {
            if (this.loginFailed) {
                window.location.href = `${oidcConfig.authorizationEndpoint}?response_type=code&scope=openid&scope=BAMS&client_id=${oidcConfig.clientId}&redirect_uri=${oidcConfig.redirectUri}`;
            } else {
                this.$router.push('/Inventory/');
            }
        }

        mounted() {
            this.onRedirect();
        }
    }
</script>
