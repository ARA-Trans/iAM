<template>
    <v-dialog v-model="showDialog" persistent max-width="450px">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Scenario</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-text-field label="Name" v-model="createScenarioData.name" outline></v-text-field>
                    <v-checkbox label="Make Public" v-model="public"/> 
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn class="ara-blue-bg white--text" @click="onSubmit(true)" :disabled="createScenarioData.name === ''">
                        Save
                    </v-btn>
                    <v-btn class="ara-orange-bg white--text" @click="onSubmit(false)">Cancel</v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {
        ScenarioCreationData, emptyCreateScenarioData
    } from '@/shared/models/modals/scenario-creation-data';
    import {clone} from 'ramda';
    import { UserInfo } from '../../../shared/models/iAM/authentication';
    import { parseLDAP } from '../../../shared/utils/parse-ldap';

    @Component
    export default class CreateScenarioDialog extends Vue {
        @Prop() showDialog: boolean;

        createScenarioData: ScenarioCreationData = clone(emptyCreateScenarioData);
        public: boolean = false;

        mounted() {
            const userInformation: UserInfo = JSON.parse(localStorage.getItem('UserInfo') as string) as UserInfo;
            this.createScenarioData.owner = parseLDAP(userInformation.sub);
        }

        @Watch('public')
        onSetPublic() {
            const userInformation: UserInfo = JSON.parse(localStorage.getItem('UserInfo') as string) as UserInfo;
            this.createScenarioData.owner = this.public ? undefined : parseLDAP(userInformation.sub);
        }

        /**
         * 'Submit' button has been clicked
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.createScenarioData);
            } else {
                this.$emit('submit', null);
            }

            this.createScenarioData = clone(emptyCreateScenarioData);
            const userInformation: UserInfo = JSON.parse(localStorage.getItem('UserInfo') as string) as UserInfo;
            this.createScenarioData.owner = parseLDAP(userInformation.sub);
            this.public = true;
        }
    }
</script>