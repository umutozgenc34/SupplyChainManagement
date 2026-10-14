import { useCallback, useEffect, useState } from 'react'
import { ApiError } from './api/client'

export interface LoadError {
  message: string
  errors: string[]
}

export function toLoadError(error: unknown): LoadError {
  if (error instanceof ApiError) return { message: error.message, errors: error.errors }
  if (error instanceof Error) return { message: error.message, errors: [] }
  return { message: 'An unexpected error occurred.', errors: [] }
}

export function useApi<T>(loader: () => Promise<T>, deps: unknown[]) {
  const [data, setData] = useState<T | null>(null)
  const [error, setError] = useState<LoadError | null>(null)
  const [loading, setLoading] = useState(true)
  const [version, setVersion] = useState(0)

  useEffect(() => {
    let active = true
    setLoading(true)
    setError(null)
    loader()
      .then(result => {
        if (active) setData(result)
      })
      .catch(err => {
        if (active) setError(toLoadError(err))
      })
      .finally(() => {
        if (active) setLoading(false)
      })
    return () => {
      active = false
    }
  }, [...deps, version])

  const reload = useCallback(() => setVersion(v => v + 1), [])

  return { data, error, loading, reload }
}
