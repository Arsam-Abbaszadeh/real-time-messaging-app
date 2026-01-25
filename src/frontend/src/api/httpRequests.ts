const API_BASE_URL = import.meta.env.VITE_API_URL as string;

export class ApiError extends Error {
    public readonly status: number;
    public readonly body?: unknown;

    constructor(message: string, status: number, body?: unknown) {
        // error constructor only takes message
        super(message);
        this.name = 'ApiError';
        this.status = status;
        this.body = body;
    }
}

export async function fetchJson<TResponse>(
    path: string,
    init: RequestInit = {}
): Promise<TResponse> {
    const url = `${API_BASE_URL}${path}`;

    const response = await fetch(url, {
        ...init,
        headers: {
            ...(init.headers ?? {}),
        },
    });

    const text = await response.text();
    const body = text ? safeJsonParse(text) : undefined;

    if (!response.ok) {
        // handle errors I expect form the API
        if (response.status === 400 || response.status === 401) {
            const errorMessage = (body as any)?.message || response.statusText;

            console.log(errorMessage);
            throw new ApiError(errorMessage, response.status, body);
        }
        // other unexpected errors
        const errorMessage =
            typeof body === 'string' ? body : (body as any)?.message || response.statusText;

        throw new ApiError(errorMessage, response.status, body);
    }

    // handle unexpected non-JSON responses
    if (typeof body === 'string') {
        throw new ApiError(`Expected JSON but got: "${body}"`, response.status);
    }

    return body as TResponse;
}

function safeJsonParse(text: string): unknown {
    try {
        return JSON.parse(text);
    } catch {
        return text;
    }
}
