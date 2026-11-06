import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OrderAction } from './order-action';

import { Order, CreateOrder } from './order.model';
import { PaginatedResult } from '../shared/model/paginated-result';
import { PaginationRequest } from '../shared/model/pagination-request';

@Injectable({
    providedIn: 'root'
})
export class OrderService {
    private apiUrl = 'http://localhost:8081/api/orders';
//'https://localhost:7185/api/orders';

    constructor(private http: HttpClient) {}

    getOrder(id: string): Observable<Order> {
        return this.http.get<Order>(`${this.apiUrl}/${id}`);
    }

    getOrders(request: PaginationRequest): Observable<PaginatedResult<Order>> {
        const params = new HttpParams()
            .set('page', request.page)
            .set('pageSize', request.pageSize);

        return this.http.get<PaginatedResult<Order>>(
            this.apiUrl,
            { params }
        );
    }

    performAction(id: string, action: OrderAction): Observable<void> {
        return this.http.post<void>(`${this.apiUrl}/${id}/${action}`,{});
    }

    submitOrder(id: string) {
        return this.performAction(id, 'submit');
    }

    payOrder(id: string) {
        return this.performAction(id, 'pay');
    }

    cancelOrder(id: string) {
        return this.performAction(id, 'cancel');
    }

    shipOrder(id: string) {
        return this.performAction(id, 'ship');
    }

    createOrder(order: CreateOrder): Observable<string> {
        return this.http.post<string>(this.apiUrl, order);
    }
}