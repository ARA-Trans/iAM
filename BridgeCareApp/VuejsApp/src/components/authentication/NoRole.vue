<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center>
                    <v-card class="no-role-container">
                        <div class="bridgecare-logo-img-div">
                            <v-img :src="require('@/assets/images/logos/Banner-logo.jpg')"
                                   class="bridgecare-logo-img">
                            </v-img>
                        </div>
                        <v-card-title style="justify-content: center">
                            <h3>Service Not Available</h3>
                        </v-card-title>
                        <v-card-text style="justify-content: center; text-align: center">
                            <p>You do not have access to the iAM BridgeCare App.</p>
                            <p>You can request access at:</p>
                            <a href="mailto:">[Email for requesting access goes here]</a>
                        </v-card-text>
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

<style>
    .no-role-container {
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
</style>