import { useEffect, useState, type FormEvent } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { suppliersApi } from '../api/endpoints'
import { ErrorBox, Loading } from '../components/Feedback'
import { toLoadError, useApi, type LoadError } from '../useApi'

export function SupplierFormPage() {
  const { id } = useParams()
  const supplierId = id ? Number(id) : null
  const navigate = useNavigate()
  const [name, setName] = useState('')
  const [contactEmail, setContactEmail] = useState('')
  const [phone, setPhone] = useState('')
  const [saveError, setSaveError] = useState<LoadError | null>(null)
  const [saving, setSaving] = useState(false)

  const { data, error, loading } = useApi(
    () => (supplierId ? suppliersApi.get(supplierId) : Promise.resolve(null)),
    [supplierId]
  )

  useEffect(() => {
    if (!data) return
    setName(data.name)
    setContactEmail(data.contactEmail)
    setPhone(data.phone)
  }, [data])

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setSaving(true)
    setSaveError(null)
    const input = { name, contactEmail, phone }
    try {
      const result = supplierId ? await suppliersApi.update(supplierId, input) : await suppliersApi.create(input)
      navigate('/suppliers', { state: { message: result.message } })
    } catch (err) {
      setSaveError(toLoadError(err))
      setSaving(false)
    }
  }

  return (
    <>
      <h1>{supplierId ? 'Edit Supplier' : 'New Supplier'}</h1>
      <ErrorBox error={error ?? saveError} />
      {loading && <Loading />}
      {!loading && (
        <form onSubmit={handleSubmit}>
          <fieldset>
            <legend>Supplier Details</legend>
            <table className="form">
              <tbody>
                <tr>
                  <td className="label">Supplier Name: <span className="required">*</span></td>
                  <td><input type="text" value={name} maxLength={100} onChange={e => setName(e.target.value)} required /></td>
                </tr>
                <tr>
                  <td className="label">Contact E-mail: <span className="required">*</span></td>
                  <td><input type="email" value={contactEmail} maxLength={255} onChange={e => setContactEmail(e.target.value)} required /></td>
                </tr>
                <tr>
                  <td className="label">Phone: <span className="required">*</span></td>
                  <td>
                    <input type="text" value={phone} maxLength={11} onChange={e => setPhone(e.target.value)} required />
                    <span className="muted"> (11 digits, e.g. 05551234567)</span>
                  </td>
                </tr>
              </tbody>
            </table>
          </fieldset>
          <div className="muted"><span className="required">*</span> Required field</div>
          <div className="buttons">
            <button type="submit" disabled={saving}>{saving ? 'Saving...' : 'Save'}</button>
            <button type="button" onClick={() => navigate('/suppliers')}>Cancel</button>
          </div>
        </form>
      )}
    </>
  )
}
