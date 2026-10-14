import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { loadAll, productsApi, suppliersApi } from '../api/endpoints'
import type { Product } from '../api/types'
import { useAuth } from '../auth/AuthContext'
import { ErrorBox, FlashMessage, Loading } from '../components/Feedback'
import { Pager } from '../components/Pager'
import { formatMoney, LOW_STOCK_LIMIT } from '../format'
import { toLoadError, useApi, type LoadError } from '../useApi'

export function ProductsPage() {
  const { isAdmin } = useAuth()
  const navigate = useNavigate()
  const [pageNumber, setPageNumber] = useState(1)
  const [actionError, setActionError] = useState<LoadError | null>(null)
  const [message, setMessage] = useState<string | null>(null)

  const { data, error, loading, reload } = useApi(async () => {
    const [products, suppliers] = await Promise.all([
      productsApi.list(pageNumber, 15),
      loadAll(suppliersApi.list)
    ])
    return { products, supplierNames: new Map(suppliers.map(s => [s.id, s.name])) }
  }, [pageNumber])

  const handleDelete = async (product: Product) => {
    if (!window.confirm(`Delete product "${product.name}"?\n\nThis action cannot be undone.`)) return
    setActionError(null)
    setMessage(null)
    try {
      const result = await productsApi.remove(product.id)
      setMessage(result.message)
      reload()
    } catch (err) {
      setActionError(toLoadError(err))
    }
  }

  return (
    <>
      <h1>Product List</h1>
      <FlashMessage />
      {message && <div className="message">{message}</div>}
      <ErrorBox error={error ?? actionError} />
      {isAdmin && (
        <div className="toolbar">
          <button type="button" onClick={() => navigate('/products/new')}>New Product</button>
        </div>
      )}
      {loading && <Loading />}
      {data && (
        <>
          <table className="grid">
            <thead>
              <tr>
                <th>Product Name</th>
                <th>Supplier</th>
                <th className="num">Quantity on Hand</th>
                <th className="num">Unit Price</th>
                {isAdmin && <th className="center" style={{ width: 110 }}>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {data.products.items.map((product, index) => (
                <tr key={product.id} className={index % 2 ? 'alt' : undefined}>
                  <td>{product.name}</td>
                  <td>{data.supplierNames.get(product.supplierId) ?? `#${product.supplierId}`}</td>
                  <td className={`num${product.quantity <= LOW_STOCK_LIMIT ? ' lowstock' : ''}`}>{product.quantity}</td>
                  <td className="num">{formatMoney(product.price)}</td>
                  {isAdmin && (
                    <td className="center">
                      <Link to={`/products/${product.id}/edit`}>Edit</Link>
                      {' | '}
                      <a href="#" onClick={e => { e.preventDefault(); handleDelete(product) }}>Delete</a>
                    </td>
                  )}
                </tr>
              ))}
              {data.products.items.length === 0 && (
                <tr><td colSpan={isAdmin ? 5 : 4} className="empty">No products found.</td></tr>
              )}
            </tbody>
          </table>
          <Pager
            pageNumber={data.products.pageNumber}
            totalPages={data.products.totalPages}
            totalCount={data.products.totalCount}
            onChange={setPageNumber}
          />
        </>
      )}
    </>
  )
}
