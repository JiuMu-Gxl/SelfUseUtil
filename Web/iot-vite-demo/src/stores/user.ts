import { defineStore } from 'pinia'

interface UserState {
  username: string
  connected: boolean
}

export const useUserStore = defineStore('user', {
  state: (): UserState => ({
    username: '',
    connected: false,
  }),

  actions: {
    login(username: string) {
      this.username = username
      this.connected = true
    },

    logout() {
      this.username = ''
      this.connected = false
    },
  },
})