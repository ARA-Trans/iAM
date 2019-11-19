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
    import {mapActions} from 'vuex';
    import {State, Action} from 'vuex-class';

    @Component
    export default class OidcCallback extends Vue {
        @State(state => state.toastr.successMessage) successMessage: string;
        @State(state => state.toastr.errorMessage) errorMessage: string;

        @Action('oidcSignInCallback') oidcSignInCallback : any;
        @Action('setSuccessMessage') setSuccessMessageAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;

        mounted() {
            this.oidcSignInCallback()
                .then((redirectPath: string) => {
                    this.setSuccessMessageAction({message: 'Authentication Successful.'});
                    this.$router.push('/Inventory/');
                })
                .catch((err: any) => {
                    console.error(err);
                    this.setErrorMessageAction({message: 'Authentication Failed.'});
                    this.$router.push('/AuthenticationFailure/');
                });
        }
    }
</script>
