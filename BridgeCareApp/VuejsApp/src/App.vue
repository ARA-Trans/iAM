<template>
    <v-app class="grey lighten-4">
        <TopNavbar/>
    </v-app>
</template>
<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import TopNavbar from './components/TopNavbar.vue';
    import iziToast from 'izitoast';
    import {hasValue} from '@/shared/utils/has-value-util';

    @Component({
        components: {TopNavbar}
    })
    export default class AppComponent extends Vue {
        @State(state => state.toastr.successMessage) successMessage: string;
        @State(state => state.toastr.errorMessage) errorMessage: string;
        @State(state => state.toastr.infoMessage) infoMessage: string;

        @Action('setSuccessMessage') setSuccessMessageAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('setInfoMessage') setInfoMessageAction: any;
        @Action('setIsBusy') setIsBusyAction: any;

        @Watch('successMessage')
        onSuccessMessageChanged() {
            if (hasValue(this.successMessage)) {
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
            if (hasValue(this.errorMessage)) {
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

        @Watch('infoMessage')
        onInfoMessageChanged() {
            if (hasValue(this.infoMessage)) {
                iziToast.info({
                    title: 'Info',
                    message: this.infoMessage,
                    position: 'topRight',
                    closeOnClick: true,
                    timeout: 3000
                });
                this.setInfoMessageAction({message: ''});
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