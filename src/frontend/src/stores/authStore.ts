import { defineStore } from 'pinia';
import { computed, ref, type Ref } from 'vue';
import { refreshAccessTokenRequest, requestlogin } from '@/api/authRequests';
import { ApiError } from '@/api/httpRequests';
import { isErrorWithMessage } from '@/utils/errorHelpers';
import type { LoginRequestDto } from '@/api/dtos/authDtos';
import type { authResult } from './types/authStoreTypes';

export const useAuthStore = defineStore('auth', () => {
    // state
    const accessToken = ref<string | null>(null);
    const accessTokenExpiry: Ref<Date | null> = ref(null);

    // getters
    const hasTokenValid = computed(() => {
        if (!accessToken || !accessTokenExpiry.value) {
            return false;
        }
        return Date.now() < accessTokenExpiry.value.getTime();
    });

    // actions
    async function login(username: string, password: string): Promise<authResult> {
        // add return type object
        const dto = {
            username,
            password,
        } as LoginRequestDto;

        try {
            // need to check if we can unsucessful login without throwing error
            const response = await requestlogin(dto);
            accessToken.value = response.accessToken;
            accessTokenExpiry.value = response.accessTokenExpiration;

            return {
                success: true,
                message: response.message, // should be mapping backend messages to valid frontend messages
            } as authResult;
        } catch (error) {
            if (error instanceof ApiError) {
                // do I need to still set these to null?
                accessToken.value = null;
                accessTokenExpiry.value = null;
                return {
                    success: false,
                    message: error.message,
                } as authResult;
            }

            if (isErrorWithMessage(error)) {
                return {
                    success: false,
                    message: error.message,
                } as authResult;
            }

            return {
                success: false,
                message: 'An unexpected error occurred',
            } as authResult;
        }
    }

    async function refreshAccessToken(): Promise<authResult> {
        if (!accessToken.value) {
            throw Error('No access token to refresh'); // should not happen if used correctly
        }

        try {
            const response = await refreshAccessTokenRequest();
            accessToken.value = response.accessToken;
            accessTokenExpiry.value = response.accessTokenExpiration;
            return {
                success: true,
                message: response.message,
            } as authResult;
        } catch (error) {
            if (error instanceof ApiError) {
                // should never fail if the token is valid so reset everything for safety if it does
                accessToken.value = null;
                accessTokenExpiry.value = null;
                return {
                    success: false,
                    message: error.message,
                } as authResult;
            }

            if (isErrorWithMessage(error)) {
                return {
                    success: false,
                    message: error.message,
                } as authResult;
            }

            return {
                success: false,
                message: 'An unexpected error occurred',
            } as authResult;
        }
    }

    return {
        // state
        accessToken,
        accessTokenExpiry,
        // getters
        hasTokenValid,
        // actions
        login,
        refreshAccessToken,
    };
});
