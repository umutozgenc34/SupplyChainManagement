export interface ServiceResult<T = unknown> {
  success: boolean
  message: string
  data?: T | null
  errors?: string[]
}

export interface PagedResult<T> {
  items: T[]
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export type UserRole = 'User' | 'Admin'

export interface AuthInfo {
  accessToken: string
  expiresAt: string
  email: string
  role: UserRole
}

export interface Product {
  id: string
  name: string
  quantity: number
  price: number
  supplierId: number
}

export interface Supplier {
  id: number
  name: string
  contactEmail: string
  phone: string
}

export type OrderStatus = 'Pending' | 'Approved' | 'Shipped' | 'Delivered' | 'Canceled'

export interface OrderItem {
  productId: string
  quantity: number
  price: number
}

export interface Order {
  id: string
  orderDate: string
  status: OrderStatus
  orderItems: OrderItem[]
  totalPrice: number
}
