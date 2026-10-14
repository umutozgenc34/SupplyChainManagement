import { useState, type FormEvent } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { ErrorBox } from '../components/Feedback'
import { toLoadError, type LoadError } from '../useApi'

export function RegisterPage() {
  const { auth, register } = useAuth()
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [error, setError] = useState<LoadError | null>(null)
  const [submitting, setSubmitting] = useState(false)

  if (auth) return <Navigate to="/" replace />

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    if (password !== confirmPassword) {
      setError({ message: 'The passwords you entered do not match.', errors: [] })
      return
    }
    setSubmitting(true)
    setError(null)
    try {
      await register(email, password)
      navigate('/', { replace: true, state: { message: 'Your account has been created successfully.' } })
    } catch (err) {
      setError(toLoadError(err))
      setSubmitting(false)
    }
  }

  return (
    <div id="login">
      <div className="loginheader">New Account Registration</div>
      <div className="loginbody">
        <p>Complete the form below to create a standard user account.</p>
        <ErrorBox error={error} />
        <form onSubmit={handleSubmit}>
          <table className="form">
            <tbody>
              <tr>
                <td className="label">E-mail Address: <span className="required">*</span></td>
                <td><input type="email" value={email} onChange={e => setEmail(e.target.value)} required autoFocus /></td>
              </tr>
              <tr>
                <td className="label">Password: <span className="required">*</span></td>
                <td><input type="password" value={password} onChange={e => setPassword(e.target.value)} required /></td>
              </tr>
              <tr>
                <td className="label">Confirm Password: <span className="required">*</span></td>
                <td><input type="password" value={confirmPassword} onChange={e => setConfirmPassword(e.target.value)} required /></td>
              </tr>
              <tr>
                <td></td>
                <td className="muted">At least 8 characters, including an uppercase letter, a lowercase letter and a digit.</td>
              </tr>
              <tr>
                <td></td>
                <td>
                  <button type="submit" disabled={submitting}>{submitting ? 'Please wait...' : 'Register'}</button>
                </td>
              </tr>
            </tbody>
          </table>
        </form>
        <p style={{ marginTop: 10 }}>
          Already registered? <Link to="/login">Return to sign in</Link>
        </p>
      </div>
      <div className="loginfooter">This system is for authorized users only.</div>
    </div>
  )
}
