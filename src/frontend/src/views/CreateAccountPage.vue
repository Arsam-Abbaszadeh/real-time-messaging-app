<template>

<div class="form-page">
    <div class="form-wrapper">
        <h1>Create Account</h1>
        <form @submit.prevent="handleCreateAccount" class="create-account-form">
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
                autocomplete="new-password"
                />
            </div>
            
            <div class="form-submit-container">
                <button type="submit" class="form-button" :disabled="disableSubmit">Create Account</button>
                <div class="error-message">{{ errorMesage }}</div>
            </div>
        </form>
        <RouterLink class="form-link" :to="{ name: ROUTE_NAMES.LOGIN }">already have an account? login</RouterLink>
    </div>
</div>

</template>
  
<script setup lang="ts">
import { ROUTE_NAMES } from '../utils/routeNames';
import { computed, ref } from 'vue';

const username = ref('');
const password = ref('');
const errorMesage = ref('');
const disableSubmit = computed(() => {
    return username.value.trim() === '' || password.value.trim() === '';
});

const handleCreateAccount = () => {
    // Placeholder for create account logic
    console.log('Creating account with', username.value, password.value);
};

</script>

<style scoped lang="scss">

</style>