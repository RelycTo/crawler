<template>
  <div>
    <v-container align-self="center" mt-4>
      <v-row no-gutters>
        <v-flex xs12 sm6 offset-sm3>
          <v-card
              elevation="2"
          >
            <v-container>
              <v-form
                  ref="form"
                  v-model="valid"
                  lazy-validation
              >
                <v-text-field
                    v-model="url"
                    :rules="urlRules"
                    label="URL to process"
                    placeholder="Enter URL"
                    required
                ></v-text-field>
              </v-form>
              <v-card-actions>
                <v-btn
                    :loading="loading"
                    :disabled="!valid || loading"
                    color="success"
                    class="mr-4"
                    @click="onSubmit"
                >
                  Run
                </v-btn>
              </v-card-actions>
            </v-container>
          </v-card>
        </v-flex>
      </v-row>
    </v-container>
    <v-container>
      <v-row v-if="!loading" no-gutters>
        <v-flex xs12 sm6 offset-sm3>
          <v-card
              elevation="2"
          >
            <v-card-title>Crawl tests</v-card-title>
            <v-card-text>
              <v-data-table
                  :headers="headers"
                  :items="infos"
                  :items-per-page="5"
                  item-key="index"
                  class="elevation-1"
                  :footer-props="{
      showFirstLastPage: true,
      firstIcon: 'mdi-arrow-collapse-left',
      lastIcon: 'mdi-arrow-collapse-right',
      prevIcon: 'mdi-minus',
      nextIcon: 'mdi-plus'
    }"
              >
                <template v-slot:[`item.details`]="{ item }">
                  <v-btn :to="{ name: 'details', params: { id: item.details }}"
                         color="info"
                         dark
                  >
                    View details
                  </v-btn>
                </template>
              </v-data-table>
            </v-card-text>
          </v-card>
        </v-flex>
      </v-row>
      <div v-else class="text-center pt-5">
        <v-progress-circular
            indeterminate
            :size="100"
            :width="4"
            color="blue-grey"
        ></v-progress-circular>
      </div>
    </v-container>
  </div>
</template>

<script>

const isURL = (value) => {
  let url;
  try {
    url = new URL(value);
  } catch (_) {
    return false;
  }
  return url.protocol === "http:" || url.protocol === "https:";
}

export default {
  name: "CrawlTests",
  data: () => ({
    valid: true,
    url: '',
    urlRules: [
      v => !!v || 'Url is required',
      v => isURL(v) || 'URL must be valid',
    ],
    headers: [
      {
        text: 'Id',
        align: 'end',
        value: 'id'
      },
      {
        text: 'Address',
        align: 'start',
        value: 'address'
      },
      {
        text: 'Date',
        value: 'created'
      },
      {
        text: 'Details',
        align: 'center',
        value: 'details'
      }
    ]
  }),
  computed: {
    loading() {
      return this.$store.getters.loading
    },
    infos() {
      return this.$store.getters.infos
    }
  },
  methods: {
    onSubmit() {
      if (this.$refs.form.validate()) {
        this.$store.dispatch('runTest', {
          url: this.url
        }).then(() => this.$store.dispatch('fetchInfos'))
            .catch(error => {
              this.$store.dispatch('setError', error.data.message)
            })
      }
    },
    validate() {
      this.$refs.form.validate()
    },
    reset() {
      this.$refs.form.reset()
    }
    ,
    resetValidation() {
      this.$refs.form.resetValidation()
    }
  }
}
</script>

<style>

</style>