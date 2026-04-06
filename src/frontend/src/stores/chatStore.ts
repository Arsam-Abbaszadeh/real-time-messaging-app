import { defineStore } from 'pinia';
import { computed, ref } from 'vue';
import type { ChatSummary } from './types/chatStoreTypes';
import type { chatSummaryDto } from '@/api';
import { requestWithAuthRetry } from '@/utils/requestHelper';
import SignalRConnection from '@/api/signalR/connection';
import type { chatMessageDto, PaginatedChatHistoryOptionsDto } from '@/api/dtos/chatDtos';
import { getChatSummaries, getMessages } from '@/api/http/chatRequests';

export const useChatStore = defineStore('chat', () => {
    const chatHub = new SignalRConnection();
    // state
    const chatSummaries = ref<ChatSummary[]>([]);
    const currentChatId = ref<string | null>(null);
    const chatHistoryCache = ref<string[]>([]); // need actual type for this. key should be chatID value should be chat summar and then nested is chat history
    const tempMessageStore = ref<chatMessageDto[]>([]); // this is just for testing the chat history retrieval functions, will be removed after testing

    // getters
    const chatCount = computed(() => chatSummaries.value.length);
    const currectChatSummaries = computed(() => {
        if (currentChatId.value === null) {
            return null;
        }
        return chatSummaries.value.find(d => d.id === currentChatId.value);
    });

    // actions
    function setCurrentChatId(id: string): void {
        if (!chatSummaries.value.some(d => d.id === id)) {
            throw new Error(`Chat with id "${id}" not found`);
        }
        currentChatId.value = id;
    }

    async function refreshChatSummaries(): Promise<void> {
        // TODO: do we need any error handelling here?
        let summaries = await requestWithAuthRetry<chatSummaryDto[]>(getChatSummaries);
        chatSummaries.value = summaries.map(toChatSummary);
    }

    async function getNewMessages(id: string, range: number): Promise<void> {
        // make request
        const history: chatMessageDto[] = await requestWithAuthRetry<chatMessageDto[]>(() => getMessages(id, range));
        tempMessageStore.value = history; // for testing, remove after testing
    }

    async function getMessageRange(id: string, chatHistoryOptions: PaginatedChatHistoryOptionsDto): Promise<void> {
        const history: chatMessageDto[] = await requestWithAuthRetry<chatMessageDto[]>(() =>
            getMessages(id, undefined, chatHistoryOptions)
        );
        tempMessageStore.value = history; // for testing, remove after testing
    }

    function getCachedChatHistory(id: string) {}
    // use dexie.js to create cache and use it within this store

    return {
        // state
        tempMessageStore,
        // getters
        refreshChatSummaries,
        getNewMessages,
        getMessageRange,
        // actions
        setCurrentChatId,
    };
});

// helpers
function toChatSummary(dto: chatSummaryDto): ChatSummary {
    return {
        id: dto.chatId,
        name: dto.chatName,
        imageUrl: dto.chatImageUrl,
    };
}
