import { defineStore } from 'pinia';
import { computed, ref, type Ref } from 'vue';
import type { ChatDetails } from './types/chatStoreTypes';

export const useChatStore = defineStore('chat', () => {
    // state
    const chatDetails = ref<ChatDetails[]>([]);

    // getters
});
