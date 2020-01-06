import '@babel/polyfill';
import '@fortawesome/fontawesome-free/css/all.css';
import Vue from 'vue';
import 'vuetify/dist/vuetify.min.css';
import 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import Vuetify from 'vuetify';
import VueSocketio from 'vue-socket.io-extended';
import io from 'socket.io-client';
import App from './App.vue';
import router from './router';
import store from './store/root-store';
import './assets/css/main.css';
import 'izitoast/dist/css/iziToast.min.css';
import 'izitoast/dist/js/iziToast.min';
// @ts-ignore
import VueWorker from 'vue-worker';

Vue.use(Vuetify, {
    iconfont: 'fa'
});

const socketioClient = io(process.env.VUE_APP_NODE_URL, { path: process.env.VUE_APP_SOCKET_PATH, upgrade: true, transports: ['websocket'] });

Vue.use(VueSocketio, socketioClient, { store });

Vue.use(VueWorker);

Vue.config.productionTip = false;

new Vue({
    store,
    router,
    render: h => h(App)
}).$mount('#app');
