import api from '../api'
import dateFormatter from './dateFormatter'

class Info {
    constructor(id, address, status, created, updated) {
        this.id = id
        this.details = id
        this.address = address
        this.status = status
        this.created = created
        this.updated = updated
    }
}

export default {
    state: {
        infos: []
    },
    mutations: {
        loadInfos(state, payload) {
            state.infos = payload
        }
    },
    actions: {
        async runTest({commit}, payload){
            commit('clearError')
            commit('setLoading', true)
            try {
                await api.info.post(payload)
                commit('setLoading', false)
            } catch (error) {
                commit('setError', error.message)
                commit('setLoading', false)
                throw error
            }
        },
        async fetchInfos({commit}) {
            commit('clearError')
            commit('setLoading', true)

            let results = []

            try {
                const response = await api.info.getAll()
                const data = response.data;

                results = data.map(item => {
                    return new Info(item.id, item.url, item.status, dateFormatter(item.createdUtc), dateFormatter(item.updatedUtc))
                })

                commit('loadInfos', results)
                commit('setLoading', false)
            } catch (error) {
                commit('setError', error.message)
                commit('setLoading', false)
                throw error
            }
        }
    },
    getters: {
        infos(state) {
            return state.infos
        }
    }
}