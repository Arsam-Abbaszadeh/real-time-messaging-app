import { fetchJson } from './httpRequests.ts';
import { HEADERS, HEADER_VALUES } from './httpRequestHeaderConstants.ts';
import type { chatSummary } from './dtos/chatDtos.ts';
import { useAuthStore } from '@/stores/authStore.ts';

const authstore = useAuthStore();

// DTOs are not exactly perfect yet, also need to use auth store probs
export function getChatSummaries(): Promise<Array<chatSummary>> {
    return fetchJson<Array<chatSummary>>('/chat/chatsummaries', {
        method: 'GET',
    });
}
