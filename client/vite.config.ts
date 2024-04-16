import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import VueDevTools from 'vite-plugin-vue-devtools'
import mkcert from'vite-plugin-mkcert'

// https://vitejs.dev/config/
export default defineConfig({
  server: {
    https: true,
  },
  plugins: [
    vue(),
    VueDevTools(),
    mkcert(),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  }
})
