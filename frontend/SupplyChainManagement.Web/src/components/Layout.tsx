import type { MouseEvent } from 'react'
import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { formatLongDate } from '../format'

const menu = [
  { to: '/', label: 'Home', end: true },
  { to: '/orders', label: 'Orders', end: false },
  { to: '/products', label: 'Products', end: false },
  { to: '/suppliers', label: 'Suppliers', end: false }
]

export function Layout() {
  const { auth, isAdmin, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = (e: MouseEvent) => {
    e.preventDefault()
    if (!window.confirm('Are you sure you want to log off?')) return
    logout()
    navigate('/login')
  }

  return (
    <div id="page">
      <div id="header">
        <div className="title">Supply Chain Management System</div>
        <div className="subtitle">Procurement, Inventory and Order Processing</div>
      </div>

      <div id="menubar">
        {menu.map((item, index) => (
          <span key={item.to}>
            {index > 0 && <span className="sep">|</span>}
            <NavLink to={item.to} end={item.end} className={({ isActive }) => (isActive ? 'active' : undefined)}>
              {item.label}
            </NavLink>
          </span>
        ))}
      </div>

      <div id="statusbar">
        <span className="right">
          <a href="#" onClick={handleLogout}>Log Off</a>
        </span>
        Logged in as: <b>{auth?.email}</b> ({auth?.role}) &nbsp;|&nbsp; {formatLongDate(new Date())}
      </div>

      <table id="layout">
        <tbody>
          <tr>
            <td id="sidebar">
              <div className="sidebox">
                <div className="boxtitle">Order Processing</div>
                <ul>
                  <li><Link to="/orders">Order List</Link></li>
                  <li><Link to="/orders/new">Enter New Order</Link></li>
                </ul>
              </div>
              <div className="sidebox">
                <div className="boxtitle">Inventory</div>
                <ul>
                  <li><Link to="/products">Product List</Link></li>
                  {isAdmin && <li><Link to="/products/new">Add Product</Link></li>}
                </ul>
              </div>
              <div className="sidebox">
                <div className="boxtitle">Procurement</div>
                <ul>
                  <li><Link to="/suppliers">Supplier List</Link></li>
                  {isAdmin && <li><Link to="/suppliers/new">Add Supplier</Link></li>}
                </ul>
              </div>
              <div className="sidebox">
                <div className="boxtitle">System Information</div>
                <div className="boxbody">
                  Version 1.0.0<br />
                  Access level: {isAdmin ? 'Administrator' : 'Standard User'}
                </div>
              </div>
            </td>
            <td id="content">
              <Outlet />
            </td>
          </tr>
        </tbody>
      </table>

      <div id="footer">
        Copyright &copy; {new Date().getFullYear()} Supply Chain Management System. All rights reserved.
      </div>
    </div>
  )
}
