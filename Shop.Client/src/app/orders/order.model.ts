import { OrderStatus } from './order-status';
import { Currency } from '../shared/model/currency';

export interface Order {
    id: string;
    status: OrderStatus;
    orderCurrency: Currency | null;
    createdAt: string;
    items: OrderItem[];
}

export interface OrderItem {
    productId: string;
    productName: string;
    unitPriceAmount: number;
    currency: Currency;
    quantity: number;
    totalPrice: number;
}

export interface CreateOrderItem {
    productId: string;
    quantity: number;
}

export interface CreateOrder {
    items: CreateOrderItem[];
}
