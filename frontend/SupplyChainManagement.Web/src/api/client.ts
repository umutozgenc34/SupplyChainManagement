import type { AuthInfo, ServiceResult } from './types'

const AUTH_KEY = 'scm.auth'

export class ApiError extends Error {
  status: number
  errors: string[]

  constructor(message: string, status: number, errors: string[] = []) {
    super(message)
    this.status = status
    this.errors = errors
  }
}

let unauthorizedHandler: () => void = () => {}

export function onUnauthorized(handler: () => void) {
  unauthorizedHandler = handler
}

export function loadAuth(): AuthInfo | null {
  try {
    const raw = localStorage.getItem(AUTH_KEY)
    if (!raw) return null
    const auth = JSON.parse(raw) as AuthInfo
    if (new Date(auth.expiresAt).getTime() <= Date.now()) {
      localStorage.removeItem(AUTH_KEY)
      return null
    }
    return auth
  } catch {
    return null
  }
}

export function saveAuth(auth: AuthInfo | null) {
  try {
    if (auth) localStorage.setItem(AUTH_KEY, JSON.stringify(auth))
    else localStorage.removeItem(AUTH_KEY)
  } catch {
    return
  }
}

function collectErrors(payload: unknown): string[] {
  if (!payload || typeof payload !== 'object' || !('errors' in payload)) return []
  const errors = (payload as { errors: unknown }).errors
  if (Array.isArray(errors)) return errors.map(String)
  if (errors && typeof errors === 'object') return Object.values(errors).flat().map(String)
  return []
}

async function request<T>(method: string, url: string, body?: unknown): Promise<ServiceResult<T>> {
  const auth = loadAuth()
  const headers: Record<string, string> = { Accept: 'application/json' }
  if (body !== undefined) headers['Content-Type'] = 'application/json'
  if (auth) headers.Authorization = `Bearer ${auth.accessToken}`

  let response: Response
  try {
    response = await fetch(`/api${url}`, {
      method,
      headers,
      body: body === undefined ? undefined : JSON.stringify(body)
    })
  } catch {
    throw new ApiError('The server could not be reached. Please try again later.', 0)
  }

  const text = await response.text()
  let payload: (ServiceResult<T> & { title?: string }) | null = null
  if (text) {
    try {
      payload = JSON.parse(text)
    } catch {
      payload = null
    }
  }

  if (response.status === 401 && auth) {
    saveAuth(null)
    unauthorizedHandler()
  }

  if (!response.ok || payload?.success === false) {
    const message = payload?.message || payload?.title || `Request failed with status ${response.status}.`
    throw new ApiError(message, response.status, collectErrors(payload))
  }

  return payload ?? { success: true, message: '' }
}

export const api = {
  async get<T>(url: string): Promise<T> {
    const result = await request<T>('GET', url)
    return result.data as T
  },
  post<T = unknown>(url: string, body?: unknown) {
    return request<T>('POST', url, body)
  },
  put<T = unknown>(url: string, body?: unknown) {
    return request<T>('PUT', url, body)
  },
  patch<T = unknown>(url: string, body?: unknown) {
    return request<T>('PATCH', url, body)
  },
  delete<T = unknown>(url: string) {
    return request<T>('DELETE', url)
  }
}
