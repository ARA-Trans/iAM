<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center>
                    <v-card class="under-construction-card">
                        <div class="under-construction-img-div">
                            <v-img :src="require('@/assets/images/under-construction.png')"
                                   class="under-construction-img">
                            </v-img>
                        </div>
                        <v-card-title>
                            <h3>Page Not Yet Ready</h3>
                        </v-card-title>
                        <v-card-text>
                            <p>The page you are viewing has not yet been developed, but we are hard at work on it.</p>
                        </v-card-text>
                    </v-card>
                </v-layout>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component} from 'vue-property-decorator';

    @Component
    export default class UnderConstruction extends Vue {
    }
</script>

<style>
    .under-construction-card {
        width: 800px;
    }

    .under-construction-img-div {
        width: 100%;
    }

    .under-construction-img {
        width: 75%;
        margin: 0 auto;
    }
</style>
