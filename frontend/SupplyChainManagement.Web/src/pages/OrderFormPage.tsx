import { useEffect, useState, type FormEvent } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { loadAll, ordersApi, productsApi } from '../api/endpoints'
import { ErrorBox, Loading } from '../components/Feedback'
import { formatMoney, orderNumber } from '../format'
import { toLoadError, useApi, type LoadError } from '../useApi'

interface Line {
  key: number
  productId: string
  quantity: string
}

let nextKey = 1
const newLine = (productId = '', quantity = '1'): Line => ({ key: nextKey++, productId, quantity })

export function OrderFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [lines, setLines] = useState<Line[]>(() => [newLine()])
  const [saveError, setSaveError] = useState<LoadError | null>(null)
  const [saving, setSaving] = useState(false)

  const { data, error, loading } = useApi(async () => {
    const [products, order] = await Promise.all([
      loadAll(productsApi.list),
      id ? ordersApi.get(id) : Promise.resolve(null)
    ])
    return { products, order }
  }, [id])

  useEffect(() => {
    if (data?.order) {
      setLines(data.order.orderItems.map(item => newLine(item.productId, String(item.quantity))))
    }
  }, [data])

  const products = data?.products ?? []
  const productById = new Map(products.map(p => [p.id, p]))
  const reserved = new Map((data?.order?.orderItems ?? []).map(item => [item.productId, item.quantity]))
  const available = (productId: string) => (productById.get(productId)?.quantity ?? 0) + (reserved.get(productId) ?? 0)

  const total = lines.reduce((sum, line) => {
    const product = productById.get(line.productId)
    return product ? sum + product.price * (Number(line.quantity) || 0) : sum
  }, 0)

  const updateLine = (key: number, changes: Partial<Line>) =>
    setLines(current => current.map(line => (line.key === key ? { ...line, ...changes } : line)))

  const removeLine = (key: number) => setLines(current => current.filter(line => line.key !== key))

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    const items = lines
      .filter(line => line.productId)
      .map(line => ({ productId: line.productId, quantity: Number(line.quantity) }))
    if (items.length === 0) {
      setSaveError({ message: 'Please add at least one product to the order.', errors: [] })
      return
    }
    setSaving(true)
    setSaveError(null)
    try {
      const result = id ? await ordersApi.update(id, items) : await ordersApi.create(items)
      navigate(`/orders/${result.data?.id ?? id}`, { state: { message: result.message } })
    } catch (err) {
      setSaveError(toLoadError(err))
      setSaving(false)
    }
  }

  return (
    <>
      <h1>{id ? `Edit Order${data?.order ? ` - ${orderNumber(data.order.id)}` : ''}` : 'Enter New Order'}</h1>
      <ErrorBox error={error ?? saveError} />
      {loading && <Loading />}
      {data && data.order && data.order.status !== 'Pending' && (
        <div className="error">Only pending orders can be edited. Current status: <b>{data.order.status}</b>.</div>
      )}
      {data && (!data.order || data.order.status === 'Pending') && (
        <form onSubmit={handleSubmit}>
          <fieldset>
            <legend>Order Lines</legend>
            <table className="grid">
              <thead>
                <tr>
                  <th style={{ width: 30 }}>#</th>
                  <th>Product</th>
                  <th className="num">Available</th>
                  <th className="num">Unit Price</th>
                  <th className="num">Quantity</th>
                  <th className="num">Line Total</th>
                  <th className="center" style={{ width: 60 }}></th>
                </tr>
              </thead>
              <tbody>
                {lines.map((line, index) => {
                  const product = productById.get(line.productId)
                  return (
                    <tr key={line.key} className={index % 2 ? 'alt' : undefined}>
                      <td>{index + 1}</td>
                      <td>
                        <select value={line.productId} onChange={e => updateLine(line.key, { productId: e.target.value })}>
                          <option value="">-- Select Product --</option>
                          {products.map(p => <option key={p.id} value={p.id}>{p.name}</option>)}
                        </select>
                      </td>
                      <td className="num">{product ? available(product.id) : ''}</td>
                      <td className="num">{product ? formatMoney(product.price) : ''}</td>
                      <td className="num">
                        <input
                          type="number"
                          min={1}
                          step={1}
                          value={line.quantity}
                          onChange={e => updateLine(line.key, { quantity: e.target.value })}
                          required={Boolean(line.productId)}
                        />
                      </td>
                      <td className="num">{product ? formatMoney(product.price * (Number(line.quantity) || 0)) : ''}</td>
                      <td className="center">
                        {lines.length > 1 && (
                          <a href="#" onClick={e => { e.preventDefault(); removeLine(line.key) }}>Remove</a>
                        )}
                      </td>
                    </tr>
                  )
                })}
              </tbody>
              <tfoot>
                <tr>
                  <td colSpan={5} className="num">Estimated Total:</td>
                  <td className="num">{formatMoney(total)}</td>
                  <td></td>
                </tr>
              </tfoot>
            </table>
            <button type="button" onClick={() => setLines(current => [...current, newLine()])}>Add Line</button>
          </fieldset>
          <div className="muted">Prices are taken from the product catalog at the time the order is saved.</div>
          <div className="buttons">
            <button type="submit" disabled={saving}>{saving ? 'Saving...' : id ? 'Save Changes' : 'Submit Order'}</button>
            <button type="button" onClick={() => navigate(id ? `/orders/${id}` : '/orders')}>Cancel</button>
          </div>
        </form>
      )}
    </>
  )
}
