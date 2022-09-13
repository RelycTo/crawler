<template>
    <v-app id="app">
        <v-navigation-drawer app
                             temporary
                             width="60"
                             dark
                             color="blue-grey"
                             v-model="drawer">
            <v-list>
                <v-list-item class="px-2">
                    <v-list-item-avatar>
                        <v-icon>home</v-icon>
                    </v-list-item-avatar>
                </v-list-item>
                <v-divider></v-divider>
                <v-list-item link
                             tag="a"
                             v-for="link of links"
                             :key="link.title"
                             :to="link.url">
                    <v-list-item-content>
                        <v-list-item-title class="text-h6">
                            <v-icon medium>{{ link.icon }}</v-icon>
                        </v-list-item-title>
                    </v-list-item-content>
                </v-list-item>
            </v-list>
        </v-navigation-drawer>

        <v-app-bar app dark color="blue-grey">
            <v-app-bar-nav-icon @click="drawer = !drawer"></v-app-bar-nav-icon>
            <v-toolbar-title tag="h1">
                <router-link :to="'/'" v-slot="{ navigate }" class="pointer" custom>
                    <span @click="navigate" @keypress.enter="navigate" role="link">Crawler</span>
                </router-link>
            </v-toolbar-title>
        </v-app-bar>

        <v-main>
            <router-view></router-view>
        </v-main>
        <template v-if="error">
            <v-snackbar :timeout="5000"
                        :multi-line="true"
                        color="error"
                        @input="closeError"
                        :value="true">
                {{ error }}
                <v-btn dark @click.native="closeError">Close</v-btn>
            </v-snackbar>
        </template>
    </v-app>
</template>

<script>
    export default {
        data() {
            return {
                drawer: false,
                links: [{
                    title: 'Crawl tests info',
                    icon: 'poll',
                    url: '/'
                }]
            }
        },
        computed: {
            error() {
                return this.$store.getters.error
            }
        },
        created() {
            this.$store.dispatch('fetchInfos')
        },
        methods: {
            closeError() {
                this.$store.dispatch('clearError')
            },
        }
    }
</script>

<style scoped>
    .pointer {
        cursor: pointer;
    }

    main {
        background-color: #CFD8DC;
    }
</style>