import { OrderStatus } from './order-status';

export const OrderStatusLabel: Record<OrderStatus, string> = {
    [OrderStatus.Draft]: 'Draft',
    [OrderStatus.Submitted]: 'Submitted',
    [OrderStatus.Paid]: 'Paid',
    [OrderStatus.Shipped]: 'Shipped',
    [OrderStatus.Cancelled]: 'Cancelled'
};
