import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'

// https://vite.dev/config/
export default defineConfig({
    plugins: [vue()],
    resolve: {
        alias: {
            '@': path.resolve(__dirname, './src'),
        },
    },
    server: {
        port: process.env.DEFAULT_FRONTEND_PORT ? parseInt(process.env.DEFAULT_FRONTEND_PORT) : 5173,
        strictPort: true, // fail if 5173 is taken instead of auto-choosing another
    },
    build: {
        sourcemap: true,
    },
    css: {
        preprocessorOptions: {
            scss: {
                additionalData: `@use "@/styles/_globals.scss" as *;\n`,
            },
        },
    },
})
