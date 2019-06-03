import '@babel/polyfill';
import '@fortawesome/fontawesome-free/css/all.css';
import Vue from 'vue';
import 'vuetify/dist/vuetify.min.css';
import 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import Vuetify from 'vuetify';
import VueFire from 'vuefire';
import VueSocketio from 'vue-socket.io-extended';
import io from 'socket.io-client';
//@ts-ignore
//import VueSocketIO from 'vue-socket.io';

import App from './App.vue';
import router from './router';
import store from './store/root-store';
import './assets/css/main.css';
import 'izitoast/dist/css/iziToast.min.css';
import 'izitoast/dist/js/iziToast.min';

Vue.use(Vuetify, {
    iconfont: 'fa'
}, VueFire);

Vue.use(VueSocketio, io('http://localhost:4000'), { store });
//Vue.use(new VueSocketIO({
//    debug: true,
//    connection: 'http://localhost:4000',
//    vuex: {
//        store,
//        actionPrefix: 'SOCKET_',
//        mutationPrefix: 'SOCKET_'
//    }
//}));

Vue.config.productionTip = false;

//Vue.prototype.$socketIO = io('http://localhost:4000');

new Vue({
    store,
    router,
    render: h => h(App)
}).$mount('#app');
