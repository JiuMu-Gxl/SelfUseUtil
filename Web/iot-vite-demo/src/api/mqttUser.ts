import request from '@/utils/request'

export function createUserApi(username: string, password: string) {
  return request.post('/MqttUser/create', null, {
    params: {
      username,
      password,
    },
  })
}

export function changePasswordApi(username: string, newPassword: string) {
  return request.post('/MqttUser/change-password', null, {
    params: {
      username,
      newPassword,
    },
  })
}

export function deleteUserApi(username: string) {
  return request.delete('/MqttUser/delete', {
    params: {
      username,
    },
  })
}