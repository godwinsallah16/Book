import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import basicSsl from '@vitejs/plugin-basic-ssl'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), basicSsl()],
  base: process.env.VITE_DEPLOYMENT_TARGET === 'github-pages' ? '/Book/' : '/',
  server: {
    host: true,
    port: 5174,
    proxy: {
      '/api': {
        target: 'https://localhost:7176',
        changeOrigin: true,
        secure: false
      }
    }
  },
  preview: {
    port: 5174
  },
  build: {
    outDir: 'dist',
    assetsDir: 'assets',
    sourcemap: false
  }
})
