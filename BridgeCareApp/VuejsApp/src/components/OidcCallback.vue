<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center>
                    <v-card>
                        <v-card-title>
                            <h3>Authenticating...</h3>
                        </v-card-title>
                    </v-card>
                </v-layout>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
import {mapActions} from 'vuex';

export default {
    name: 'OidcCallback',
    methods: {
        ...mapActions([
            'oidcSignInCallback'
        ])
    },
    mounted() {
        this.oidcSignInCallback()
            .then((redirectPath) => {
                this.$router.push(redirectPath);
            })
            .catch((err) => {
                console.error(err);
                //this.$router.push('/AuthenticationFailure/');
            });
    }
};
</script>