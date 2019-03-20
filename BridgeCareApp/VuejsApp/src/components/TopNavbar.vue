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
        <v-content v-if="!loginFailed">
            <router-view></router-view>
        </v-content>
        <v-flex xs12>
            <AppSpinner />
        </v-flex>
    </nav>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import { db } from '@/firebase';
    import { append } from 'ramda';
    import axios from 'axios';

    import AppSpinner from '../shared/AppSpinner.vue';

    axios.defaults.baseURL = process.env.VUE_APP_URL;

    @Component({
        components: { AppSpinner }
    })
    export default class TopNavbar extends Vue {
        @State(state => state.security.loginFailed) loginFailed: boolean;
        @State(state => state.security.userName) userName: string;
        @State(state => state.busy.isBusy) isBusy: boolean;

        @Action('setIsBusy') setIsBusyAction: any;
        @Action('setLoginStatus') setLoginStatusAction: any;
        @Action('setUsername') setUsernameAction: any;

        routes: any[];
        filteredRoutes: any[];
        totalRoutes: any[];
        authenticationDetail: any;
        usersData = ['bridgecareAdministrator', 'testRole'] as Array<string>;
        userRoles = [] as Array<string>;

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

        async created() {
            this.setIsBusyAction({ isBusy: true });
            await axios.get('/auth/getuser', { withCredentials: true })
                .then(response => {
                    this.setLoginStatusAction({ status: false });
                    this.setUsernameAction({ userName: response.data });
                    this.authenticationDetail = response.data;
                }).then(() => {
                    db.ref('roles').once('value', (snapshot: any) => {
                        let data = snapshot.val();
                        for (let key in data) {
                            if (this.usersData.includes(data[key])) {
                                this.userRoles.push(key);
                            }
                        }
                        console.log(this.userRoles);
                        this.updateUI();
                        this.routes = this.totalRoutes;
                        this.$forceUpdate();
                        this.setIsBusyAction({ isBusy: false });
                    });
                }).catch(error => {
                    this.setLoginStatusAction({ status: true });
                    this.setIsBusyAction({ isBusy: false });
                    console.log(error);
                });
        }

        routing(routeName: string) {
            this.$router.push(routeName);
        }
        updateUI() {
            this.setUsernameAction({ userName: this.authenticationDetail });          
        }
    }
</script>