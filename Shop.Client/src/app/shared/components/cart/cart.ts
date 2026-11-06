import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CartService } from '../../services/cart.service';
import { Product } from '../../../products/product.model';
import { ProductService } from '../../../products/product.service';
import { OrderService } from '../../../orders/order.service';
import { CreateOrder } from '../../../orders/order.model';
import { ErrorService } from '../../services/error.service';
import { finalize } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-cart',
  imports: [ ],
  templateUrl: './cart.html',
  styleUrl: './cart.css',
  standalone: true
})
export class CartComponent implements OnInit {
  private cartService = inject(CartService);
  private productService = inject(ProductService);
  private errorService = inject(ErrorService);
  private orderService = inject(OrderService);
  private router = inject(Router);

  items = this.cartService.items;
  products = signal<Record<string, Product>>({});
  loading = signal(false);
  cartItems = computed(() => {
    return this.items()
      .map(item => {
        const product = this.products()[item.productId];

        if (!product) {
          return null;
        }

        return {
          ...item,
          product,
          subtotal: product.price * item.quantity
        };
      })
      .filter((item => item !== null));
  })

  total = computed(() => {
    return this.cartItems().reduce(
      (sum, item) => sum + item.subtotal,
      0
    );
  });

  ngOnInit() {
    this.loadProducts();
  }

 /* constructor() {
    effect(() => {
      const items = this.items();
      console.log('Cart changed:', this.items());
      this.loadProducts(items);
    });
  }*/

  increaseQuantity(productId: string) {
    this.cartService.increaseQuantity(productId);
  }

  decreaseQuantity(productId: string) {
    this.cartService.decreaseQuantity(productId);
  }

  removeItem(productId: string) {
    this.cartService.removeItem(productId);
  }

  loadProducts() {
    const ids = this.items().map(i => i.productId);

    if (ids.length === 0) {
      this.products.set({});
      return;
    }
    this.loading.set(true);
    
    this.productService.getProductsByIds(ids)
      .pipe(
        finalize(() => {
          this.loading.set(false);
        })
      ).subscribe({
          next: result => {
          const productById: Record<string, Product> = {};

          result.forEach(product => {
            productById[product.id] = product;
          });

          this.products.set(productById);
        }, error: err => {
          this.errorService.setHttpError(err);
          console.error('Failed to load cart products', err);
        }
      });

  }

  checkout() {
    const command: CreateOrder = {
      items: this.items().map(item => ({
        productId: item.productId,
        quantity: item.quantity
      }))
    };

    this.orderService.createOrder(command).subscribe({
      next: orderId => {
        console.log('Order created:', orderId);
        this.cartService.clear();
        this.router.navigate(['/orders', orderId]);
      },
      error: err => {
        this.errorService.setHttpError(err);
        console.error('Order creation failed', err);
      }
    });
  }
}
