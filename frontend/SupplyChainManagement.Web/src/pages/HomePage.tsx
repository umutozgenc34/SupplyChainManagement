import { Link } from 'react-router-dom'
import { ordersApi, productsApi, suppliersApi } from '../api/endpoints'
import type { OrderStatus } from '../api/types'
import { useAuth } from '../auth/AuthContext'
import { ErrorBox, FlashMessage, Loading } from '../components/Feedback'
import { formatDate, formatMoney, LOW_STOCK_LIMIT, orderNumber } from '../format'
import { useApi } from '../useApi'

const statuses: OrderStatus[] = ['Pending', 'Approved', 'Shipped', 'Delivered', 'Canceled']

export function HomePage() {
  const { auth } = useAuth()
  const { data, error, loading } = useApi(async () => {
    const [products, suppliers, orders] = await Promise.all([
      productsApi.list(1, 100),
      suppliersApi.list(1, 1),
      ordersApi.list(1, 100)
    ])
    return { products, suppliers, orders }
  }, [])

  return (
    <>
      <h1>Home</h1>
      <FlashMessage />
      <p>Welcome, <b>{auth?.email}</b>. Use the navigation menu to access the system functions.</p>
      <ErrorBox error={error} />
      {loading && <Loading />}
      {data && (
        <>
          <h2>Summary</h2>
          <table className="details">
            <tbody>
              <tr><th>Products</th><td>{data.products.totalCount}</td></tr>
              <tr><th>Suppliers</th><td>{data.suppliers.totalCount}</td></tr>
              <tr><th>Orders</th><td>{data.orders.totalCount}</td></tr>
              {statuses.map(status => (
                <tr key={status}>
                  <th>&nbsp;&nbsp;{status}</th>
                  <td>{data.orders.items.filter(o => o.status === status).length}</td>
                </tr>
              ))}
            </tbody>
          </table>

          <h2>Recent Orders</h2>
          <table className="grid">
            <thead>
              <tr>
                <th>Order No.</th>
                <th>Order Date</th>
                <th>Status</th>
                <th className="num">Total</th>
              </tr>
            </thead>
            <tbody>
              {data.orders.items.slice(0, 5).map((order, index) => (
                <tr key={order.id} className={index % 2 ? 'alt' : undefined}>
                  <td><Link to={`/orders/${order.id}`}>{orderNumber(order.id)}</Link></td>
                  <td>{formatDate(order.orderDate)}</td>
                  <td><span className={`status-${order.status}`}>{order.status}</span></td>
                  <td className="num">{formatMoney(order.totalPrice)}</td>
                </tr>
              ))}
              {data.orders.items.length === 0 && (
                <tr><td colSpan={4} className="empty">No orders found.</td></tr>
              )}
            </tbody>
          </table>

          <h2>Low Stock Items (quantity {LOW_STOCK_LIMIT} or less)</h2>
          <table className="grid">
            <thead>
              <tr>
                <th>Product Name</th>
                <th className="num">Quantity on Hand</th>
              </tr>
            </thead>
            <tbody>
              {data.products.items.filter(p => p.quantity <= LOW_STOCK_LIMIT).map((product, index) => (
                <tr key={product.id} className={index % 2 ? 'alt' : undefined}>
                  <td>{product.name}</td>
                  <td className="num lowstock">{product.quantity}</td>
                </tr>
              ))}
              {data.products.items.every(p => p.quantity > LOW_STOCK_LIMIT) && (
                <tr><td colSpan={2} className="empty">All products are sufficiently stocked.</td></tr>
              )}
            </tbody>
          </table>
        </>
      )}
    </>
  )
}
