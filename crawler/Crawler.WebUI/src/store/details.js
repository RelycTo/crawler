import api from '../api'

const types = {
    Site: "SITE",
    Sitemap: "SITEMAP",
    Both: "BOTH",
    None: ""
}

const sourceType = value => {
    switch (value){
        case 1: return types.Site;
        case 2: return types.Sitemap;
        case 3: return types.Both;
        default: return types.None;
    }
}

class Details {
    constructor(index, address, duration, sourceType) {
        this.index = index
        this.address = address
        this.duration = duration
        this.sourceType = sourceType
    }
}

export default {
    state: {
        siteMapUniques: [],
        siteUniques: [],
        timings: [],
        headers: {
            uniquesHeaders: [
                {
                    text: '#',
                    value: 'index',
                    align: 'end'
                },
                {
                    text: 'Url',
                    align: 'start',
                    value: 'address'
                }
            ],
            timingsHeaders: [
                {
                    text: '#',
                    value: 'index',
                    align: 'end'
                },
                {
                    text: 'Url',
                    align: 'start',
                    value: 'address'
                },
                {
                    text: 'Timing (ms)',
                    align: 'end',
                    value: 'duration'
                }
            ]
        }
    },
    mutations: {
        loadSiteMapUniques(state, payload) {
            state.siteMapUniques = payload
                .filter(p => p.sourceType === types.Sitemap)
                .map((p, index) => {
                    return new Details(index + 1, p.address, p.duration, p.sourceType)
                })
        },
        loadSiteUniques(state, payload) {
            state.siteUniques = payload
                .filter(p => p.sourceType === types.Site)
                .map((p, index) => {
                    return new Details(index + 1, p.address, p.duration, p.sourceType)
                })
        },
        loadTimings(state, payload) {
            state.timings = payload
        }
    },
    actions: {
        async fetchDetails({commit}, id) {
            commit('clearError')
            commit('setLoading', true)

            let results = []

            try {
                const response = await api.details.getByCrawlId(id)
                const data = response.data;

                results = data.map((item, index) => {
                    return new Details(index + 1, item.address, item.duration, sourceType(item.sourceType))
                })

                commit('loadSiteMapUniques', results)
                commit('loadSiteUniques', results)
                commit('loadTimings', results)
                commit('setLoading', false)
            } catch (error) {
                commit('setError', error.message)
                commit('setLoading', false)
                throw error
            }
        }
    },
    getters: {
        details(state){
          return [
              {
                  id: 'sitemap',
                  title: 'Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site',
                  headers: state.headers.uniquesHeaders,
                  items: state.siteMapUniques
              },
              {
                  id: 'site',
                  title: 'Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml',
                  headers: state.headers.uniquesHeaders,
                  items: state.siteUniques
              },
              {
                  id: 'timings',
                  title: 'Timing',
                  headers: state.headers.timingsHeaders,
                  items: state.timings
              },
          ]
        },
        siteMapUniques(state) {
            return state.siteMapUniques
        },
        siteUniques(state) {
            return state.siteUniques
        },
        timings(state) {
            return state.timings
        }
    }
}