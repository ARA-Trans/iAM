// This import loads the firebase namespace along with all its type information.
import * as firebase from 'firebase/app';

// These imports load individual services into the firebase namespace.
import 'firebase/database';

const firebaseApp = firebase.initializeApp({
    apiKey: 'AIzaSyAEw-O3t5gVEwh37pXYkBzN1pSiEVc7BYM',
    authDomain: 'bridgecareapp-ca3ed.firebaseapp.com',
    databaseURL: 'https://bridgecareapp-ca3ed.firebaseio.com',
    projectId: 'bridgecareapp-ca3ed',
    storageBucket: 'bridgecareapp-ca3ed.appspot.com',
    messagingSenderId: '826288103315'
});
export const db = firebaseApp.database();
export const statusReference = db.ref('scenarioStatus');
export const usersReference = db.ref('users');
export default firebaseApp;