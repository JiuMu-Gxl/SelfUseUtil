import request from '@/utils/request'
import type { LoginRequest, PublishRequest } from '@/types/mqtt'

export function testConnectApi(data: LoginRequest) {
  return request.post('/Mqtt/test-connect', data)
}

export function publishApi(data: PublishRequest) {
  return request.post('/Mqtt/publish', data)
}

export function subscribeApi(topic: string) {
  return request.get(`/Mqtt/subscribe/${topic}`)
}

export function disconnectApi(username: string) {
  return request.get(`/Mqtt/disconnect/${username}`)
}