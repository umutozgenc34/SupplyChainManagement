import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { suppliersApi } from '../api/endpoints'
import type { Supplier } from '../api/types'
import { useAuth } from '../auth/AuthContext'
import { ErrorBox, FlashMessage, Loading } from '../components/Feedback'
import { Pager } from '../components/Pager'
import { toLoadError, useApi, type LoadError } from '../useApi'

export function SuppliersPage() {
  const { isAdmin } = useAuth()
  const navigate = useNavigate()
  const [pageNumber, setPageNumber] = useState(1)
  const [actionError, setActionError] = useState<LoadError | null>(null)
  const [message, setMessage] = useState<string | null>(null)

  const { data, error, loading, reload } = useApi(() => suppliersApi.list(pageNumber, 15), [pageNumber])

  const handleDelete = async (supplier: Supplier) => {
    if (!window.confirm(`Delete supplier "${supplier.name}"?\n\nAll products of this supplier will also be deleted.`)) return
    setActionError(null)
    setMessage(null)
    try {
      const result = await suppliersApi.remove(supplier.id)
      setMessage(result.message)
      reload()
    } catch (err) {
      setActionError(toLoadError(err))
    }
  }

  return (
    <>
      <h1>Supplier List</h1>
      <FlashMessage />
      {message && <div className="message">{message}</div>}
      <ErrorBox error={error ?? actionError} />
      {isAdmin && (
        <div className="toolbar">
          <button type="button" onClick={() => navigate('/suppliers/new')}>New Supplier</button>
        </div>
      )}
      {loading && <Loading />}
      {data && (
        <>
          <table className="grid">
            <thead>
              <tr>
                <th style={{ width: 60 }}>ID</th>
                <th>Supplier Name</th>
                <th>Contact E-mail</th>
                <th>Phone</th>
                {isAdmin && <th className="center" style={{ width: 110 }}>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {data.items.map((supplier, index) => (
                <tr key={supplier.id} className={index % 2 ? 'alt' : undefined}>
                  <td>{supplier.id}</td>
                  <td>{supplier.name}</td>
                  <td><a href={`mailto:${supplier.contactEmail}`}>{supplier.contactEmail}</a></td>
                  <td>{supplier.phone}</td>
                  {isAdmin && (
                    <td className="center">
                      <Link to={`/suppliers/${supplier.id}/edit`}>Edit</Link>
                      {' | '}
                      <a href="#" onClick={e => { e.preventDefault(); handleDelete(supplier) }}>Delete</a>
                    </td>
                  )}
                </tr>
              ))}
              {data.items.length === 0 && (
                <tr><td colSpan={isAdmin ? 5 : 4} className="empty">No suppliers found.</td></tr>
              )}
            </tbody>
          </table>
          <Pager pageNumber={data.pageNumber} totalPages={data.totalPages} totalCount={data.totalCount} onChange={setPageNumber} />
        </>
      )}
    </>
  )
}
