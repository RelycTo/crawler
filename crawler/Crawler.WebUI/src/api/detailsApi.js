export default function (apiClient) {
    const detailsRoute = 'CrawlDetails';
    return {
        getByCrawlId(id) {
            return apiClient.get(`${detailsRoute}/${id}`)
        }
    }
}