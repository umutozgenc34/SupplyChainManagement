import { createContext, useCallback, useContext, useEffect, useMemo, useState, type ReactNode } from 'react'
import { loadAuth, onUnauthorized, saveAuth } from '../api/client'
import { authApi } from '../api/endpoints'
import type { AuthInfo } from '../api/types'

interface AuthContextValue {
  auth: AuthInfo | null
  isAdmin: boolean
  login: (email: string, password: string) => Promise<void>
  register: (email: string, password: string) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [auth, setAuth] = useState<AuthInfo | null>(() => loadAuth())

  const apply = useCallback((value: AuthInfo | null) => {
    saveAuth(value)
    setAuth(value)
  }, [])

  useEffect(() => {
    onUnauthorized(() => setAuth(null))
  }, [])

  useEffect(() => {
    if (!auth) return
    const remaining = new Date(auth.expiresAt).getTime() - Date.now()
    const timer = window.setTimeout(() => apply(null), Math.max(remaining, 0))
    return () => window.clearTimeout(timer)
  }, [auth, apply])

  const value = useMemo<AuthContextValue>(() => ({
    auth,
    isAdmin: auth?.role === 'Admin',
    login: async (email, password) => {
      const result = await authApi.login(email, password)
      apply(result.data ?? null)
    },
    register: async (email, password) => {
      const result = await authApi.register(email, password)
      apply(result.data ?? null)
    },
    logout: () => apply(null)
  }), [auth, apply])

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) throw new Error('useAuth must be used inside AuthProvider')
  return context
}
