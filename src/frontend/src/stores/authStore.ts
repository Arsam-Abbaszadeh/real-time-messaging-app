import { defineStore } from 'pinia';
import { computed, ref, type Ref } from 'vue';
import { requestNewAccessToken, requestlogin } from '@/api/authRequests';
import { ApiError } from '@/api/httpRequests';
import { isErrorWithMessage } from '@/utils/errorHelpers';
import type { LoginRequestDto } from '@/api/dtos/authDtos';
import type { authResult } from './types/authStoreTypes';

export const useAuthStore = defineStore('auth', () => {
    // state
    const accessToken = ref<string | null>(null);
    const accessTokenExpiry = ref<Date | null>(null);
    // getters
    const hasValidToken = computed(()=> {
        if (accessToken.value === null) {
            return false
        }
        const date = new Date()
        return date.toUTCString()
    })
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
            accessTokenExpiry.value = response.accessTokenExpiration;

            return {
                success: true,
                message: response.message, // should be mapping backend messages to valid frontend messages
            } as authResult;
        } catch (error) {
            if (error instanceof ApiError) {
                // do I need to still set these to null?
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
            const response = await requestNewAccessToken();
            accessToken.value = response.accessToken;
            accessTokenExpiry.value = response.accessTokenExpiration;
            return {
                success: true,
                message: response.message,
            } as authResult;
        } catch (error) {
            if (error instanceof ApiError) {
                // should never fail if the token is valid so reset everything for safety if it does
                // TODO: should we then be re directing to login page
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
        hasValidToken,
        // actions
        login,
        refreshAccessToken,
    };
});
