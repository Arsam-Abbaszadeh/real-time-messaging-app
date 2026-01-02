<template>
    <h1>
        Create account page
    </h1>
    <RouterLink :to="{ name: ROUTE_NAMES.LOGIN }">link to login page</RouterLink>    
</template>

<script setup lang="ts">
import { ROUTE_NAMES } from '../utils/routeNames';

</script>