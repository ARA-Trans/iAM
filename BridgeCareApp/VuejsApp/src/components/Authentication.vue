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

    @Component
    export default class Authentication extends Vue {
        @State(state => state.authentication.loginFailed) loginFailed: boolean;

        @Action('setSuccessMessage') setSuccessMessageAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('getUserTokens') getUserTokensAction: any;
        @Action('getNetworks') getNetworksAction: any;
        @Action('getAttributes') getAttributesAction: any;

        @Watch('loginFailed')
        onLoginChange() {
            if (this.loginFailed) {
                this.onAuthenticationFailure();
            } else {
                this.setSuccessMessageAction({message: 'Authentication successful.'});
                this.$router.push('/Inventory/');
                this.$forceUpdate();
                this.getNetworksAction();
                this.getAttributesAction();
            }
        }

        mounted() {
            var code: string = this.$route.query.code as string;
            this.getUserTokensAction(code).then(() => {
                if (this.loginFailed) {
                    this.onAuthenticationFailure();
                }
            });
        }

        onAuthenticationFailure() {
            this.setErrorMessageAction({message: 'Authentication failed.'});
            this.$router.push('/AuthenticationFailure/');
        }
    }
</script>
