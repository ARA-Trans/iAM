<template>
    <v-app class="grey lighten-4">
        <TopNavbar/>
    </v-app>
</template>
<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import {AxiosError, AxiosRequestConfig, AxiosResponse} from 'axios';
    import TopNavbar from './components/TopNavbar.vue';
    import iziToast from 'izitoast';
    import {axiosInstance} from '@/shared/utils/axios-instance';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {getErrorMessage, setContentTypeCharset} from "@/shared/utils/http-utils";
    import metaInfo from './meta';

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

        mounted() {
            // create a request handler
            const requestHandler = (request: AxiosRequestConfig) => {
                request.headers = setContentTypeCharset(request.headers);
                this.setIsBusyAction({isBusy: true});
                return request;
            };
            // set axios request interceptor to use request handler
            axiosInstance.interceptors.request.use(
                request => requestHandler(request)
            );
            // create a success & error handler
            const successHandler = (response: AxiosResponse) => {
                response.headers = setContentTypeCharset(response.headers);
                this.setIsBusyAction({isBusy: false});
                return response;
            };
            const errorHandler = (error: AxiosError) => {
                if (error.request) {
                    error.request.headers = setContentTypeCharset(error.request.headers);
                }
                if (error.response) {
                    error.response.headers = setContentTypeCharset(error.response.headers);
                }
                this.setIsBusyAction({isBusy: false});
                this.setErrorMessageAction({message: getErrorMessage(error)});
            };
            // set axios response handler to use success & error Handler
            axiosInstance.interceptors.response.use(
                response => successHandler(response),
                error => errorHandler(error)
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