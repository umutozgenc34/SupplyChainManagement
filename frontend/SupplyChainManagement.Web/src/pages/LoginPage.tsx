import { useState, type FormEvent } from 'react'
import { Link, Navigate, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { ErrorBox } from '../components/Feedback'
import { toLoadError, type LoadError } from '../useApi'

export function LoginPage() {
  const { auth, login } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<LoadError | null>(null)
  const [submitting, setSubmitting] = useState(false)

  const from = (location.state as { from?: string } | null)?.from || '/'
  const message = (location.state as { message?: string } | null)?.message

  if (auth) return <Navigate to={from} replace />

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setSubmitting(true)
    setError(null)
    try {
      await login(email, password)
      navigate(from, { replace: true })
    } catch (err) {
      setError(toLoadError(err))
      setSubmitting(false)
    }
  }

  return (
    <div id="login">
      <div className="loginheader">Supply Chain Management System</div>
      <div className="loginbody">
        <p>Please enter your user name and password to sign in.</p>
        {message && <div className="message">{message}</div>}
        <ErrorBox error={error} />
        <form onSubmit={handleSubmit}>
          <table className="form">
            <tbody>
              <tr>
                <td className="label">E-mail Address:</td>
                <td><input type="email" value={email} onChange={e => setEmail(e.target.value)} required autoFocus /></td>
              </tr>
              <tr>
                <td className="label">Password:</td>
                <td><input type="password" value={password} onChange={e => setPassword(e.target.value)} required /></td>
              </tr>
              <tr>
                <td></td>
                <td>
                  <button type="submit" disabled={submitting}>{submitting ? 'Signing in...' : 'Sign In'}</button>
                </td>
              </tr>
            </tbody>
          </table>
        </form>
        <p style={{ marginTop: 10 }}>
          Don't have an account? <Link to="/register">Request a new account</Link>
        </p>
      </div>
      <div className="loginfooter">This system is for authorized users only.</div>
    </div>
  )
}
