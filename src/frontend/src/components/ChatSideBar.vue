<template>
    <div>My side bar</div>
</template>

<script setup lang="ts">
import { useChatStore } from '@/stores/chatStore';
import { computed, onMounted, ref } from 'vue';

const chatStore = useChatStore();

// TODO: defo should cache chat details, get the chat details first from cache and then make a request to get new potential chats

onMounted(async () => {
    await chatStore.refreshChatSummaries();
});
</script>

<style></style>
