<template>
    <v-app class="grey lighten-4">
        <TopNavbar/>
    </v-app>
</template>
<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import axios from 'axios';
    import TopNavbar from './components/TopNavbar.vue';
    import {isEmpty} from 'ramda';

    @Component({
        components: {TopNavbar}
    })
    export default class AppComponent extends Vue {
        @State(state => state.toastr.successMessage) successMessage: string;
        @State(state => state.toastr.errorMessage) errorMessage: string;

        @Action('setSuccessMessage') setSuccessMessageAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;

        mounted() {
            axios.interceptors.response.use(
                function(response) {
                    return response;
                },
                function(error) {
                    if (error.response) {
                        this.setErrorMessageAction({message: error.response.data.message});                    }
                }
            );
        }

        @Watch('successMessage')
        onSuccessMessageChanged() {
            if (!isEmpty(this.successMessage)) {
                iZ
                this.setSuccessMessageAction({message: ''});
            }
        }

        @Watch('errorMessage')
        onErrorMessageChanged() {
            if (!isEmpty(this.errorMessage)) {
                this.setErrorMessageAction({message: ''});
            }
        }
    }
</script>

<style>
    html {
        overflow: auto;
        overflow-x: hidden;
    }
</style>