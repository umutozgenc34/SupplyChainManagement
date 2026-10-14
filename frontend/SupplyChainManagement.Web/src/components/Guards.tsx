import type { ReactNode } from 'react'
import { Navigate, useLocation } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

export function RequireAuth({ children }: { children: ReactNode }) {
  const { auth } = useAuth()
  const location = useLocation()
  if (!auth) return <Navigate to="/login" replace state={{ from: location.pathname }} />
  return <>{children}</>
}

export function RequireAdmin({ children }: { children: ReactNode }) {
  const { isAdmin } = useAuth()
  if (!isAdmin) {
    return (
      <div className="error">
        <b>Access denied.</b> This function is available to administrators only.
      </div>
    )
  }
  return <>{children}</>
}
