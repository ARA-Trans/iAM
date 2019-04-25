<template>
    <nav>
        <v-toolbar flat app>
            <v-toolbar-side-icon @click="drawer = !drawer" class="grey--text"
                                 v-if="!loginFailed"></v-toolbar-side-icon>
            <v-toolbar-title class="grey--text">
                <span class="font-weight-light">iAM</span>
                <span>BridgeCare</span>
            </v-toolbar-title>
            <v-spacer></v-spacer>
            <v-toolbar-title v-if="!loginFailed">
                <span class="font-weight-light">Hello </span>
                <span>{{userName}}</span>
            </v-toolbar-title>
        </v-toolbar>

        <v-navigation-drawer app class="grey lighten-3" v-if="!loginFailed" v-model="drawer">
            <v-list dense class="pt-0">
                <v-list-tile @click="routing('/Inventory/')">
                    <v-list-tile-action><v-icon>home</v-icon></v-list-tile-action>
                    <v-list-tile-title>Inventory</v-list-tile-title>
                </v-list-tile>
                <v-list-tile @click="routing('/Scenarios/')">
                    <v-list-tile-action><v-icon>list</v-icon></v-list-tile-action>
                    <v-list-tile-title>Scenarios</v-list-tile-title>
                </v-list-tile>
                <v-list-group prepend-icon="library_books">
                    <template slot="activator">
                        <v-list-tile>
                            <v-list-tile-title>Libraries</v-list-tile-title>
                        </v-list-tile>
                    </template>
                    <v-list-tile @click="routing('/InvestmentEditor/Library/')">
                        <v-list-tile-title>Investment Editor</v-list-tile-title>
                    </v-list-tile>
                    <v-list-tile @click="routing('/PerformanceEditor/')">
                        <v-list-tile-title>Performance Editor</v-list-tile-title>
                    </v-list-tile>
                    <v-list-tile @click="routing('/UnderConstruction/')">
                        <v-list-tile-title>Committed Projects Editor</v-list-tile-title>
                    </v-list-tile>
                    <v-list-tile @click="routing('/TreatmentEditor/')">
                        <v-list-tile-title>Treatment Editor</v-list-tile-title>
                    </v-list-tile>
                </v-list-group>
                <v-list-tile @click="routing('/UnderConstruction/')">
                    <v-list-tile-action><v-icon>lock</v-icon></v-list-tile-action>
                    <v-list-tile-title>Security</v-list-tile-title>
                </v-list-tile>
                <v-list-group prepend-icon="trending_up">
                    <template slot="activator">
                        <v-list-tile>
                            <v-list-tile-title>Reports</v-list-tile-title>
                        </v-list-tile>
                    </template>
                    <v-list-tile @click="routing('/DetailedReport/')">
                        <v-list-tile-title>Detailed report</v-list-tile-title>
                    </v-list-tile>
                </v-list-group>
            </v-list>
        </v-navigation-drawer>
        <v-content v-if="!loginFailed">
            <v-flex xs12>
                <v-breadcrumbs :items="navigation" divider=">">
                    <v-breadcrumbs-item slot="item"
                                        slot-scope="{ item }"
                                        exact
                                        :to="item.to">
                        {{ item.text }}
                    </v-breadcrumbs-item>
                </v-breadcrumbs>
            </v-flex>
            <router-view></router-view>
        </v-content>
        <v-flex xs12>
            <AppSpinner />
            <AppModalPopup :modalData="warning" @decision="onWarningModalDecision" />
        </v-flex>
    </nav>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Watch } from 'vue-property-decorator';
    import { Action, State } from 'vuex-class';

    import { Alert } from '@/shared/models/iAM/alert';
    import AppSpinner from '../shared/dialogs/AppSpinner.vue';
    import AppModalPopup from '../shared/dialogs/AppModalPopup.vue';
    import {Route} from '@/shared/models/iAM/route';

    @Component({
        components: { AppSpinner, AppModalPopup }
    })
    export default class TopNavbar extends Vue {
        @State(state => state.security.loginFailed) loginFailed: boolean;
        @State(state => state.security.userName) userName: string;
        @State(state => state.busy.isBusy) isBusy: boolean;
        @State(state => state.security.userRoles) userRoles: Array<string>;
        @State(state => state.breadcrumb.navigation) navigation: any[];

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('setLoginStatus') setLoginStatusAction: any;
        @Action('setUsername') setUsernameAction: any;
        @Action('getAuthentication') getAuthenticationAction: any;
        @Action('setNavigation') setNavigationAction: any;

        warning: Alert = { showModal: false, heading: '', message: '', choice: false };

        data() {
            return {
                drawer: true
            };
        }

        mounted() {
            this.setIsBusyAction({ isBusy: true });
            this.getAuthenticationAction().then((result: any) => {
                this.setIsBusyAction({ isBusy: false });
                console.log(this.userRoles);
                if (result.status == '401') {
                    this.warning.showModal = true;
                    this.warning.heading = 'Error';
                    this.warning.choice = false;
                    this.warning.message = result.data.message;
                }
                else {
                    this.$forceUpdate();
                }
            }).catch((error: any) => {
                this.setIsBusyAction({ isBusy: false });
                console.log(error);
            });
        }

        routing(routeName: string) {
            this.setNavigationAction([]);
            this.$router.push(routeName);
        }

        onWarningModalDecision(value: boolean) {
            this.warning.showModal = false;
        }
    }
</script>