import apiClient from "./apiClient"
import infoApi from "./infoApi"
import detailsApi from "./detailsApi"

export default {
    info: infoApi(apiClient),
    details: detailsApi(apiClient)
}