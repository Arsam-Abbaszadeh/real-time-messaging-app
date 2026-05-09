<template>
    <div class="chat-layout">
        <div class="chat-sidebar">
            <ChatSideBar />
        </div>
        <!-- Need condition for rendering one over the other -->
        <div class="chat-container">
            <ChatPlaceholder v-if="currentChat == null" />
            <ChatWindow v-else />
        </div>
    </div>
</template>

<script setup lang="ts">
import ChatPlaceholder from '@/components/ChatPlaceholder.vue';
import ChatSideBar from '@/components/ChatSideBar.vue';
import ChatWindow from '@/components/ChatWindow.vue';
import { useChatStore } from '@/stores/chatStore';
import { computed, ref } from 'vue';

// should be spread propely amongst sidebar and chat
let chatDetails = ref(null);
let currentChat = ref<string | null>(null);
const chatStore = useChatStore();

currentChat.value = 'temp';

async function test() {
    await chatStore.refreshChatSummaries();
    console.log(chatStore);
}

// await test();
</script>

<style scoped lang="scss">
.chat-layout {
    flex: 1;
    display: grid;
    grid-template-columns: 15rem 1fr;
    min-height: 0;
}

.chat-container {
    border: 1px solid red;
    min-width: 0;
    min-height: 0;
}
</style>
