<template>
    <div id="app" class="container-fluid">
        <div class="row">
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

    @Component({
        components: {
            //MenuComponent: require('./views/navmenu.vue')
        }
    })
    export default class AppComponent extends Vue {
        data() {
            return {
                items: [
                    { title: 'Home', icon: 'dashboard' },
                    { title: 'TODO', icon: 'question_answer' }
                ],
                right: null
            }
        }

        routing(routeName: string) {
            this.$router.push(routeName);
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