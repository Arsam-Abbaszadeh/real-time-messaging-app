<template>
    <div>This is the component where chat window will go</div>
    <div v-if="currentChatId">{{ chatSummaries.find(c => c.id === currentChatId)?.name ?? '' }}</div>
    <ChatPlaceHolder v-else />
</template>

<script setup lang="ts">
import { useChatStore } from '@/stores/chatStore';
import { onMounted } from 'vue'; // will need eventually
import { storeToRefs } from 'pinia';
import ChatPlaceHolder from './ChatPlaceholder.vue';

const chatStore = useChatStore();
const { chatSummaries, currentChatId } = storeToRefs(chatStore);
</script>
