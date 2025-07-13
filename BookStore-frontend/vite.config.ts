import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  const isProduction = mode === 'production';
  
  return {
    plugins: [react()],
    base: process.env.VITE_DEPLOYMENT_TARGET === 'github-pages' ? '/Book/' : '/',
    server: {
      host: true,
      port: 5174,
      proxy: {
        '/api': {
          target: process.env.VITE_API_BASE_URL || 'https://localhost:7176',
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
      sourcemap: !isProduction,
      minify: isProduction ? 'esbuild' : false,
      target: 'es2015',
      rollupOptions: {
        output: {
          manualChunks: {
            vendor: ['react', 'react-dom'],
            router: ['react-router-dom'],
            utils: ['axios']
          }
        }
      }
    }
  }
})
