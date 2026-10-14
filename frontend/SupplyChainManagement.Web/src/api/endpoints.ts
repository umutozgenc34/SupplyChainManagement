import { api } from './client'
import type { AuthInfo, Order, OrderStatus, PagedResult, Product, Supplier } from './types'

export interface OrderLineInput {
  productId: string
  quantity: number
}

export interface ProductInput {
  name: string
  quantity: number
  price: number
  supplierId: number
}

export interface SupplierInput {
  name: string
  contactEmail: string
  phone: string
}

const page = (pageNumber: number, pageSize: number) => `?pageNumber=${pageNumber}&pageSize=${pageSize}`

export const authApi = {
  login: (email: string, password: string) => api.post<AuthInfo>('/auth/login', { email, password }),
  register: (email: string, password: string) => api.post<AuthInfo>('/auth/register', { email, password })
}

export const productsApi = {
  list: (pageNumber = 1, pageSize = 10) => api.get<PagedResult<Product>>(`/products${page(pageNumber, pageSize)}`),
  get: (id: string) => api.get<Product>(`/products/${id}`),
  create: (input: ProductInput) => api.post<Product>('/products', input),
  update: (id: string, input: ProductInput) => api.put<Product>('/products', { id, ...input }),
  remove: (id: string) => api.delete(`/products/${id}`)
}

export const suppliersApi = {
  list: (pageNumber = 1, pageSize = 10) => api.get<PagedResult<Supplier>>(`/suppliers${page(pageNumber, pageSize)}`),
  get: (id: number) => api.get<Supplier>(`/suppliers/${id}`),
  create: (input: SupplierInput) => api.post<Supplier>('/suppliers', input),
  update: (id: number, input: SupplierInput) => api.put<Supplier>('/suppliers', { id, ...input }),
  remove: (id: number) => api.delete(`/suppliers/${id}`)
}

export const ordersApi = {
  list: (pageNumber = 1, pageSize = 10) => api.get<PagedResult<Order>>(`/orders${page(pageNumber, pageSize)}`),
  get: (id: string) => api.get<Order>(`/orders/${id}`),
  create: (orderItems: OrderLineInput[]) => api.post<Order>('/orders', { orderItems }),
  update: (id: string, orderItems: OrderLineInput[]) => api.put<Order>('/orders', { id, orderItems }),
  changeStatus: (id: string, status: OrderStatus) => api.patch<Order>(`/orders/${id}/status`, { status }),
  remove: (id: string) => api.delete(`/orders/${id}`)
}

export async function loadAll<T>(loader: (pageNumber: number, pageSize: number) => Promise<PagedResult<T>>): Promise<T[]> {
  const first = await loader(1, 100)
  const items = [...first.items]
  for (let pageNumber = 2; pageNumber <= first.totalPages; pageNumber++) {
    const next = await loader(pageNumber, 100)
    items.push(...next.items)
  }
  return items
}
