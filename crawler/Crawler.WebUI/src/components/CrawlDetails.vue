<template>
  <v-container v-if="!loading" align-self="center" mt-4>
    <v-row no-gutters>
      <v-flex xs12 sm6 offset-sm3>
        <h1>Crawl details</h1>
      </v-flex>
    </v-row>
    <details-block v-for="detailsItem in details" :details="detailsItem" :key="detailsItem.id" />
  </v-container>
  <div v-else class="text-center">
    <v-progress-circular
        indeterminate
        :size="100"
        :width="4"
        color="blue-grey"
    />
  </div>
</template>

<script>
import DetailsBlock from "@/components/DetailsBlock";
export default {
  components: {DetailsBlock},
  props: [
    'id'
  ],
  name: "CrawlDetails",
  computed: {
    loading() {
      return this.$store.getters.loading
    },
    details() {
      return this.$store.getters.details
    }
  },
  created() {
    this.$store.dispatch('fetchDetails', this.id)
        .catch(error => {
          this.$store.dispatch('setError', error.data.message)
        })
  }
}
</script>

<style scoped>

</style>