<template>
    <v-layout column style="width: 66%; margin: auto">
        <v-flex xs12>
            <v-card class="test1">
                <div v-if="unassignedUsers.length > 0">
                    <v-card-title class="userCriteriaTableHeader">
                        Unassigned Users
                    </v-card-title>
                    <v-card-text style="justify-content: center; text-align: center; padding-top: 0px">
                        The following users have no access to any inventory items.
                    </v-card-text>
                    <v-divider style="margin: 0px"/>
                    <div v-for="user in unassignedUsers">
                    <v-layout row style="margin: auto; width: 75%;text-align: center">
                        <v-flex xs2 style="display: flex; flex-direction: column; justify-content: center; font-size: 1.2em; padding-top: 0px; padding-bottom: 0px">
                        {{user.username}}
                        </v-flex>
                        <v-flex xs4 style="padding: 0px">
                            <v-btn class="ara-blue-bg white--text" @click="onEditCriteria(user)"
                                title="Give the user limited access to the bridge inventory">
                                <v-icon size="1.5em" style="padding-right: 0.5em">fas fa-edit</v-icon>
                                Assign Criteria Filter
                            </v-btn>
                        </v-flex>
                        <v-flex xs4 style="padding: 0px; justify-content: center">
                            <v-btn class="ara-blue-bg white--text" @click="onGiveUnrestrictedAccess(user)"
                                title="Allow the user to access the full bridge inventory">
                                <v-icon size="1.5em" style="padding-right: 0.5em">fas fa-lock-open</v-icon>
                                Give Unrestricted Access
                            </v-btn>
                        </v-flex>
                    </v-layout>
                    <v-divider style="margin: 0px"/>
                    </div>
                </div>
                <v-card-title class="userCriteriaTableHeader">
                    Inventory Access Criteria
                </v-card-title>
                <div>
                    <v-data-table :headers="userCriteriaGridHeaders"
                                    :items="assignedUsers"
                                    :items-per-page="5"
                                    hide-actions
                                    class="elevation-1">
                        <template slot="items" slot-scope="props">
                            <td style="width: 10%; font-size: 1.2em; padding-top: 0.4em">{{ props.item.username }}</td>
                            <td style="width: 25%">
                                <v-layout align-center style="flex-wrap:nowrap; margin-left: 0px">
                                    <v-menu v-if="props.item.hasCriteria"
                                        bottom min-width="500px" min-height="500px">
                                        <template slot="activator">
                                            <input type="text" class="output" style="width: 25em" readonly
                                                   :value="props.item.criteria" />
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea rows="5" no-resize readonly full-width outline
                                                            :value="props.item.criteria">
                                                </v-textarea>
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <div v-if="!props.item.hasCriteria" style="font-size: 1.2em; font-weight: bold; padding-top: 0.4em; padding-right: 1em">
                                        Unrestricted Access
                                    </div>
                                    <v-btn v-if="props.item.hasCriteria" icon class="edit-icon" @click="onEditCriteria(props.item)"
                                        title="Edit Criteria">
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </v-layout>
                            </td>
                            <td>
                                <v-btn v-if="!props.item.hasCriteria" icon class="ara-blue" @click="onEditCriteria(props.item)"
                                    title="Restrict Access with Criteria Filter">
                                    <v-icon>fas fa-lock</v-icon>
                                </v-btn>
                                <v-btn v-if="props.item.hasCriteria" icon class="ara-blue" @click="onGiveUnrestrictedAccess(props.item)"
                                    title="Give Unrestricted Access">
                                    <v-icon>fas fa-lock-open</v-icon>
                                </v-btn>
                                <v-btn icon class="ara-orange" @click="onRevokeAccess(props.item)"
                                    title="Revoke Access">
                                    <v-icon>fas fa-times-circle</v-icon>
                                </v-btn>
                            </td>
                        </template>
                    </v-data-table>
                </div>
            </v-card>
        </v-flex>

        <CriteriaEditorDialog :dialogData="criteriaEditorDialogData" @submit="onSubmitCriteria" />
    </v-layout>
</template>

<script lang="ts">
        import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import moment from 'moment';
    import {emptyScenario, Scenario} from '@/shared/models/iAM/scenario';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
    import Alert from '@/shared/modals/Alert.vue';
    import CriteriaEditorDialog from '@/shared/modals/CriteriaEditorDialog.vue';
    import {
        CriteriaEditorDialogData,
        emptyCriteriaEditorDialogData
    } from '@/shared/models/modals/criteria-editor-dialog-data';
    import {find, propEq, clone, isNil, filter, insert} from 'ramda';
    import {UserCriteria} from '@/shared/models/iAM/user-criteria';

import { getUserName } from '../../shared/utils/get-user-info';

    @Component({
        components: {
           CriteriaEditorDialog, Alert}
    })
    export default class UserCriteriaEditor extends Vue {
        @State(state => state.userCriteria.allUserCriteria) allUserCriteria: UserCriteria[];

        @Action('getAllUserCriteria') getAllUserCriteriaAction : any;
        @Action('setUserCriteria') setUserCriteriaAction : any;

        alertData: AlertData = clone(emptyAlertData);
        alertBeforeDelete: AlertData = clone(emptyAlertData);
        alertBeforeRunRollup: AlertData = clone(emptyAlertData);
        showCreateScenarioDialog: boolean = false;
        userCriteriaGridHeaders: object[] = [
            {text: 'User', align: 'left', sortable: false, value: 'username'},
            {text: 'Criteria Filter', sortable: false, value: 'criteria'},
            {text: '', align: 'right', sortable: false, value: 'actions'}
        ];
        newUserGridHeaders: object[] = [
            {text: 'User', align: 'left', sortable: false, value: 'username'},
            {text: '', align: 'right', sortable: false, value: 'actions'}
        ];

        unassignedUsers: UserCriteria[] = [];
        assignedUsers: UserCriteria[] = [];
        criteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
        selectedUser: any = undefined;

        created() {
            this.getAllUserCriteriaAction();
        }

        @Watch('allUserCriteria')
        onUserCriteriaChanged() {
            this.unassignedUsers = this.allUserCriteria.filter((user: UserCriteria) => !user.hasAccess);
            this.assignedUsers = this.allUserCriteria.filter((user: UserCriteria) => user.hasAccess);
            this.$forceUpdate();
        }

        onEditCriteria(user: UserCriteria) {
            this.selectedUser = user;
            this.criteriaEditorDialogData = {
                showDialog: true,
                criteria: user.criteria === undefined ? '' : user.criteria
            };
        }

        onSubmitCriteria(criteria: string) {
            this.criteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                const userCriteria = {
                    ...this.selectedUser,
                    criteria,
                    hasAccess: true,
                    hasCriteria: true
                };
                this.setUserCriteriaAction({userCriteria});
            }

            this.selectedUser = undefined;
        }

        onRevokeAccess(targetUser: UserCriteria) {
            const userCriteria = {
                ...targetUser,
                criteria: undefined,
                hasAccess: false,
                hasCriteria: false
            };
            this.setUserCriteriaAction({userCriteria});
        }

        onGiveUnrestrictedAccess(targetUser: UserCriteria) {
            const userCriteria = {
                ...targetUser,
                criteria: undefined,
                hasAccess: true,
                hasCriteria: false
            };
            this.setUserCriteriaAction({userCriteria});
        }
    }
</script>

<style>
.userCriteriaTableHeader {
    justify-content: center;
    font-size: 1.5em;
    font-weight: bold;
}
</style>