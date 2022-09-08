import Vue from 'vue'
import VueRouter from 'vue-router'
import CrawlInfos from "@/components/CrawlInfos"
import CrawlDetails from "@/components/CrawlDetails"
import NotFound from "@/components/NotFound"

Vue.use(VueRouter)

const routes = [
    {
        path: '/',
        name: 'home',
        component: CrawlInfos
    },
    {
        path: '/details/:id',
        props: true,
        name: 'details',
        component: CrawlDetails
    },
    {
        path: '*',
        name: 'notFound',
        component: NotFound
    }
]

const router = new VueRouter({
    routes,
    mode: "history"
})

export default router
