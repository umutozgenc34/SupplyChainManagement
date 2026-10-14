import { useLocation } from 'react-router-dom'
import type { LoadError } from '../useApi'

export function ErrorBox({ error }: { error: LoadError | null }) {
  if (!error) return null
  return (
    <div className="error">
      <b>Error:</b> {error.message}
      {error.errors.length > 0 && (
        <ul>
          {error.errors.map(e => <li key={e}>{e}</li>)}
        </ul>
      )}
    </div>
  )
}

export function Loading() {
  return <div className="loading">Loading, please wait...</div>
}

export function FlashMessage() {
  const location = useLocation()
  const message = (location.state as { message?: string } | null)?.message
  if (!message) return null
  return <div className="message">{message}</div>
}
