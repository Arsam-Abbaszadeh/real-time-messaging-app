<template>
    <H1>Test</H1>
</template>

<script setup lang="ts">
import { useChatStore } from '@/stores/chatStore';
import { computed, ref } from 'vue';
// should these be moved to a store?
let chatDetails = ref(null);
let currentChat = ref<string | null>(null);
const chatStore = useChatStore();

async function test() {
    await chatStore.refreshChatSummaries();
    console.log(chatStore);
}
await test();
</script>

<style scoped></style>
