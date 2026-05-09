<template>
    <div>My side bar</div>
    <ul class="chat-details">
        <li
            v-for="chatDetails in chatSummaries"
            :key="chatDetails.id"
            :class="['chat-detail', chatDetails.id === currentChatId ? 'active-chat' : '']"
            @click="chatStore.setCurrentChatId(chatDetails.id)"
        >
            {{ chatDetails.name }}
        </li>
    </ul>
</template>

<script setup lang="ts">
import { useChatStore } from '@/stores/chatStore';
import { onMounted } from 'vue';
import { storeToRefs } from 'pinia';

const chatStore = useChatStore();
const { chatSummaries, currentChatId } = storeToRefs(chatStore);
// TODO: defo should cache chat details, get the chat details first from cache and then make a request to get new potential chats

onMounted(async () => {
    await chatStore.refreshChatSummaries();
});
</script>

<style scoped>
.chat-detail {
    border: black 1px solid;
}

.chat-details {
    list-style: none;
}

.chat-details li {
    cursor: pointer;
    /* padding: 0.75rem 1rem; */
}
</style>
