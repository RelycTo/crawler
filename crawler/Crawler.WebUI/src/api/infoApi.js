export default function (apiClient) {
    const infoRoute = 'CrawlInfo';
    return {
        post(payload) {
            return apiClient.post(infoRoute, payload)
        },
        getAll() {
            return apiClient.get(infoRoute)
        }
    }
}