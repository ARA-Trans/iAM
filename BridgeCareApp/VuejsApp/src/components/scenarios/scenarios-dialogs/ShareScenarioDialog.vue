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
                    <v-text-field label="Name" outline></v-text-field>
                    <v-checkbox label="Make Public" v-model="public"/> 
                    <div v-if="public">Warning: You will not be this scenario's owner. Your access to it may be restricted.</div>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn class="ara-blue-bg white--text" @click="onSubmit(true)" :disabled="false">
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
    import { getUserName } from '../../../shared/utils/get-user-info';
import { ScenarioUser } from '../../../shared/models/iAM/scenario';

    @Component
    export default class ShareScenarioDialog extends Vue {
        @Prop() showDialog: boolean;
        @Prop() scenarioUsers: ScenarioUser[];

        mounted() {
        }

        /**
         * 'Submit' button has been clicked
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', null);
            } else {
                this.$emit('submit', null);
            }

        }
    }
</script>