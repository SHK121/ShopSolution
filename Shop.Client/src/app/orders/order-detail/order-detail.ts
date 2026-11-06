import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { OrderService } from '../order.service';
import { Order } from '../order.model';
import { OrderStatus } from '../order-status';
import { OrderStatusLabel } from '../order-status-label';
import { Currency } from '../../shared/model/currency';
import { CurrencyLabel } from '../../shared/model/currency-label';
import { ErrorService } from '../../shared/services/error.service';
import { DatePipe } from '@angular/common';
import { finalize, switchMap, tap, of } from 'rxjs';
import { OrderAction } from '../order-action';
import { ProductService } from '../../products/product.service';
import { Product } from '../../products/product.model';
//import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-order-detail',
  imports: [ DatePipe, RouterLink ],
  templateUrl: './order-detail.html',
  styleUrl: './order-detail.css',
  standalone: true
})
export class OrderDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private orderService = inject(OrderService);
  private errorService = inject(ErrorService);
  private productService = inject(ProductService);

  order = signal<Order | null>(null);
  loading = signal(true);
  OrderStatus = OrderStatus;
  productsById = signal<Record<string, Product>>({});

  /*loading: boolean = true;

  order: Order = {
    id: '',
    status: OrderStatus.Draft,
    orderCurrency: null,
    createdAt: '',
    items : []
  };


  constructor(private route: ActivatedRoute,
    private orderService: OrderService,
    private cdr: ChangeDetectorRef
  ) {
    //ActivatedRoute :"현재 내가 어떤 URL에 있는지 알려주는 Angular 객체"
  }*/

  ngOnInit() {
    this.errorService.clearError();
    const id = this.route.snapshot.paramMap.get('id');
    if (!id){
      this.loading.set(false);
      return;
    }
    console.log('Order ID:', id);
    this.getOrder(id);

   /* this.errorService.setHttpError(
      new HttpErrorResponse({
        status: 409,
        error: {
          code: 'CONCURRENCY_ERROR',
          message: 'The resource was modified by another request. Please try again.'
        }
      })
    );*/
  }
  

  getOrder(id: string) {
    /*this.loading = true;

    this.orderService.getOrder(id)
      .subscribe(result => {
        this.order = result;
        this.loading = false;
        this.cdr.detectChanges();
      });*/

    this.loading.set(true);
    this.productsById.set({});

    this.orderService.getOrder(id)
      .pipe(
        tap(order => {
          this.order.set(order);
        }),

        switchMap(order => {
          const productIds = [
            ...new Set(order.items.map(item => item.productId))
          ];

          if (productIds.length === 0) {
            return of([] as Product[]);
          }

          return this.productService.getProductsByIds(productIds);
        }),

        finalize(() => {
          this.loading.set(false);
        })
        )
      .subscribe({
        next: products => {
          const productsMap = Object.fromEntries(
            products.map(product => [product.id, product])
          );

          this.productsById.set(productsMap);
        },
        error: err => {
          this.errorService.setHttpError(err);
        }
      });
      

   /* this.orderService.getOrder(id)
      .pipe(finalize(() => {
        this.loading.set(false);
      }))
      .subscribe({
        next: order => {
          console.log('order', order);
          this.order.set(order);
        },
        error: err => {
          console.error('Failed to load order', err);
        }
      });*/
  }

  getOrderStatusLabel(status: OrderStatus): string {
    return OrderStatusLabel[status];
  }

  getOrderCurrencyLabel(currency: Currency | null): string {
    if (!currency) {
      return '';
    }
    return CurrencyLabel[currency];
  }

  getTotalPrice(): number {
    return this.order()!.items.reduce(
      (sum, item) => sum + item.totalPrice,
      0
    );
  }

  executeOrderAction(action: OrderAction) {
    const orderId = this.order()?.id;

    if (!orderId) {
      return;
    }
    this.errorService.clearError();

    this.orderService.performAction(orderId, action)
      .subscribe({
        next: () => {
          this.getOrder(orderId);
        },
        error: err => {
          this.errorService.setHttpError(err);
        }
      });
  }

  submitOrder() {
    this.executeOrderAction('submit');
  }

  payOrder() {
    this.executeOrderAction('pay');
  }

  cancelOrder() {
    this.executeOrderAction('cancel');
  }

  shipOrder() {
    this.executeOrderAction('ship');
  }
}
