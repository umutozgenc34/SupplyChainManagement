import type { OrderStatus } from './api/types'

export function formatMoney(value: number) {
  return value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

export function formatDate(value: string) {
  const iso = /Z$|[+-]\d{2}:\d{2}$/.test(value) ? value : `${value}Z`
  return new Date(iso).toLocaleString('en-US', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    hour12: false
  })
}

export function formatLongDate(date: Date) {
  return date.toLocaleDateString('en-US', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })
}

export function orderNumber(id: string) {
  return id.replace(/-/g, '').slice(0, 8).toUpperCase()
}

export const orderTransitions: Record<OrderStatus, OrderStatus[]> = {
  Pending: ['Approved', 'Canceled'],
  Approved: ['Shipped', 'Canceled'],
  Shipped: ['Delivered'],
  Delivered: [],
  Canceled: []
}

export const statusActionLabels: Record<OrderStatus, string> = {
  Pending: 'Reopen',
  Approved: 'Approve',
  Shipped: 'Mark as Shipped',
  Delivered: 'Mark as Delivered',
  Canceled: 'Cancel Order'
}

export const LOW_STOCK_LIMIT = 5
