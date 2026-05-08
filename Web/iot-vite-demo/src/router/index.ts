import { createRouter, createWebHistory } from 'vue-router'
import { useUserStore } from '@/stores/user'

const router = createRouter({
  history: createWebHistory(),

  routes: [
    {
      path: '/login',
      component: () => import('@/views/Login.vue'),
    },

    {
      path: '/register',
      component: () => import('@/views/Register.vue'),
    },

    {
      path: '/change-password',
      component: () => import('@/views/ChangePassword.vue'),
    },

    {
      path: '/',
      component: () => import('@/views/Dashboard.vue'),
      meta: {
        requiresAuth: true,
      },
    },
  ],
})

router.beforeEach((to) => {
  const userStore = useUserStore()

  // 需要登录
  if (to.meta.requiresAuth) {
    if (!userStore.connected || !userStore.username) {
      return '/login'
    }
  }

  return true
})

export default router