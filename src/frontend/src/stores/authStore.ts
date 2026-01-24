import { defineStore } from "pinia";
import { computed, ref, type Ref } from "vue";
import { refreshAccessTokenRequest, requestlogin } from "@/api/authRequests";
import type { LoginRequestDto } from "@/api/dtos/authDtos";

export const useAuthStore = defineStore("auth", () => {
    // state
    const accessToken = ref<string | null>(null);
    const accessTokenExpiry: Ref<Date | null> = ref(null);

    // getters
    const hasTokenValid = computed(() => {
        if (!accessToken || !accessTokenExpiry.value) {
            return false;
        }
        return Date.now() < accessTokenExpiry.value.getTime();
    })

    // actions 
    async function getAccessToken(username: string, password: string) { // add return type object
        const dto = {
            username,
            password
        } as LoginRequestDto;

        try {
            const response = await requestlogin(dto)
            accessToken.value = response.accessToken;
            accessTokenExpiry.value = response.accessTokenExpiration;

        }
        catch (error) {
            // do I need to still set these to null?
            accessToken.value = null;
            accessTokenExpiry.value = null;
            throw error;
        }
    }

    async function refreshAccessToken() {
        if (!accessToken.value) {
            throw Error("No access token to refresh"); // should not happen if used correctly
        }

        try {
            const response = await refreshAccessTokenRequest()
            accessToken.value = response.accessToken;
            accessTokenExpiry.value = response.accessTokenExpiration;
            // return response; // return proper thing
        } catch (error) {
            accessToken.value = null;
            accessTokenExpiry.value = null;
            throw error;
        }
    }

    return {
        // state
        accessToken,
        accessTokenExpiry,
        // getters
        hasTokenValid,
        // actions
        getAccessToken,
        refreshAccessToken
    }
})