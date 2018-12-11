<template>
    <div id="app" class="container-fluid">
        <div class="row">
            <TopNavbar />
            <v-toolbar>

                <v-spacer></v-spacer>
                <v-toolbar-title v-if="loginFailed==false" id="label">Hello {{userName}}</v-toolbar-title>
                <v-toolbar-items>
                    <v-btn id="auth" v-if="loginFailed==true" flat @click="login">Login or Sign-up</v-btn>
                    <v-btn flat v-if="loginFailed==false" @click="logout">Logout</v-btn>
                </v-toolbar-items>
            </v-toolbar>
        </div>
        <v-parallax dark style="height:720px" v-if="loginFailed==true"
                    src="https://cdn.vuetifyjs.com/images/backgrounds/vbanner.jpg">
            <v-layout align-center
                      column
                      justify-center>
                <h1 class="display-2 font-weight-thin mb-3">Bridge Care application</h1>
                <h4 class="subheading">Build your projects today!</h4>
            </v-layout>
        </v-parallax>
        <div class="row" v-if="loginFailed==false">
            <div class="col-sm-3">
                <v-navigation-drawer permanent>
                    <v-toolbar flat>
                        <v-list>
                            <v-list-tile>
                                <v-list-tile-title class="title">
                                    iAM
                                </v-list-tile-title>
                            </v-list-tile>
                        </v-list>
                    </v-toolbar>

                    <v-divider></v-divider>

                    <v-list dense class="pt-0">
                        <v-list-tile v-for="item in items"
                                     :key="item.title"
                                     @click="routing(item.title)">
                            <v-list-tile-action>
                                <v-icon>{{ item.icon }}</v-icon>
                            </v-list-tile-action>

                            <v-list-tile-content>
                                <v-list-tile-title>{{ item.title }}</v-list-tile-title>
                            </v-list-tile-content>
                        </v-list-tile>
                    </v-list>
                </v-navigation-drawer>
            </div>
            <div class="col-sm-9">
                <router-view></router-view>
            </div>
        </div>
    </div>
</template>
<script lang="ts">

    import Vue from 'vue';
    import { Component } from 'vue-property-decorator';
    import * as Msal from 'msal';

    //@ts-ignore
    import TopNavbar from './components/TopNavbar'
    //import AuthService from './services/auth.service';
    //import GraphService from './services/graph.service';
    //const GraphService = require('./services/graph.service')

    @Component({
        components: { TopNavbar }
    })
    export default class AppComponent extends Vue {

        applicationConfig: { clientID: string; graphScopes: string[]; authority: string;};
        app: Msal.UserAgentApplication;
        logMessage(s: string) {
            console.log(s);
        }
        constructor() {
            super();
            let PROD_REDIRECT_URI = 'http://localhost:1337/';
            let redirectUri = window.location.origin;
            if (window.location.hostname !== '127.0.0.1') {
                redirectUri = PROD_REDIRECT_URI;
            }

            this.applicationConfig = {
                clientID: '6b4fef89-350c-4dfe-8aa2-5ed5fddb9c5b',
                graphScopes: ['https://aradomain.onmicrosoft.com/user/user_impersonation'],
                authority: 'https://login.microsoftonline.com/tfp/aradomain.onmicrosoft.com/b2c_1_su-si-pol'
            };
            this.app = new Msal.UserAgentApplication(
                this.applicationConfig.clientID, this.applicationConfig.authority,
                (errorDesc, token, error, tokenType) => {
                    // callback for login redirect
                },
                {
                    redirectUri
                }
            );
        }
        authService: any;
        graphService: any;
        userName: string = '';
        user: any = null
        userInfo: any = null
        apiCallFailed: boolean = false
        loginFailed: boolean = true
        data() {
            return {
                items: [
                    { title: 'Home', icon: 'dashboard' },
                    { title: 'TODO', icon: 'question_answer' }
                ],
                right: null,
            }
        }

        routing(routeName: string) {
            this.$router.push(routeName);
        }

        //created() {
        //    this.authService = new AuthService();
        //    this.graphService = new GraphService();
        //}

        //callAPI() {
        //    this.apiCallFailed = false;
        //    this.authService.getToken().then(
        //        (token: any) => {
        //            this.graphService.getUserInfo(token).then(
        //                (data: any) => {
        //                    this.userInfo = data;
        //                },
        //                (error: any) => {
        //                    console.error(error);
        //                    this.apiCallFailed = true;
        //                }
        //            );
        //        },
        //        (error: any) => {
        //            console.error(error);
        //            this.apiCallFailed = true;
        //        }
        //    );
        //}


        login() {
            this.loginFailed = true;
            this.app.loginPopup(this.applicationConfig.graphScopes).then(
                idToken => {
                    this.app.acquireTokenSilent(this.applicationConfig.graphScopes).then(
                        accessToken => {
                            this.loginFailed = false;
                            this.updateUI();
                        }, error => {
                            this.app.acquireTokenPopup(this.applicationConfig.graphScopes).then(accessToken => {
                                this.updateUI();
                            }, error => {
                                this.logMessage("Error acquiring the popup:\n" + error);
                            });
                        })
                },
                (error) => {
                    this.logMessage("Error during login:\n" + error);
                }
            );
        }

        logout() {
            this.app.logout();
        }

        updateUI() {
            this.userName = this.app.getUser().name;
            this.logMessage("User '" + this.userName + "' logged-in");
            }
        }
</script>

<style>
    .main-nav li .glyphicon {
        margin-right: 10px;
    }

    /* Highlighting rules for nav menu items */
    .main-nav li a.router-link-active,
    .main-nav li a.router-link-active:hover,
    .main-nav li a.router-link-active:focus {
        background-color: #4189C7;
        color: white;
    }
    /* Keep the nav menu independent of scrolling and on top of other items */
    .main-nav {
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        z-index: 1;
    }

    @media (min-width: 768px) {
        /* On small screens, convert the nav menu to a vertical sidebar */
        .main-nav {
            height: 100%;
            width: calc(25% - 20px);
        }

            .main-nav .navbar {
                border-radius: 0px;
                border-width: 0px;
                height: 100%;
            }

            .main-nav .navbar-header {
                float: none;
            }

            .main-nav .navbar-collapse {
                border-top: 1px solid #444;
                padding: 0px;
            }

            .main-nav .navbar ul {
                float: none;
            }

            .main-nav .navbar li {
                float: none;
                font-size: 15px;
                margin: 6px;
            }

                .main-nav .navbar li a {
                    padding: 10px 16px;
                    border-radius: 4px;
                }

            .main-nav .navbar a {
                /* If a menu item's text is too long, truncate it */
                width: 100%;
                white-space: nowrap;
                overflow: hidden;
                text-overflow: ellipsis;
            }
    }
</style>