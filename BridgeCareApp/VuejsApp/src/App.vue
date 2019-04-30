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
    import iziToast from 'izitoast';

    @Component({
        components: {TopNavbar}
    })
    export default class AppComponent extends Vue {
        @State(state => state.toastr.successMessage) successMessage: string;
        @State(state => state.toastr.errorMessage) errorMessage: string;

        @Action('setSuccessMessage') setSuccessMessageAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('setIsBusy') setIsBusyAction: any;

        @Watch('successMessage')
        onSuccessMessageChanged() {
            if (!isEmpty(this.successMessage)) {
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
            if (!isEmpty(this.errorMessage)) {
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

        mounted() {
            const setErrorMessageAction = this.setErrorMessageAction;
            const setIsBusyAction = this.setIsBusyAction;
            axios.interceptors.response.use(
                function(response) {
                    return response;
                },
                function (error) {
                    setIsBusyAction({ isBusy: false });
                    if (error.response) {
                        setErrorMessageAction({message: error.response.data.message});                    }
                }
            );
        }
    }
</script>

<style>
    html {
        overflow: auto;
        overflow-x: hidden;
    }
</style>