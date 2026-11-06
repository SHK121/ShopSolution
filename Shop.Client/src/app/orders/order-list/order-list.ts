import { Component, OnInit, signal, inject  } from '@angular/core';
import { DatePipe } from '@angular/common';
import { PaginatedResult } from '../../shared/model/paginated-result';
import { PaginationRequest } from '../../shared/model/pagination-request';
import { Order } from '../order.model';
import { OrderService } from '../order.service';
import { OrderStatus } from '../order-status';
import { OrderStatusLabel } from '../order-status-label';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { ErrorService } from '../../shared/services/error.service';

@Component({
  selector: 'app-order-list',
  imports: [ DatePipe, RouterLink ],
  templateUrl: './order-list.html',
  styleUrl: './order-list.css',
  standalone: true
})
export class OrderListComponent implements OnInit {

  private orderService = inject(OrderService);
  private errorService = inject(ErrorService);
  orders = signal<PaginatedResult<Order>> ({
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 2,
    totalPages: 0
  });
  loading = signal(false);
  page = signal(1);
  pageSize = 2;

  ngOnInit() { this.loadOrders(); }
  loadOrders() {
    this.loading.set(true);
    const request: PaginationRequest = {
      page: this.page(),
      pageSize: this.pageSize
    };

    this.orderService.getOrders(request)
      .pipe(
        finalize(() => {
          this.loading.set(false);
        })
      )
      .subscribe({
          next: result => {
          console.log('orders', result);

          this.orders.set(result);
        }, error: err => {
          this.errorService.setHttpError(err);
          console.error('Failed to load orders', err);
        }
      });
  }

  nextPage() {
    console.log('currentPage', this.page());
    console.log('totalPages', this.orders().totalPages);
    if (this.page() >= this.orders().totalPages) {
      return;
    }

    this.page.update(page => page + 1);
    this.loadOrders();
  }

  previousPage() {
    if (this.page() <= 1) {
      return;
    }

    this.page.update(page => page - 1);
    this.loadOrders();
  }

  getOrderStatusLabel(status: OrderStatus): string {
    return OrderStatusLabel[status];
  }
}
