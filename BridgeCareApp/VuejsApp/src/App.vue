<template>
    <div id="app">
        <v-app class="grey lighten-4">
            <nav>
                <v-toolbar flat app>
                    <v-toolbar-side-icon v-if="loginFailed==false" class="grey--text" @click="drawer = !drawer"></v-toolbar-side-icon>
                    <v-toolbar-title class="grey--text">
                        <span class="font-weight-light">iAM</span>
                        <span>BridgeCare</span>
                    </v-toolbar-title>
                    <v-spacer></v-spacer>
                    <v-toolbar-title v-if="loginFailed==false">
                        <span class="font-weight-light">Hello </span>
                        <span>{{userName}}</span>
                    </v-toolbar-title>
                    <v-toolbar-items>
                        <v-btn v-if="loginFailed==true" flat color="grey" @click="login">
                            <v-icon left class="material-icons">account_circle</v-icon>
                            <span>Login or Sign-up</span>
                        </v-btn>
                        <v-btn flat color="grey" v-if="loginFailed==false" @click="logout">
                            <span>Sign Out</span>
                            <v-icon right>exit_to_app</v-icon>
                        </v-btn>
                    </v-toolbar-items>
                </v-toolbar>

                <v-navigation-drawer class="grey lighten-3" v-if="loginFailed==false" app v-model="drawer">
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

            </nav>
            <v-parallax dark style="height:720px" v-if="loginFailed==true"
                        src="https://cdn.vuetifyjs.com/images/backgrounds/vbanner.jpg">
                <v-layout align-center
                          column
                          justify-center>
                    <h1 class="display-2 font-weight-thin mb-3">Bridge Care application</h1>
                    <h4 class="subheading">Build your projects today!</h4>
                </v-layout>
            </v-parallax>
            <v-content v-if="loginFailed==false">
                <router-view></router-view>
            </v-content>
        </v-app>
    </div>
</template>
<script lang="ts">

    import Vue from 'vue';
    import { Component } from 'vue-property-decorator';
    import * as Msal from 'msal';

    //@ts-ignore
    import TopNavbar from './components/TopNavbar'

    @Component({
        components: { TopNavbar }
    })
    export default class AppComponent extends Vue {

        applicationConfig: { clientID: string; graphScopes: string[]; authority: string; };
        app: Msal.UserAgentApplication;
        logMessage(s: string) {
            console.log(s);
        }
        constructor() {
            super();
            let PROD_REDIRECT_URI = process.env.VUE_APP_CLIENT_APP_URL;
            let redirectUri = window.location.origin;
            if (window.location.hostname !== '127.0.0.1') {
                redirectUri = PROD_REDIRECT_URI;
            }

            if (process.env.VUE_APP_IS_PRODUCTION == false) {
                this.applicationConfig = {
                    clientID: '6b4fef89-350c-4dfe-8aa2-5ed5fddb9c5b',
                    graphScopes: ['https://aradomain.onmicrosoft.com/user/user_impersonation'],
                    authority: 'https://login.microsoftonline.com/tfp/aradomain.onmicrosoft.com/b2c_1_su-si-pol'
                };
            }
            else {
                this.applicationConfig = {
                    clientID: '6b4fef89-350c-4dfe-8aa2-5ed5fddb9c5b',
                    graphScopes: ['https://aradomain.onmicrosoft.com/user/user_impersonation'],
                    authority: 'https://login.microsoftonline.com/tfp/aradomain.onmicrosoft.com/b2c_1_su-si-pol'
                };
            }
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

        userName: string = '';
        loginFailed: boolean = true
        data() {
            return {
                items: [
                    { title: 'Home', icon: 'dashboard' },
                    { title: 'TODO', icon: 'question_answer' }
                ],
                drawer: true
            }
        }

        routing(routeName: string) {
            this.$router.push(routeName);
        }

        //created() {
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