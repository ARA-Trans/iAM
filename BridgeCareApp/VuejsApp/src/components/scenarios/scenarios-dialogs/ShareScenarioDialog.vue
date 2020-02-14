<template>
    <v-dialog v-model="showDialog" persistent max-width="450px">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>Scenario Sharing</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-layout class="sharing-row">
                        <v-text-field label="User Name" 
                            class="sharing-username" style="margin-top: 0.5em"
                            v-model="newUser.username" />
                        <v-checkbox
                            class="sharing-checkbox" 
                            v-model="newUser.canModify" />
                        <div class="sharing-text">
                            {{newUser.canModify ? "Can Modify" : "Cannot Modify"}}
                        </div>
                        <v-btn icon title="Add User"
                            :disabled="newUser.username==''"
                            class="ara-blue sharing-button"
                            @click="onAddUser()">
                            <v-icon>fas fa-plus</v-icon>
                        </v-btn>
                    </v-layout>
                    <v-layout justify-center style="margin-top: 2em"
                        v-if="scenarioUsers.length !== 0">
                        <h4>Currently Shared With:</h4>
                    </v-layout>
                    <v-layout class="sharing-row"
                        v-for="user in scenarioUsers">
                        <div class="sharing-username">
                            {{user.username}}
                        </div>
                        <v-switch class="sharing-checkbox" 
                            v-model="user.canModify" />
                        <div class="sharing-text">
                            {{user.canModify ? "Can Modify" : "Cannot Modify"}}
                        </div>
                        <v-btn icon title="Remove User"
                            class="ara-orange sharing-button"
                            @click="onRemoveUser(user)">
                            <v-icon>fas fa-times</v-icon>
                        </v-btn>
                    </v-layout>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn class="ara-blue-bg white--text" @click="onSubmit(true)">
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
    import {clone, append, reject} from 'ramda';
    import { getUserName } from '../../../shared/utils/get-user-info';
import { ScenarioUser, Scenario, emptyScenario } from '../../../shared/models/iAM/scenario';

    @Component
    export default class ShareScenarioDialog extends Vue {
        @Prop() showDialog: boolean;
        @Prop() scenario: Scenario;
        scenarioUsers: ScenarioUser[] = [];
        newUser: ScenarioUser = {username: '', canModify: false};

        @Watch('scenario')
        onScenarioChanged() {
            this.scenarioUsers = clone(this.scenario.users);
        }

        onRemoveUser(removedUser: ScenarioUser) {
            this.scenarioUsers = reject(user => user.username == removedUser.username, this.scenarioUsers);
        }

        onAddUser() {
            this.scenarioUsers = append(clone(this.newUser), this.scenarioUsers);
            this.newUser = {username: '', canModify: false};
        }

        /**
         * 'Submit' button has been clicked
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', clone(this.scenarioUsers));
            } else {
                this.$emit('submit', null);
            }
            this.newUser = {username: '', canModify: false};
        }
    }
</script>

<style>
    .sharing-username {
        margin: auto;
        padding-left: 5%;
        padding-right: 5%;
        flex: 2;
        font-size: 1.2em;
        font-weight: bold;
    }

    .sharing-checkbox {
        margin: auto;
        margin-top: 1.3em;
        margin-left: 1em;
        flex: 0;
    }

    .sharing-row {
        white-space: nowrap;
        display: flex;
        flex-direction: row;
        justify-content: flex-start;
    }

    .sharing-text {
        margin: auto;
        margin-left: 0px;
        padding-left: 0px;
        margin-left: 0px;
        padding-right: 1.5em;
        flex: 2;
    }

    .sharing-button {
        margin: auto;
        margin-top: 1.3em;
        flex: 0.55;
        margin-right: 1.5em;
    }
</style>
