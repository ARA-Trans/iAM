<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center>
                    <v-card>
                        <v-card-title>
                            <h3>Authentication Failed</h3>
                        </v-card-title>
                        <v-btn @click="onAttemptAuthentication" class="v-btn theme--light ara-blue-bg white--text">
                            Try Again
                        </v-btn>
                    </v-card>
                </v-layout>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component} from 'vue-property-decorator';

    @Component
    export default class AuthenticationFailure extends Vue {
        onAttemptAuthentication() {
            this.$router.push('/AuthenticationStart/');
        }
    }
</script>
