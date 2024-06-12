<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue'
import { HubConnectionBuilder } from '@microsoft/signalr'

type Update = {
  id: string
  status: string
}

const updates = ref<Update[]>([])

const connection = new HubConnectionBuilder()
  .withUrl('https://localhost:7138/task-hub', { withCredentials: false })
  .build()

connection.on('ReceiveMessage', (newUpdate: Update) => {
  const existingUpdate = updates.value.find((update) => update.id === newUpdate.id)

  if (existingUpdate === undefined) {
    updates.value = [...updates.value, newUpdate]
    return
  }

  existingUpdate.status = newUpdate.status

  updates.value = [...updates.value]
})

onMounted(() => {
  try {
    connection.start()
  } catch (error) {
    console.error(error)
  }
})

onUnmounted(() => {
  try {
    connection.stop()
  } catch (error) {
    console.error(error)
  }
})

const addTaskStatus = ref<'idle' | 'pending' | 'success' | 'error'>('idle')
const lastTaskIdAdded = ref<string>('')

async function addTask() {
  try {
    addTaskStatus.value = 'pending'
    const result = await fetch('https://localhost:7138/add-task', { method: 'POST' })

    if (result.ok === false) {
      addTaskStatus.value = 'error'
      return
    }

    const body = await result.json()

    lastTaskIdAdded.value = body.id
    addTaskStatus.value = 'success'
  } catch (error) {
    addTaskStatus.value = 'error'
  }
}
</script>

<template>
  <main>
    <div class="add-task-container">
      <button @click="addTask" type="button">Add Task</button>
      <Transition mode="out-in">
        <div v-if="addTaskStatus === 'pending'">Adding task...</div>
        <div v-else-if="addTaskStatus === 'success'">Task added with id: {{ lastTaskIdAdded }}</div>
        <div v-else-if="addTaskStatus === 'error'">Failed to add task</div>
        <div v-else>Click the button to add a task</div>
      </Transition>
    </div>
    <div class="updates-container">
      <h2>Updates</h2>
      <ul>
        <li v-for="update in updates" :key="update.id">{{ update.id }}: {{ update.status }}</li>
      </ul>
    </div>
  </main>
</template>

<style scoped>
main {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.add-task-container {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.updates-container {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
</style>
