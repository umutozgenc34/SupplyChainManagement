import { Navigate, Route, Routes } from 'react-router-dom'
import { Layout } from './components/Layout'
import { RequireAdmin, RequireAuth } from './components/Guards'
import { HomePage } from './pages/HomePage'
import { LoginPage } from './pages/LoginPage'
import { OrderDetailPage } from './pages/OrderDetailPage'
import { OrderFormPage } from './pages/OrderFormPage'
import { OrdersPage } from './pages/OrdersPage'
import { ProductFormPage } from './pages/ProductFormPage'
import { ProductsPage } from './pages/ProductsPage'
import { RegisterPage } from './pages/RegisterPage'
import { SupplierFormPage } from './pages/SupplierFormPage'
import { SuppliersPage } from './pages/SuppliersPage'

export function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route element={<RequireAuth><Layout /></RequireAuth>}>
        <Route index element={<HomePage />} />
        <Route path="orders" element={<OrdersPage />} />
        <Route path="orders/new" element={<OrderFormPage />} />
        <Route path="orders/:id" element={<OrderDetailPage />} />
        <Route path="orders/:id/edit" element={<OrderFormPage />} />
        <Route path="products" element={<ProductsPage />} />
        <Route path="products/new" element={<RequireAdmin><ProductFormPage /></RequireAdmin>} />
        <Route path="products/:id/edit" element={<RequireAdmin><ProductFormPage /></RequireAdmin>} />
        <Route path="suppliers" element={<SuppliersPage />} />
        <Route path="suppliers/new" element={<RequireAdmin><SupplierFormPage /></RequireAdmin>} />
        <Route path="suppliers/:id/edit" element={<RequireAdmin><SupplierFormPage /></RequireAdmin>} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}
