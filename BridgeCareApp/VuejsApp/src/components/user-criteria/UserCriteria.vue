<template>
    <v-layout column style="width: 66%; margin: auto">
        <v-flex xs12>
            <v-card class="test1">
                <div v-if="unassignedUsers.length > 0">
                    <v-card-title class="userCriteriaTableHeader">
                        Unassigned Users
                    </v-card-title>
                    <v-card-text style="justify-content: center; text-align: center; padding-top: 0px">
                        The following users do not currently have any criteria filter assigned, and so they cannot access any inventory data.
                    </v-card-text>
                    <v-divider style="margin: 0px"/>
                    <div v-for="user in unassignedUsers">
                    <v-layout row style="margin: auto; width: 75%;text-align: center">
                        <v-flex xs2 style="display: flex; flex-direction: column; justify-content: center; font-size: 1.2em; padding-top: 0px; padding-bottom: 0px">
                        {{user.username}}
                        </v-flex>
                        <v-flex xs4 style="padding: 0px">
                            <v-btn class="ara-blue-bg white--text" @click="onEditCriteria(user)">Assign Criteria Filter</v-btn>
                        </v-flex>
                        <v-flex xs4 style="padding: 0px; justify-content: center">
                            <v-btn class="ara-blue-bg white--text" @click="onEditCriteria(user)">Give Unrestricted Access</v-btn>
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
                            <td>{{ props.item.username }}</td>
                            <td>
                                <v-layout align-center style="flex-wrap:nowrap; margin-left: 0px">
                                    <v-menu bottom min-width="500px" min-height="500px">
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
                                    <v-btn icon class="edit-icon" @click="onEditCriteria(props.item)">
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                    <v-btn icon class="ara-orange" @click="onRemoveCriteria(props.item)">
                                        <v-icon>fas fa-times-circle</v-icon>
                                    </v-btn>
                                </v-layout>
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

import { getUserName } from '../../shared/utils/get-user-info';

    @Component({
        components: {
           CriteriaEditorDialog, Alert}
    })
    export default class UserCriteria extends Vue {
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

        unassignedUsers: any[];
        assignedUsers: any[];
        users: any[] = [{username: 'Mr. Test2', criteria: 'some criteria'},{username: 'Mr. Test3', criteria: 'some criteria'},
        {username: 'Mr. Test', criteria: undefined},{username: 'Mr. Test5', criteria: undefined}];
        criteriaEditorDialogData: CriteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);
        selectedUser: any = undefined;

        created() {
            this.unassignedUsers = this.users.filter(propEq('criteria', undefined));
            this.assignedUsers = this.users.filter((user) => !propEq('criteria', undefined)(user));
        }

        onEditCriteria(user: any) {
            this.selectedUser = user;
            this.criteriaEditorDialogData = {
                showDialog: true,
                criteria: user.criteria
            };
        }

        onSubmitCriteria(criteria: string) {
            this.criteriaEditorDialogData = clone(emptyCriteriaEditorDialogData);

            if (!isNil(criteria)) {
                if (this.selectedUser.criteria === undefined) {
                    this.assignedUsers = insert(0, this.selectedUser, this.assignedUsers);
                    this.unassignedUsers = this.unassignedUsers.filter((user) => !propEq('username', this.selectedUser.username)(user));
                }
                this.selectedUser.criteria = criteria;
            }

            this.selectedUser = undefined;
        }

        onRemoveCriteria(targetUser: any) {
            targetUser.criteria = undefined;
            this.assignedUsers = this.assignedUsers.filter((user) => !propEq('username', targetUser.username)(user));
            this.unassignedUsers = insert(0, targetUser, this.unassignedUsers);
            this.$forceUpdate();
            console.log(this.assignedUsers);
            console.log(this.unassignedUsers);
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