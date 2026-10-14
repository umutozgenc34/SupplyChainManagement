import { useEffect, useState, type FormEvent } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { loadAll, productsApi, suppliersApi } from '../api/endpoints'
import { ErrorBox, Loading } from '../components/Feedback'
import { toLoadError, useApi, type LoadError } from '../useApi'

export function ProductFormPage() {
  const { id } = useParams()
  const isEdit = Boolean(id)
  const navigate = useNavigate()
  const [name, setName] = useState('')
  const [quantity, setQuantity] = useState('0')
  const [price, setPrice] = useState('')
  const [supplierId, setSupplierId] = useState('')
  const [saveError, setSaveError] = useState<LoadError | null>(null)
  const [saving, setSaving] = useState(false)

  const { data, error, loading } = useApi(async () => {
    const [suppliers, product] = await Promise.all([
      loadAll(suppliersApi.list),
      id ? productsApi.get(id) : Promise.resolve(null)
    ])
    return { suppliers, product }
  }, [id])

  useEffect(() => {
    if (!data?.product) return
    setName(data.product.name)
    setQuantity(String(data.product.quantity))
    setPrice(String(data.product.price))
    setSupplierId(String(data.product.supplierId))
  }, [data])

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setSaving(true)
    setSaveError(null)
    const input = { name, quantity: Number(quantity), price: Number(price), supplierId: Number(supplierId) }
    try {
      const result = id ? await productsApi.update(id, input) : await productsApi.create(input)
      navigate('/products', { state: { message: result.message } })
    } catch (err) {
      setSaveError(toLoadError(err))
      setSaving(false)
    }
  }

  return (
    <>
      <h1>{isEdit ? 'Edit Product' : 'New Product'}</h1>
      <ErrorBox error={error ?? saveError} />
      {loading && <Loading />}
      {data && (
        <form onSubmit={handleSubmit}>
          <fieldset>
            <legend>Product Details</legend>
            <table className="form">
              <tbody>
                <tr>
                  <td className="label">Product Name: <span className="required">*</span></td>
                  <td><input type="text" value={name} maxLength={100} onChange={e => setName(e.target.value)} required /></td>
                </tr>
                <tr>
                  <td className="label">Supplier: <span className="required">*</span></td>
                  <td>
                    <select value={supplierId} onChange={e => setSupplierId(e.target.value)} required>
                      <option value="">-- Select Supplier --</option>
                      {data.suppliers.map(s => <option key={s.id} value={s.id}>{s.name}</option>)}
                    </select>
                  </td>
                </tr>
                <tr>
                  <td className="label">Quantity on Hand: <span className="required">*</span></td>
                  <td><input type="number" min={0} step={1} value={quantity} onChange={e => setQuantity(e.target.value)} required /></td>
                </tr>
                <tr>
                  <td className="label">Unit Price: <span className="required">*</span></td>
                  <td><input type="number" min={0.01} step={0.01} value={price} onChange={e => setPrice(e.target.value)} required /></td>
                </tr>
              </tbody>
            </table>
          </fieldset>
          <div className="muted"><span className="required">*</span> Required field</div>
          <div className="buttons">
            <button type="submit" disabled={saving}>{saving ? 'Saving...' : 'Save'}</button>
            <button type="button" onClick={() => navigate('/products')}>Cancel</button>
          </div>
        </form>
      )}
    </>
  )
}
