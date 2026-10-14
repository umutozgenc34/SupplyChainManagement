import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { ordersApi } from '../api/endpoints'
import { ErrorBox, FlashMessage, Loading } from '../components/Feedback'
import { Pager } from '../components/Pager'
import { formatDate, formatMoney, orderNumber } from '../format'
import { useApi } from '../useApi'

export function OrdersPage() {
  const navigate = useNavigate()
  const [pageNumber, setPageNumber] = useState(1)
  const { data, error, loading } = useApi(() => ordersApi.list(pageNumber, 15), [pageNumber])

  return (
    <>
      <h1>Order List</h1>
      <FlashMessage />
      <ErrorBox error={error} />
      <div className="toolbar">
        <button type="button" onClick={() => navigate('/orders/new')}>Enter New Order</button>
      </div>
      {loading && <Loading />}
      {data && (
        <>
          <table className="grid">
            <thead>
              <tr>
                <th>Order No.</th>
                <th>Order Date</th>
                <th className="num">Line Items</th>
                <th className="num">Total Amount</th>
                <th>Status</th>
                <th className="center" style={{ width: 70 }}>Details</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map((order, index) => (
                <tr key={order.id} className={index % 2 ? 'alt' : undefined}>
                  <td>{orderNumber(order.id)}</td>
                  <td>{formatDate(order.orderDate)}</td>
                  <td className="num">{order.orderItems.length}</td>
                  <td className="num">{formatMoney(order.totalPrice)}</td>
                  <td><span className={`status-${order.status}`}>{order.status}</span></td>
                  <td className="center"><Link to={`/orders/${order.id}`}>View</Link></td>
                </tr>
              ))}
              {data.items.length === 0 && (
                <tr><td colSpan={6} className="empty">No orders found.</td></tr>
              )}
            </tbody>
          </table>
          <Pager pageNumber={data.pageNumber} totalPages={data.totalPages} totalCount={data.totalCount} onChange={setPageNumber} />
        </>
      )}
    </>
  )
}
