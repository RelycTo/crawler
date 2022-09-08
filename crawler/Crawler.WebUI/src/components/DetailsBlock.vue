<template>
  <v-container align-self="center" mt-4>
    <v-row no-gutters>
      <v-flex xs12 sm6 offset-sm3>
        <v-card
            elevation="2"
        >
          <v-card-title>{{ details.title }}</v-card-title>
          <v-card-text>
            <v-data-table
                :headers="details.headers"
                :items="details.items"
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
            </v-data-table>
          </v-card-text>
        </v-card>
      </v-flex>
    </v-row>
  </v-container>
</template>

<script>
export default {
  name: "DetailsBlock",
  props: [
    'details'
  ]
}
</script>

<style scoped>

</style>