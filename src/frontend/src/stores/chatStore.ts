import { defineStore } from 'pinia';
import { computed, ref } from 'vue';
import type { ChatSummary } from './types/chatStoreTypes';

export const useChatStore = defineStore('chat', () => {
    // state
    const chatSummaries = ref<ChatSummary[]>([]);
    const currentChatId = ref<string | null>(null);
    const chatHistoryCache = ref<string[]>([]); // need actual type for this. key should be chatID value should be chat summar and then nested is chat history

    // getters
    const chatCount = computed(() => chatSummaries.value.length);
    const currectChatSummaries = computed(() => {
        if (currentChatId.value === null) {
            return null;
        }
        return chatSummaries.value.find((d) => d.id === currentChatId.value);
    });

    // actions
    function setCurrentChatId(id: string) {
        if (!chatSummaries.value.some((d) => d.id === id)) {
            throw new Error(`Chat with id "${id}" not found`);
        }
        currentChatId.value = id;
    }

    function getChatSummaries() {
        // make request and set value
        let summaries: ChatSummary[] = [];
        chatSummaries.value = summaries;
    }

    function getChatHistory(id: string, firstMessageSeq: number, lastMessageSeq: number) {
        // make request
    }

    function getChatCachedHistory(id: string) {
        // use dexie.js to create cache and use it within this store
    }
});
