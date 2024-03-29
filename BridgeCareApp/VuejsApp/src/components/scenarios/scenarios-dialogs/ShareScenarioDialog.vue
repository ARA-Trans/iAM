<template>
    <v-dialog max-width="450px" persistent v-model="showDialog">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>Scenario Sharing</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-layout class="sharing-row">
                        <v-text-field class="sharing-username"
                                      label="User Name" style="margin-top: 0.5em"
                                      v-model="newUser.username"/>
                        <v-checkbox
                                class="sharing-checkbox"
                                v-model="newUser.canModify"/>
                        <div class="sharing-text">
                            {{newUser.canModify ? "Can Modify" : "Cannot Modify"}}
                        </div>
                        <v-btn :disabled="newUser.username==''"
                               @click="onAddUser()"
                               class="ara-blue-bg white--text sharing-button"
                               title="Add User">
                            Share
                        </v-btn>
                    </v-layout>
                    <v-layout>
                        <v-btn :disabled="public"
                               @click="onSetPublic()"
                               class="ara-blue-bg white--text"
                               style="margin: auto"
                               title="Share With Everyone">
                            Share with everyone
                        </v-btn>
                    </v-layout>
                    <v-layout justify-center style="margin-top: 2em"
                              v-if="scenarioUsers.length !== 0">
                        <h4>Currently Shared With:</h4>
                    </v-layout>
                    <v-layout class="sharing-row"
                              v-for="user in scenarioUsers" :key="user.id">
                        <div class="sharing-username">
                            {{user.username === null ? "[All Users]" : user.username}}
                        </div>
                        <v-switch class="sharing-checkbox"
                                  v-model="user.canModify"/>
                        <div class="sharing-text">
                            {{user.canModify ? "Can Modify" : "Cannot Modify"}}
                        </div>
                        <v-btn @click="onRemoveUser(user)" class="ara-orange sharing-button"
                               icon
                               title="Remove User">
                            <v-icon>fas fa-times</v-icon>
                        </v-btn>
                    </v-layout>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn @click="onSubmit(true)" class="ara-blue-bg white--text">
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
    import {any, append, clone, reject} from 'ramda';
    import {Scenario, ScenarioUser} from '../../../shared/models/iAM/scenario';

    @Component
    export default class ShareScenarioDialog extends Vue {
        @Prop() showDialog: boolean;
        @Prop() scenario: Scenario;
        scenarioUsers: ScenarioUser[] = [];
        newUser: ScenarioUser = {username: '', canModify: false};
        public: boolean = false;

        @Watch('scenario')
        onScenarioChanged() {
            this.scenarioUsers = clone(this.scenario.users);
            this.public = any(user => user.username === null, this.scenarioUsers);
        }

        onRemoveUser(removedUser: ScenarioUser) {
            if (removedUser.username === null) {
                this.public = false;
            }
            this.scenarioUsers = reject(user => user.username == removedUser.username, this.scenarioUsers);
        }

        onAddUser() {
            this.scenarioUsers = append(clone(this.newUser), this.scenarioUsers);
            this.newUser = {username: '', canModify: false};
        }

        onSetPublic() {
            this.scenarioUsers = append({username: null, canModify: false}, this.scenarioUsers);
            this.public = true;
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
