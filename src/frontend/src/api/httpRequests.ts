const API_BASE_URL = import.meta.env.VITE_API_URL as string;

export class ApiError extends Error {
  public readonly status: number;
  public readonly body?: unknown;

  constructor(
    message: string,
    status: number,
    body?: unknown
  ) {
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

if (typeof body === 'string') {
  throw new ApiError(`Got: "${body}"`, response.status);
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