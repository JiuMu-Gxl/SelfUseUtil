<template>
  <div class="dashboard">
    <div class="top-card">
      <div class="user-area">
        <div class="avatar">
          {{ userStore.username?.charAt(0).toUpperCase() }}
        </div>

        <div>
          <div class="welcome">
            欢迎回来
          </div>

          <div class="username">
            {{ userStore.username }}
          </div>
        </div>
      </div>

      <el-button
        type="danger"
        plain
        @click="handleDisconnect"
      >
        断开连接
      </el-button>
    </div>

    <div class="device-card">
      <div class="icon-wrap">
        💡
      </div>

      <h1>ESP32 智能灯控</h1>

      <p>通过 MQTT 实时控制设备状态</p>

      <div class="actions">
        <el-button
          type="success"
          size="large"
          class="control-btn"
          @click="handleLightOn"
        >
          开灯
        </el-button>

        <el-button
          type="warning"
          size="large"
          class="control-btn"
          @click="handleLightOff"
        >
          关灯
        </el-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'

import { ElMessage } from 'element-plus'

import { useUserStore } from '@/stores/user'

import {
  publishApi,
  disconnectApi,
} from '@/api/mqtt'

const router = useRouter()

const userStore = useUserStore()

async function handleLightOn() {
  try {
    await publishApi({
      topic: 'esp32/led',
      payload: 'ON',
    })

    ElMessage.success('灯已打开')
  } catch {
    ElMessage.error('操作失败')
  }
}

async function handleLightOff() {
  try {
    await publishApi({
      topic: 'esp32/led',
      payload: 'OFF',
    })

    ElMessage.success('灯已关闭')
  } catch {
    ElMessage.error('操作失败')
  }
}

async function handleDisconnect() {
  try {
    await disconnectApi(userStore.username)

    userStore.logout()

    ElMessage.success('已断开连接')

    router.push('/login')
  } catch {
    ElMessage.error('断开失败')
  }
}
</script>

<style scoped>
.dashboard {
  width: 100%;
  min-height: 100vh;

  padding: 24px;

  color: white;
}

.top-card {
  max-width: 1200px;

  margin: 0 auto 40px;

  padding: 18px 24px;

  border-radius: 24px;

  backdrop-filter: blur(20px);

  background: rgba(255, 255, 255, 0.08);

  border: 1px solid rgba(255, 255, 255, 0.12);

  display: flex;

  justify-content: space-between;

  align-items: center;
}

.user-area {
  display: flex;

  align-items: center;

  gap: 16px;
}

.avatar {
  width: 56px;
  height: 56px;

  border-radius: 50%;

  background:
    linear-gradient(
      135deg,
      #3b82f6,
      #06b6d4
    );

  display: flex;
  justify-content: center;
  align-items: center;

  font-size: 22px;
  font-weight: bold;
}

.welcome {
  color: rgba(255, 255, 255, 0.7);

  margin-bottom: 4px;
}

.username {
  font-size: 22px;

  font-weight: bold;
}

.device-card {
  max-width: 560px;

  margin: 0 auto;

  padding: 50px 32px;

  border-radius: 30px;

  text-align: center;

  backdrop-filter: blur(20px);

  background: rgba(255, 255, 255, 0.08);

  border: 1px solid rgba(255, 255, 255, 0.12);

  box-shadow:
    0 8px 32px rgba(0, 0, 0, 0.28);
}

.icon-wrap {
  font-size: 88px;

  margin-bottom: 20px;
}

.device-card h1 {
  font-size: 34px;

  margin-bottom: 12px;
}

.device-card p {
  color: rgba(255, 255, 255, 0.72);

  margin-bottom: 36px;
}

.actions {
  display: flex;

  justify-content: center;

  gap: 20px;
}

.control-btn {
  width: 150px;

  height: 52px;

  border-radius: 14px;

  font-size: 16px;
}

@media (max-width: 768px) {
  .dashboard {
    padding: 16px;
  }

  .top-card {
    flex-direction: column;

    gap: 20px;

    align-items: flex-start;
  }

  .device-card {
    padding: 36px 20px;
  }

  .actions {
    flex-direction: column;
  }

  .control-btn {
    width: 100%;
  }
}
</style>