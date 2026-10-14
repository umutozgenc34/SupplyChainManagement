import { useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { loadAll, ordersApi, productsApi } from '../api/endpoints'
import type { OrderStatus } from '../api/types'
import { useAuth } from '../auth/AuthContext'
import { ErrorBox, FlashMessage, Loading } from '../components/Feedback'
import { formatDate, formatMoney, orderNumber, orderTransitions, statusActionLabels } from '../format'
import { toLoadError, useApi, type LoadError } from '../useApi'

export function OrderDetailPage() {
  const { id = '' } = useParams()
  const { isAdmin } = useAuth()
  const navigate = useNavigate()
  const [actionError, setActionError] = useState<LoadError | null>(null)
  const [message, setMessage] = useState<string | null>(null)
  const [busy, setBusy] = useState(false)

  const { data, error, loading, reload } = useApi(async () => {
    const [order, products] = await Promise.all([ordersApi.get(id), loadAll(productsApi.list)])
    return { order, productNames: new Map(products.map(p => [p.id, p.name])) }
  }, [id])

  const changeStatus = async (status: OrderStatus) => {
    if (!window.confirm(`Change order status to "${status}"?`)) return
    setBusy(true)
    setActionError(null)
    setMessage(null)
    try {
      const result = await ordersApi.changeStatus(id, status)
      setMessage(result.message)
      reload()
    } catch (err) {
      setActionError(toLoadError(err))
    } finally {
      setBusy(false)
    }
  }

  const handleDelete = async () => {
    if (!window.confirm('Delete this order?\n\nThis action cannot be undone.')) return
    setBusy(true)
    setActionError(null)
    try {
      const result = await ordersApi.remove(id)
      navigate('/orders', { state: { message: result.message } })
    } catch (err) {
      setActionError(toLoadError(err))
      setBusy(false)
    }
  }

  const order = data?.order

  return (
    <>
      <h1>Order Details{order ? ` - ${orderNumber(order.id)}` : ''}</h1>
      <FlashMessage />
      {message && <div className="message">{message}</div>}
      <ErrorBox error={error ?? actionError} />
      {loading && !data && <Loading />}
      {order && data && (
        <>
          <div className="toolbar">
            <button type="button" onClick={() => navigate('/orders')}>Back to List</button>
            {order.status === 'Pending' && (
              <button type="button" disabled={busy} onClick={() => navigate(`/orders/${order.id}/edit`)}>Edit Items</button>
            )}
            {isAdmin && orderTransitions[order.status].map(status => (
              <button key={status} type="button" disabled={busy} onClick={() => changeStatus(status)}>
                {statusActionLabels[status]}
              </button>
            ))}
            {(order.status === 'Pending' || order.status === 'Canceled') && (
              <button type="button" disabled={busy} onClick={handleDelete}>Delete Order</button>
            )}
          </div>

          <h2>General Information</h2>
          <table className="details">
            <tbody>
              <tr><th>Order No.</th><td>{orderNumber(order.id)}</td></tr>
              <tr><th>Reference</th><td className="muted">{order.id}</td></tr>
              <tr><th>Order Date</th><td>{formatDate(order.orderDate)}</td></tr>
              <tr><th>Status</th><td><span className={`status-${order.status}`}>{order.status}</span></td></tr>
              <tr><th>Total Amount</th><td><b>{formatMoney(order.totalPrice)}</b></td></tr>
            </tbody>
          </table>

          <h2>Line Items</h2>
          <table className="grid">
            <thead>
              <tr>
                <th style={{ width: 40 }}>#</th>
                <th>Product</th>
                <th className="num">Quantity</th>
                <th className="num">Unit Price</th>
                <th className="num">Line Total</th>
              </tr>
            </thead>
            <tbody>
              {order.orderItems.map((item, index) => (
                <tr key={item.productId} className={index % 2 ? 'alt' : undefined}>
                  <td>{index + 1}</td>
                  <td>{data.productNames.get(item.productId) ?? <span className="muted">(deleted product)</span>}</td>
                  <td className="num">{item.quantity}</td>
                  <td className="num">{formatMoney(item.price)}</td>
                  <td className="num">{formatMoney(item.price * item.quantity)}</td>
                </tr>
              ))}
            </tbody>
            <tfoot>
              <tr>
                <td colSpan={4} className="num">Order Total:</td>
                <td className="num">{formatMoney(order.totalPrice)}</td>
              </tr>
            </tfoot>
          </table>
          {!isAdmin && orderTransitions[order.status].length > 0 && (
            <p className="muted">Status changes are performed by administrators.</p>
          )}
        </>
      )}
    </>
  )
}
