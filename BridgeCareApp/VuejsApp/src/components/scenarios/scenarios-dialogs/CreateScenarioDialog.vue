<template>
    <v-dialog max-width="450px" persistent v-model="showDialog">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Scenario</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-text-field label="Name" outline v-model="createScenarioData.name"></v-text-field>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn :disabled="createScenarioData.name === ''" @click="onSubmit(true)"
                           class="ara-blue-bg white--text">
                        Save
                    </v-btn>
                    <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">Cancel</v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {emptyCreateScenarioData, ScenarioCreationData} from '@/shared/models/modals/scenario-creation-data';
    import {clone} from 'ramda';
    import {getUserName} from '../../../shared/utils/get-user-info';

    @Component
    export default class CreateScenarioDialog extends Vue {
        @Prop() showDialog: boolean;

        createScenarioData: ScenarioCreationData = clone(emptyCreateScenarioData);
        public: boolean = false;

        mounted() {
            this.createScenarioData.creator = getUserName();
            this.createScenarioData.owner = this.createScenarioData.creator;
        }

        @Watch('public')
        onSetPublic() {
            this.createScenarioData.owner = this.public ? undefined : getUserName();
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
            this.createScenarioData.creator = getUserName();
            this.createScenarioData.owner = this.createScenarioData.creator;
            this.public = false;
        }
    }
</script>
