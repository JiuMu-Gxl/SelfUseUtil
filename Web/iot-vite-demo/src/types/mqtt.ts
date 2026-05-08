export interface LoginRequest {
  username: string
  password: string
}

export interface PublishRequest {
  topic: string
  payload: string
}