<template>
    <nav>
        <v-toolbar flat app>
            <v-toolbar-side-icon @click="drawer = !drawer" class="grey--text" v-if="!loginFailed"></v-toolbar-side-icon>
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
                <v-list-tile @click="onNavigate('/Inventory/')">
                    <v-list-tile-action><v-icon>home</v-icon></v-list-tile-action>
                    <v-list-tile-title>Inventory</v-list-tile-title>
                </v-list-tile>
                <v-list-tile @click="onNavigate('/Scenarios/')">
                    <v-list-tile-action><v-icon>list</v-icon></v-list-tile-action>
                    <v-list-tile-title>Scenarios</v-list-tile-title>
                </v-list-tile>
                <v-list-group prepend-icon="library_books">
                    <template slot="activator">
                        <v-list-tile>
                            <v-list-tile-title>Libraries</v-list-tile-title>
                        </v-list-tile>
                    </template>
                    <v-list-tile @click="onNavigate('/InvestmentEditor/Library/')">
                        <v-list-tile-title>Investment Editor</v-list-tile-title>
                    </v-list-tile>
                    <v-list-tile @click="onNavigate('/PerformanceEditor/Library/')">
                        <v-list-tile-title>Performance Editor</v-list-tile-title>
                    </v-list-tile>
                    <v-list-tile @click="onNavigate('/TreatmentEditor/Library/')">
                        <v-list-tile-title>Treatment Editor</v-list-tile-title>
                    </v-list-tile>
                </v-list-group>
                <v-list-tile @click="onNavigate('/UnderConstruction/')">
                    <v-list-tile-action><v-icon>lock</v-icon></v-list-tile-action>
                    <v-list-tile-title>Security</v-list-tile-title>
                </v-list-tile>
            </v-list>
        </v-navigation-drawer>

        <v-content v-if="!loginFailed">
            <v-flex xs12>
                <v-breadcrumbs :items="navigation" divider=">">
                    <v-breadcrumbs-item slot="item" slot-scope="{item}" exact :to="item.to">
                        {{item.text}}
                        <div v-if="item.text.toLowerCase()=='analysis editor' || item.text.toLowerCase()=='investment editor' || item.text.toLowerCase()=='performance editor' || item.text.toLowerCase()=='treatment editor'|| item.text.toLowerCase()=='priorities, targets, & deficients'">: {{item.to.query.simulationName}}</div>
                    </v-breadcrumbs-item>
                </v-breadcrumbs>
            </v-flex>
            <router-view></router-view>
        </v-content>

        <v-flex xs12>
            <Spinner />
        </v-flex>
    </nav>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import Spinner from '../shared/modals/Spinner.vue';
    import { AxiosError, AxiosRequestConfig, AxiosResponse } from 'axios';
    import { getErrorMessage, setContentTypeCharset } from '@/shared/utils/http-utils';
    import { axiosInstance } from '@/shared/utils/axios-instance';

    @Component({
        components: {Spinner}
    })
    export default class TopNavbar extends Vue {
        @State(state => state.authentication.loginFailed) loginFailed: boolean;
        @State(state => state.authentication.userName) userName: string;
        @State(state => state.breadcrumb.navigation) navigation: any[];
        
        @Action('authenticateUser') authenticateUserAction: any;
        @Action('getNetworks') getNetworksAction: any;
        @Action('setNavigation') setNavigationAction: any;
        @Action('getAttributes') getAttributesAction: any;
        @Action('setIsBusy') setIsBusyAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;

        drawer: boolean = false;

        created() {
            // create a request handler
            const requestHandler = (request: AxiosRequestConfig) => {
                request.headers = setContentTypeCharset(request.headers);
                this.setIsBusyAction({isBusy: true});
                return request;
            };
            // set axios request interceptor to use request handler
            axiosInstance.interceptors.request.use(
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
            // set axios response handler to use success & error Handler
            axiosInstance.interceptors.response.use(
                (response: any) => successHandler(response),
                (error: any) => errorHandler(error)
            );
        }

        /**
         * Component has been mounted: Dispatches an action to authenticate the current user, then if user is authenticated
         * another another action is dispatched to get the networks
         */
        mounted() {
            this.authenticateUserAction().then(() => {
                this.$forceUpdate();
                this.getNetworksAction();
                this.getAttributesAction();
            });
        }

        /**
         * Navigates a user to a page using the specified routeName
         * @param routeName The route name to use when navigating a user
         */
        onNavigate(routeName: string) {
            this.setNavigationAction([]);
            this.$router.push(routeName);
        }
    }
</script>