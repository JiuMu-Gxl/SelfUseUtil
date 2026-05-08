import axios from 'axios'

import type {
  AxiosResponse,
  InternalAxiosRequestConfig,
} from 'axios'

import { ElMessage } from 'element-plus'

const service = axios.create({
  baseURL: import.meta.env.VITE_APP_BASE_API,

  timeout: Number(
    import.meta.env.VITE_APP_TIMEOUT,
  ),
})
service.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    return config
  },

  (error) => {
    return Promise.reject(error)
  },
)

service.interceptors.response.use(
  (response: AxiosResponse) => {
    return response.data
  },

  (error) => {
    let message = '请求失败'

    if (error.response) {
      switch (error.response.status) {
        case 400:
          message = '请求错误'
          break

        case 401:
          message = '未授权，请重新登录'
          break

        case 403:
          message = '拒绝访问'
          break

        case 404:
          message = '接口不存在'
          break

        case 500:
          message = '服务器内部错误'
          break

        default:
          message = `请求失败 ${error.response.status}`
      }
    } else if (error.message?.includes('timeout')) {
      message = '请求超时'
    } else {
      message = '网络异常'
    }

    ElMessage.error(message)

    return Promise.reject(error)
  },
)

export default service