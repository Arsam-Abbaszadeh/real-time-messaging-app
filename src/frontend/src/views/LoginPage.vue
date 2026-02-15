<template>
    <div class="form-page">
        <div class="form-wrapper">
            <h1>Login</h1>
            <form @submit.prevent="handleLogin" class="login-form">
                <div class="form-group">
                    <label for="username">Username</label>
                    <input
                        type="text"
                        id="username"
                        v-model="username"
                        required
                        placeholder="Enter your username"
                        class="form-text-input"
                        autocomplete="username"
                    />
                </div>

                <div class="form-group">
                    <label for="password">Password</label>
                    <input
                        type="password"
                        id="password"
                        v-model="password"
                        required
                        placeholder="Enter your password"
                        class="form-text-input"
                        autocomplete="password"
                    />
                </div>

                <div class="form-submit-container">
                    <button type="submit" class="form-button" :disabled="disableSubmit">
                        Login
                    </button>
                    <div class="error-message">{{ errorMesage }}</div>
                </div>
            </form>
            <RouterLink class="form-link" :to="{ name: ROUTE_NAMES.ACCOUNT }">
                need an account? create account</RouterLink
            >
        </div>
    </div>
</template>

<script setup lang="ts">
import { ROUTE_NAMES } from '../routing/routeNames';
import { computed, ref } from 'vue';
// import { requestlogin } from '../api/authRequests';
// import type { LoginRequestDto, LoginResponseDto } from '../api/dtos';
import { useAuthStore } from '../stores/authStore';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const route = useRouter();
const username = ref('');
const password = ref('');
const errorMesage = ref('');
const disableSubmit = computed(() => {
    return username.value.trim() === '' || password.value.trim() === '';
});

async function handleLogin() {
    errorMesage.value = '';
    const result = await authStore.login(username.value, password.value);
    // error messages not being mapped
    if (result.success) {
        console.log('Login successful'); // consider replacing with or adding a toast notification
        route.push({ name: ROUTE_NAMES.CHAT_LAYOUT });
    } else {
        errorMesage.value = result.message;
    }
}
</script>

<style scoped lang="scss"></style>
