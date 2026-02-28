import { ApiError } from '@/api/httpRequests';
import { useAuthStore } from '@/stores/authStore';

const authStore = useAuthStore();

export async function requestWithAuthRetry<TResponse>(
    requestCallback: () => Promise<TResponse>,
    requestRetryCount = 2,
    maxAuthRetryCount = 1,
    redirectOnFail = true
) {
    let authRetryCount = 0;
    while (requestRetryCount > 0) {
        try {
            let response = await requestCallback();
            return response;
        } catch (error) {
            if (error instanceof ApiError && error.status === 401) {
                authRetryCount++;
                if (authRetryCount > maxAuthRetryCount) {
                    if (redirectOnFail) {
                        // redirect to login page
                        // TODO: figure out how to the above
                        throw new Error(); // temp
                    } else {
                        throw new ApiError(
                            `re auth reqeust failed, with message: ${error.message}`,
                            401,
                            error.body
                        );
                    }
                }
                authStore.refreshAccessToken();
            } else {
                throw error; // is this correct?
            }
        }
        requestRetryCount--;
    }
    if (redirectOnFail) {
        // redirect to login page
        throw new Error(); // temp
    }
    throw new Error(); // probs need to create an auther error
}
