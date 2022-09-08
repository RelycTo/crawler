import Vue from 'vue'
import Vuex from 'vuex'
import common from './common'
import infos from './infos'
import details from './details'

Vue.use(Vuex)

export default new Vuex.Store({
    modules: {
        common,
        infos,
        details
    }
})