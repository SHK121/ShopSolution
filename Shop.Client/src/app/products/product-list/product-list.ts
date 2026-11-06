import { Component, OnInit, signal, computed  } from '@angular/core';
import { Product } from '../product.model';
import { ProductService } from '../product.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { PaginatedResult } from '../../shared/model/paginated-result';
import { PaginationRequest } from '../../shared/model/pagination-request';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../shared/services/cart.service';

@Component({
  selector: 'app-product-list',
  imports: [ CommonModule, RouterLink, FormsModule ],
  templateUrl: './product-list.html',
  standalone: true,
  styleUrl: './product-list.css'
})
export class ProductList implements OnInit {

  paginatedResult = signal<PaginatedResult<Product>>({
    page: 1,
    pageSize: 2,
    totalCount: 0,
    items: [],
    totalPages: 0
  });
  paginationRequest: PaginationRequest = {
    page: 1,
    pageSize: 2
  };
  pageNumbers = computed(() => {
    const totalPages = this.paginatedResult().totalPages;

    return Array.from(
      { length: totalPages },
      (_, index) => index + 1
    );
  });

  /*I used computed() because the page numbers depend on the total number of pages in paginatedResult. When the pagination result changes, Angular automatically recalculates the page numbers. This avoids manually synchronizing the page number array with the pagination state.

computed()는 값을 계산하고, effect()는 값이 바뀌었을 때 어떤 작업을 실행해.
1. computed()는 다른 Signal을 기반으로 값을 계산한다.
2. 의존하는 Signal이 변경되면 계산된 값도 자동으로 갱신된다.*/
  //loading: boolean = true;
  loading = signal(true);
  //quantity: number = 0;

  quantities: Record<string, number> = {};

  constructor(
    private productService: ProductService,
    private cartService: CartService
  ) {}

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
     if(this.paginationRequest.page < 1){
      this.paginationRequest.page = 1;
    }

    //this.loading = true;
    this.loading.set(true);

    this.productService.getProducts(this.paginationRequest)
      .subscribe(paginatedResult => {
        console.log("result", paginatedResult);
          //this.paginatedResult = paginatedResult;
          this.paginatedResult.set(paginatedResult);
          //this.loading = false;
          this.loading.set(false);
        });
  }

  test() {
    console.log(this.paginatedResult().items);
    console.log(this.paginatedResult().totalCount);

    this.paginatedResult().items.push({
      id: crypto.randomUUID(),
      name: 'TEST',
      price: 1,
      currency: 'EUR',
      stock: 1,
      description: 'test'
    });

    console.log("after push", this.paginatedResult().items.length);
  }

  delete(id: string) {
    if (!confirm("Delete this product?"))
    return;

  this.productService.deleteProduct(id)
    .subscribe({
      next: () => {
        this.loadProducts();
      },
      error: err => console.error(err)
    });
  }

  addToCart(id: string) {
    const quantity = this.quantities[id] ?? 1;
    this.cartService.addItem(id, quantity);
  }

  goToPage(page: number) {
    const totalPages = this.paginatedResult().totalPages;

    if (page < 1|| page > totalPages) {
      return;
    }

    this.paginationRequest.page = page;
    this.loadProducts();
  }

  /*testAddToCart(id: string) {
    console.log('TEST: adding to cart', id);
    this.cartService.addItem(id, 1);
  }*/
}