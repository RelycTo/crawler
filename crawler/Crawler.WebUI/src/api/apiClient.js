import axios from "axios"

const apiClient = axios.create({
    baseURL: 'http://localhost:5002',
    //withCredentials: true,
    headers: {
        accept: "application/json",
        "Access-Control-Allow-Origin": "*"
    }
})

export default apiClient