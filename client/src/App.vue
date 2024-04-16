<script setup lang="ts">
import { ref } from 'vue';


const addTaskStatus = ref<'idle' | 'pending' | 'success' | 'error'>('idle');
const lastTaskIdAdded = ref<string>('');

async function addTask() {
  try {
    addTaskStatus.value = 'pending';
    const result = await fetch('https://localhost:7138/add-task', { method: 'POST' });

    if (result.ok === false) {
      addTaskStatus.value = 'error';
      return;
    }

    const body = await result.json();

    lastTaskIdAdded.value = body.id;
    addTaskStatus.value = 'success';
  } catch (error) {
    addTaskStatus.value = 'error';
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
  </main>
</template>

<style scoped>
main {
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
}

.add-task-container {
  display: flex;
  align-items: center;
  gap: 1rem;
}
</style>
