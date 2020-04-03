<template>
    <v-app class="paper-white-bg">
        <v-content>
            <v-navigation-drawer :disable-resize-watcher="true" app class="paper-white-bg" v-if="authenticatedWithRole"
                                 v-model="drawer">
                <v-list class="pt-0" dense>
                    <v-list-tile @click="drawer=false; onNavigate('/News/')">
                        <v-list-tile-action>
                            <v-icon class="ara-dark-gray">fas fa-newspaper</v-icon>
                        </v-list-tile-action>
                        <v-list-tile-title>Announcements</v-list-tile-title>
                    </v-list-tile>
                    <v-list-tile @click="onNavigate('/Inventory/')">
                        <v-list-tile-action>
                            <v-icon class="ara-dark-gray">fas fa-archive</v-icon>
                        </v-list-tile-action>
                        <v-list-tile-title>Inventory</v-list-tile-title>
                    </v-list-tile>
                    <v-list-tile @click="onNavigate('/Scenarios/')">
                        <v-list-tile-action>
                            <v-icon class="ara-dark-gray">fas fa-project-diagram</v-icon>
                        </v-list-tile-action>
                        <v-list-tile-title>Scenarios</v-list-tile-title>
                    </v-list-tile>
                    <v-list-group prepend-icon="fas fa-book">
                        <template slot="activator">
                            <v-list-tile>
                                <v-list-tile-title>Libraries</v-list-tile-title>
                            </v-list-tile>
                        </template>
                        <v-list-tile @click="onNavigate('/InvestmentEditor/Library/')">
                            <v-list-tile-title>Investment</v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile @click="onNavigate('/PerformanceEditor/Library/')">
                            <v-list-tile-title>Performance</v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile @click="onNavigate('/TreatmentEditor/Library/')">
                            <v-list-tile-title>Treatment</v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile @click="onNavigate('/PriorityEditor/Library/')">
                            <v-list-tile-title>Priority</v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile @click="onNavigate('/TargetEditor/Library/')">
                            <v-list-tile-title>Target</v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile @click="onNavigate('/DeficientEditor/Library/')">
                            <v-list-tile-title>Deficient</v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile @click="onNavigate('/RemainingLifeLimitEditor/Library/')" v-if="isAdmin">
                            <v-list-tile-title>Remaining Life Limit</v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile @click="onNavigate('/CashFlowEditor/Library/')">
                            <v-list-tile-title>Cash Flow</v-list-tile-title>
                        </v-list-tile>
                    </v-list-group>
                    <v-list-tile @click="onNavigate('/UserCriteria/')" v-if="isAdmin">
                        <v-list-tile-action>
                            <v-icon class="ara-dark-gray">fas fa-lock</v-icon>
                        </v-list-tile-action>
                        <v-list-tile-title>Security</v-list-tile-title>
                    </v-list-tile>
                </v-list>
            </v-navigation-drawer>
            <v-toolbar app class="ara-blue-pantone-289-bg">
                <v-toolbar-side-icon @click="drawer = !drawer"
                                     class="white--text" v-if="authenticatedWithRole && ($router.currentRoute.name !== 'News')"></v-toolbar-side-icon>
                <v-toolbar-title class="white--text"
                                 v-if="authenticatedWithRole && ($router.currentRoute.name === 'News')">
                    <v-btn @click="onNavigate('/Inventory/')" class="ara-blue-bg white--text" round>
                        <v-icon style="padding-right: 12px">fas fa-archive</v-icon>
                        Inventory Lookup
                    </v-btn>
                    <v-btn @click="onNavigate('/Scenarios/')" class="ara-blue-bg white--text" round>
                        <v-icon style="padding-right: 12px">fas fa-project-diagram</v-icon>
                        BridgeCare Analysis
                    </v-btn>
                    <v-btn @click="onNavigate('/UserCriteria/')" class="ara-blue-bg white--text" round v-if="isAdmin">
                        <v-icon style="padding-right: 12px">fas fa-lock</v-icon>
                        Security
                    </v-btn>
                </v-toolbar-title>
                <v-toolbar-title class="white--text" v-if="selectedScenarioName !== ''">
                    <span class="font-weight-light">Scenario: </span>
                    <span>{{selectedScenarioName}}</span>
                </v-toolbar-title>
                <v-spacer></v-spacer>
                <!-- <v-toolbar-title  class="white--text">
                    <v-btn round class="pink white--text" @click="onJobQueue()">
                        Job list
                        <v-icon right>star</v-icon>
                    </v-btn>
                </v-toolbar-title> -->
                <v-toolbar-title class="white--text" v-if="authenticated">
                    <span class="font-weight-light">Hello, </span>
                    <span>{{username}}</span>
                </v-toolbar-title>
                <v-toolbar-title class="white--text" v-if="!authenticated">
                    <v-btn @click="onNavigate('/AuthenticationStart/')" class="ara-blue-bg white--text" round>
                        Log In
                    </v-btn>
                </v-toolbar-title>
                <v-toolbar-title class="white--text" v-if="authenticated">
                    <v-btn @click="onLogout()" class="ara-blue-bg white--text" round>
                        Log Out
                    </v-btn>
                </v-toolbar-title>
            </v-toolbar>
            <v-container fluid v-bind="container">
                <router-view></router-view>
            </v-container>
            <v-footer app class="ara-blue-pantone-289-bg white--text" fixed>
                <v-spacer></v-spacer>
                <v-flex xs1>
                    <span class="font-weight-light">iAM </span>
                    <span>BridgeCare &copy; 2019</span>
                </v-flex>
                <v-spacer></v-spacer>
            </v-footer>
            <Spinner/>
            <Alert :dialog-data="alertDialogData" @submit="onAlertResult"/>
        </v-content>
    </v-app>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from 'vue-class-component';
    import {Watch} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import Spinner from './shared/modals/Spinner.vue';
    import iziToast from 'izitoast';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {AxiosError, AxiosRequestConfig, AxiosResponse} from 'axios';
    import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
    import {getErrorMessage, setAuthHeader, setContentTypeCharset} from '@/shared/utils/http-utils';
    import ReportsService from './services/reports.service';
    import Alert from '@/shared/modals/Alert.vue';
    import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
    import {clone} from 'ramda';

    @Component({
        components: {Alert, Spinner}
    })
    export default class AppComponent extends Vue {
        @State(state => state) state: any;
        @State(state => state.authentication.authenticated) authenticated: boolean;
        @State(state => state.authentication.hasRole) hasRole: boolean;
        @State(state => state.authentication.username) username: string;
        @State(state => state.authentication.isAdmin) isAdmin: boolean;
        @State(state => state.breadcrumb.navigation) navigation: any[];
        @State(state => state.toastr.successMessage) successMessage: string;
        @State(state => state.toastr.errorMessage) errorMessage: string;
        @State(state => state.toastr.infoMessage) infoMessage: string;
        @State(state => state.scenario.selectedScenarioName) stateSelectedScenarioName: string;
        @State(state => state.unsavedChangesFlag.hasUnsavedChanges) hasUnsavedChanges: boolean;

        @Action('refreshTokens') refreshTokensAction: any;
        @Action('checkBrowserTokens') checkBrowserTokensAction: any;
        @Action('logOut') logOutAction: any;
        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getNetworks') getNetworksAction: any;
        @Action('getAttributes') getAttributesAction: any;
        @Action('setSuccessMessage') setSuccessMessageAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('setInfoMessage') setInfoMessageAction: any;
        @Action('pollEvents') pollEventsAction: any;
        @Action('generatePollingSessionId') generatePollingSessionIdAction: any;
        @Action('getUserCriteria') getUserCriteriaAction: any;

        drawer: boolean = false;
        selectedScenarioName: string = '';
        alertDialogData: AlertData = clone(emptyAlertData);
        pushRouteUpdate: boolean = false;
        route: any = {};

        get container() {
            const container: any = {};

            if (this.$vuetify.breakpoint.xs) {
                container['grid-list-xs'] = true;
            }

            if (this.$vuetify.breakpoint.sm) {
                container['grid-list-sm'] = true;
            }

            if (this.$vuetify.breakpoint.md) {
                container['grid-list-md'] = true;
            }

            if (this.$vuetify.breakpoint.lg) {
                container['grid-list-lg'] = true;
            }

            if (this.$vuetify.breakpoint.xl) {
                container['grid-list-xl'] = true;
            }

            return container;
        }

        get authenticatedWithRole() {
            return this.authenticated && this.hasRole;
        }

        @Watch('successMessage')
        onSuccessMessageChanged() {
            if (hasValue(this.successMessage)) {
                iziToast.success({
                    title: 'Success',
                    message: this.successMessage,
                    position: 'topRight',
                    closeOnClick: true,
                    timeout: 3000
                });
                this.setSuccessMessageAction({message: ''});
            }
        }

        @Watch('errorMessage')
        onErrorMessageChanged() {
            if (hasValue(this.errorMessage)) {
                iziToast.error({
                    title: 'Error',
                    message: this.errorMessage,
                    position: 'topRight',
                    closeOnClick: true,
                    timeout: 3000
                });
                this.setErrorMessageAction({message: ''});
            }
        }

        @Watch('infoMessage')
        onInfoMessageChanged() {
            if (hasValue(this.infoMessage)) {
                iziToast.info({
                    title: 'Info',
                    message: this.infoMessage,
                    position: 'topRight',
                    closeOnClick: true,
                    timeout: 3000
                });
                this.setInfoMessageAction({message: ''});
            }
        }

        @Watch('stateSelectedScenarioName')
        onStateSelectedScenarioNameChanged() {
            this.selectedScenarioName = hasValue(this.stateSelectedScenarioName) ? this.stateSelectedScenarioName : '';
        }

        @Watch('authenticatedWithRole')
        onAuthenticationChange() {
            if (this.authenticated && this.hasRole) {
                this.onLogin();
            } else if (!this.authenticated) {
                this.onLogout();
            }
        }

        created() {
            // create a request handler
            const requestHandler = (request: AxiosRequestConfig) => {
                request.headers = setContentTypeCharset(request.headers);
                request.headers = setAuthHeader(request.headers);
                this.setIsBusyAction({isBusy: true});
                return request;
            };
            // set axios request interceptor to use request handler
            axiosInstance.interceptors.request.use(
                (request: any) => requestHandler(request)
            );
            // set nodejs axios request interceptor to use request handler
            nodejsAxiosInstance.interceptors.request.use(
                (request: any) => requestHandler(request)
            );
            // create a success & error handler
            const successHandler = (response: AxiosResponse) => {
                response.headers = setContentTypeCharset(response.headers);
                this.setIsBusyAction({isBusy: false});
                return response;
            };
            const errorHandler = (error: AxiosError) => {
                if (error.request) {
                    error.request.headers = setContentTypeCharset(error.request.headers);
                }
                if (error.response) {
                    error.response.headers = setContentTypeCharset(error.response.headers);
                }
                this.setIsBusyAction({isBusy: false});
                this.setErrorMessageAction({message: getErrorMessage(error)});
            };
            // set axios response handler to use success & error handlers
            axiosInstance.interceptors.response.use(
                (response: any) => successHandler(response),
                (error: any) => errorHandler(error)
            );
            // set nodejs axios response handler to user success & error handlers
            nodejsAxiosInstance.interceptors.response.use(
                (response: any) => successHandler(response),
                (error: any) => errorHandler(error)
            );

            this.$router.beforeEach((to: any, from: any, next: any) => {
                this.route = to;

                const showAlert: boolean = this.pushRouteUpdate === false && this.hasUnsavedChanges === true;
                if (showAlert) {
                    this.alertDialogData = {
                        showDialog: true,
                        heading: 'Unsaved Changes',
                        message: 'You have unsaved changes. Are you sure you wish to continue?',
                        choice: true
                    };
                } else {
                    this.pushRouteUpdate = false;
                    next();
                }
            });

            // Upon opening the page, and every 30 seconds, check if authentication data
            // has been changed by another tab or window
            this.checkBrowserTokensAction();
            window.setInterval(this.checkBrowserTokensAction, 30000);

            // Generate a polling session id, and begin polling once per 5 seconds
            this.generatePollingSessionIdAction();
            window.setInterval(this.pollEventsAction, 5000);
        }

        onAlertResult(submit: boolean) {
            this.alertDialogData = clone(emptyAlertData);

            if (submit) {
                this.pushRouteUpdate = true;
                this.onNavigate(this.route);
            }
        }

        /**
         * Sets up a recurring attempt at refreshing user tokens, and fetches network and attribute data
         */
        onLogin() {
            // Tokens expire after 30 minutes. They are refreshed after 29 minutes.
            this.refreshIntervalID = window.setInterval(this.refreshTokensAction, 29 * 60 * 1000);
            this.$forceUpdate();
            this.getNetworksAction();
            this.getAttributesAction();
            this.getUserCriteriaAction();
        }

        /**
         * Dispatches an action that will revoke all user tokens, prevents token refresh attempts,
         * and redirects users to the landing page
         */
        onLogout() {
            this.logOutAction().then(() => {
                window.clearInterval(this.refreshIntervalID);
                if (window.location.host.toLowerCase().indexOf('penndot.gov') === -1) {
                    /*
                     * In order to log out properly, the browser must visit the /iAM page of a penndot deployment, as iam-deploy.com cannot
                     * modify browser cookies for penndot.gov. So, the current host is sent as part of the query to the penndot site
                     * to allow the landing page to redirect the browser to the original host.
                     */
                    window.location.href = 'http://bamssyst.penndot.gov/iAM?host=' + encodeURI(window.location.host);
                } else {
                    this.onNavigate('/iAM/');
                }
            });
        }

        /**
         * Navigates a user to a page using the specified routeName
         * @param routeName The route name to use when navigating a user
         */
        onNavigate(route: any) {
            if (this.$router.currentRoute.path !== route.path) {
                this.$router.push(route);
            }
        }

        async onJobQueue() {
            await ReportsService.getJobList()
                .then((response: AxiosResponse<any>) => {
                    if (response == undefined) {
                        this.setErrorMessageAction({message: 'unauthorized access'});
                    } else {
                        window.open(process.env.VUE_APP_URL + '/hangfire/');
                    }
                });
        }
    }
</script>

<style>
    html {
        overflow: auto;
        overflow-x: hidden;
    }

    .v-list__group__header__prepend-icon .v-icon {
        color: #798899 !important;
    }

    .v-list__group__header__prepend-icon .primary--text .v-icon {
        color: #008FCA;
    }
</style>
