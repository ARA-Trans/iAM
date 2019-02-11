<template>
        <v-app class="grey lighten-4">
            <TopNavbar/>           
        </v-app>
</template>
<script lang="ts">

    import Vue from 'vue'
    import { Component } from 'vue-property-decorator'

    //@ts-ignore
    import TopNavbar from './components/TopNavbar'

    @Component({
        components: { TopNavbar }
    })
    export default class AppComponent extends Vue {
    }
</script>