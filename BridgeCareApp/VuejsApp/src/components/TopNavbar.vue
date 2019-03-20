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
            <v-toolbar-items>
                <v-btn @click="login" color="grey" flat v-if="loginFailed">
                    <v-icon left class="material-icons">account_circle</v-icon>
                    <span>Login or Sign-up</span>
                </v-btn>
                <v-btn @click="logout" color="grey" flat v-if="!loginFailed">
                    <span>Sign Out</span>
                    <v-icon right>exit_to_app</v-icon>
                </v-btn>
            </v-toolbar-items>
        </v-toolbar>

        <v-navigation-drawer app class="grey lighten-3" v-if="!loginFailed" v-model="drawer">
            <v-divider></v-divider>
            <v-list dense class="pt-0">
                <v-list-tile v-for="item in routes"
                             :key="item.title"
                             @click="routing(item.navigation)">
                    <v-list-tile-action>
                        <v-icon>{{item.icon}}</v-icon>
                    </v-list-tile-action>

                    <v-list-tile-content>
                        <v-list-tile-title>{{item.name}}</v-list-tile-title>
                        <v-list-tile-title hidden>{{item.navigation}}</v-list-tile-title>
                    </v-list-tile-content>
                </v-list-tile>
            </v-list>
        </v-navigation-drawer>
        <v-parallax dark style="height:720px" v-if="loginFailed"
                    src="https://cdn.vuetifyjs.com/images/backgrounds/vbanner.jpg">
            <v-layout align-center
                      column
                      justify-center>
                <h1 class="display-2 font-weight-thin mb-3">Bridge Care application</h1>
                <h4 class="subheading">Build your projects today!</h4>
            </v-layout>
        </v-parallax>
        <v-content v-if="!loginFailed">
            <router-view></router-view>
            <v-flex xs12>
                <AppSpinner />
            </v-flex>
        </v-content>
    </nav>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import * as Msal from 'msal';
    import { usersReference, db } from '@/firebase';
    import { ifElse, equals, append } from 'ramda';

    import AppSpinner from '../shared/AppSpinner.vue';

    @Component({
        components: { AppSpinner }
    })
    export default class TopNavbar extends Vue {
        @State(state => state.security.loginFailed) loginFailed: boolean;
        @State(state => state.security.userName) userName: string;
        @State(state => state.busy.isBusy) isBusy: boolean;
        @State(state => state.userAuthorization.isAdmin) isAdmin: boolean;

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('setIsAdmin') setIsAdminAction: any;
        @Action('setLoginStatus') setLoginStatusAction: any;
        @Action('setUsername') setUsernameAction: any;

        applicationConfig: {
            clientID: string; graphScopes: string[]; authority: string;
            authorityPR: string;
        };

        clientApp: Msal.UserAgentApplication;
        routes: any[];
        filteredRoutes: any[];
        totalRoutes: any[];

        signupSignInPolicy: string = 'https://login.microsoftonline.com/tfp/aratranstest.onmicrosoft.com/b2c_1_su-si-pol';
        passwordResetPolicy: string = 'https://login.microsoftonline.com/tfp/aratranstest.onmicrosoft.com/b2c_1_pr-pol';

        constructor() {
            super();
            let PROD_REDIRECT_URI = process.env.VUE_APP_CLIENT_APP_URL;
            let redirectUri = window.location.origin;
            if (window.location.hostname !== '127.0.0.1') {
                redirectUri = PROD_REDIRECT_URI;
            }

            if (process.env.VUE_APP_IS_PRODUCTION == 'false') {
                this.applicationConfig = {
                    clientID: '6abd6916-ce43-4cb1-9189-5749fc544e78',
                    graphScopes: ['https://aratranstest.onmicrosoft.com/user/user_impersonation'],
                    authority: this.signupSignInPolicy,
                    authorityPR: this.passwordResetPolicy
                };
            } else {
                this.applicationConfig = {
                    clientID: 'c4132c6f-fbb2-489c-bdf5-dda2ce009af8',
                    graphScopes: ['https://aratranstest.onmicrosoft.com/userStage/user_impersonation'],
                    authority: this.signupSignInPolicy,
                    authorityPR: this.passwordResetPolicy
                };
            }
            this.clientApp = new Msal.UserAgentApplication(
                this.applicationConfig.clientID, this.applicationConfig.authority,
                (errorDesc, token, error, tokenType) => {
                    console.log(token);
                },
                {
                    cacheLocation: 'localStorage',
                }
            );
        }

        data() {
            return {
                drawer: true
            };
        }

        beforeCreate() {
            this.filteredRoutes = [
                { navigation: 'Inventory', icon: 'home', name: 'Inventory' },
                { navigation: 'Scenarios', icon: 'assignment', name: 'Scenarios' },
                { navigation: 'InvestmentEditor', icon: 'pie_chart', name: 'Investment Editor'},
                { navigation: 'Criteria', icon: 'assignment', name: 'Criteria' }
            ];
            this.totalRoutes = append({ navigation: 'DetailedReport', icon: 'receipt', name: 'Detailed report' }, this.filteredRoutes);
            this.routes = [];
        }

        created() {
            let user = this.clientApp.getUser();
            this.setIsBusyAction({ isBusy: true });
            if (!user) {
                this.setLoginStatusAction({ status: true });
                this.setUsernameAction({ userName: '' });
                this.setIsBusyAction({ isBusy: false });
            } else {
                this.setLoginStatusAction({ status: false });
                this.setUsernameAction({ userName: user.name });
                this.updateUI();
            }
        }

        routing(routeName: string) {
            this.$router.push(routeName);
        }

        login() {
            this.setLoginStatusAction({status: true});
            this.clientApp.loginPopup(this.applicationConfig.graphScopes).then(
                idToken => {
                    this.setLoginStatusAction({status: false});
                    this.updateUI();
                },
                (error) => {
                    this.redirectOnErrors(error);
                }
            );
        }

        redirectOnErrors(error: any) {
            // [HACK] : Msal.js has not provided a feature to add multiple policies from Azure AD B2C.
            // To add Forget Password policy, when a user clicks on `forget password` link.
            // We are catching the error thrown by Azure AD B2C and repopulating the `clientApp` object with new policy
            // and calling the `login()` method again.
            if (error.indexOf('AADB2C90118') > -1) {
                this.clientApp = new Msal.UserAgentApplication(this.applicationConfig.clientID,
                    this.applicationConfig.authorityPR,
                    (errorDesc, token, error, tokenType) => {
                        // callback for login redirect
                    },
                    {cacheLocation: 'localStorage'});
                this.login();
            } else if (error.indexOf('AADB2C90091') > -1) {
                this.clientApp = new Msal.UserAgentApplication(this.applicationConfig.clientID,
                    this.applicationConfig.authority,
                    (errorDesc, token, error, tokenType) => {
                        // callback for login redirect
                    },
                    {cacheLocation: 'localStorage'});
                this.login();
            } else {
                console.log('Error during login:\n' + error);
            }
        }

        logout() {
            this.clientApp.logout();
        }

        updateUI() {
            this.setUsernameAction({ userName: this.clientApp.getUser().name });
            usersReference.once('value', (snapshot: any) => {
                //@ts-ignore
                const userId = this.clientApp.getUser().idToken.sub;
                if (snapshot.hasChild(userId)) {
                    this.setIsAdminAction({ isAdmin: snapshot.child(userId).val().roles.admin });
                    this.routes = ifElse(() => equals(this.isAdmin, true), () => this.totalRoutes, () => this.filteredRoutes)(this.routes);
                    this.$forceUpdate();
                }
                else {
                    console.log('user not found');
                    const newUser = {
                        //@ts-ignore
                        email: this.clientApp.getUser().idToken.emails[0],
                        roles: { owner: true, admin: false }
                    };
                    db.ref('users/' + userId).update(newUser);
                    this.setIsAdminAction({ isAdmin: false });
                    this.routes = this.filteredRoutes;
                    this.$forceUpdate();
                }
                this.setIsBusyAction({ isBusy: false });
            }).catch(error => {
                this.setIsBusyAction({ isBusy: false });
                });
            
        }
    }
</script>