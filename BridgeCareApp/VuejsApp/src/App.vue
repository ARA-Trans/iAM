<template>
    <v-app class="paper-white-bg">
        <v-content>
            <v-navigation-drawer app v-if="authenticatedWithRole" class="paper-white-bg" v-model="drawer" :disable-resize-watcher="true">
                <v-list dense class="pt-0">
                    <v-list-tile @click="drawer=false; onNavigate('/News/')">
                        <v-list-tile-action><v-icon class="ara-dark-gray">fas fa-newspaper</v-icon></v-list-tile-action>
                        <v-list-tile-title>Announcements</v-list-tile-title>
                    </v-list-tile>
                    <v-list-tile @click="onNavigate('/Inventory/')">
                        <v-list-tile-action><v-icon class="ara-dark-gray">fas fa-archive</v-icon></v-list-tile-action>
                        <v-list-tile-title>Inventory</v-list-tile-title>
                    </v-list-tile>
                    <v-list-tile @click="onNavigate('/Scenarios/')">
                        <v-list-tile-action><v-icon class="ara-dark-gray">fas fa-project-diagram</v-icon></v-list-tile-action>
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
                        <v-list-tile v-if="isAdmin" @click="onNavigate('/RemainingLifeLimitEditor/Library/')">
                            <v-list-tile-title>Remaining Life Limit</v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile @click="onNavigate('/CashFlowEditor/Library/')">
                            <v-list-tile-title>Cash Flow</v-list-tile-title>
                        </v-list-tile>
                    </v-list-group>
                    <v-list-tile @click="onNavigate('/UnderConstruction/')">
                        <v-list-tile-action><v-icon class="ara-dark-gray">fas fa-lock</v-icon></v-list-tile-action>
                        <v-list-tile-title>Security</v-list-tile-title>
                    </v-list-tile>
                </v-list>
            </v-navigation-drawer>
            <v-toolbar app class="ara-blue-pantone-289-bg">
                <v-toolbar-side-icon v-if="authenticatedWithRole && ($router.currentRoute.name !== 'News')" class="white--text" @click="drawer = !drawer"></v-toolbar-side-icon>
                <v-toolbar-title v-if="authenticatedWithRole && ($router.currentRoute.name === 'News')" class="white--text">
                    <v-btn round class="ara-blue-bg white--text" @click="onNavigate('/Inventory/')">
                        <v-icon style="padding-right: 12px">fas fa-archive</v-icon>
                        Inventory Lookup
                    </v-btn>
                    <v-btn round class="ara-blue-bg white--text" @click="onNavigate('/Scenarios/')">
                        <v-icon style="padding-right: 12px">fas fa-project-diagram</v-icon>
                        BridgeCare Analysis
                    </v-btn>
                </v-toolbar-title>
                <v-toolbar-title v-if="selectedScenarioName !== ''" class="white--text">
                    <span class="font-weight-light">Scenario: </span>
                    <span>{{selectedScenarioName}}</span>
                </v-toolbar-title>
                <v-spacer></v-spacer>
                <v-toolbar-title v-if="authenticated" class="white--text">
                    <span class="font-weight-light">Hello, </span>
                    <span>{{username}}</span>
                </v-toolbar-title>
                <v-toolbar-title v-if="!authenticated" class="white--text">
                    <v-btn round class="ara-blue-bg white--text" @click="onNavigate('/AuthenticationStart/')">
                        Log In
                    </v-btn>
                </v-toolbar-title>
                <v-toolbar-title v-if="authenticated" class="white--text">
                    <v-btn round class="ara-blue-bg white--text" @click="onLogout()">
                        Log Out
                    </v-btn>
                </v-toolbar-title>
            </v-toolbar>
            <v-container fluid grid-list-xl>
                <router-view></router-view>
            </v-container>
            <v-footer app fixed class="ara-blue-pantone-289-bg white--text">
                <v-spacer></v-spacer>
                <v-flex xs1>
                    <span class="font-weight-light">iAM </span>
                    <span>BridgeCare &copy; 2019</span>
                </v-flex>
                <v-spacer></v-spacer>
            </v-footer>
            <Spinner />
        </v-content>
    </v-app>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import Spinner from './shared/modals/Spinner.vue';
    import iziToast from 'izitoast';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {AxiosError, AxiosRequestConfig, AxiosResponse} from 'axios';
    import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
    import {getErrorMessage, setAuthHeader, setContentTypeCharset} from '@/shared/utils/http-utils';
    import {getAuthorizationHeader} from '@/shared/utils/authorization-header';

    @Component({
        components: {Spinner}
    })
    export default class AppComponent extends Vue {
        @State(state => state.authentication.authenticated) authenticated: boolean;
        @State(state => state.authentication.hasRole) hasRole: boolean;
        @State(state => state.authentication.username) username: string;
        @State(state => state.authentication.isAdmin) isAdmin: boolean;
        @State(state => state.breadcrumb.navigation) navigation: any[];
        @State(state => state.toastr.successMessage) successMessage: string;
        @State(state => state.toastr.errorMessage) errorMessage: string;
        @State(state => state.toastr.infoMessage) infoMessage: string;
        @State(state => state.scenario.selectedScenarioName) stateSelectedScenarioName: string;

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

        drawer: boolean = false;
        selectedScenarioName: string = '';

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

            // Upon opening the page, and every 30 seconds, check if authentication data
            // has been changed by another tab or window
            this.checkBrowserTokensAction();
            window.setInterval(this.checkBrowserTokensAction, 30000);

            // Generate a polling session id, and begin polling once per 5 seconds
            this.generatePollingSessionIdAction();
            window.setInterval(this.pollEventsAction, 5000);
        }

        get authenticatedWithRole() {
            return this.authenticated && this.hasRole;
        }

        @Watch('authenticatedWithRole')
        onAuthenticationChange() {
            if (this.authenticated && this.hasRole) {
                this.onLogin();
            } else if (!this.authenticated) {
                this.onLogout();
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
        onNavigate(routeName: string) {
            if(this.$router.currentRoute.path !== routeName) {
                this.$router.push(routeName);
            }
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
