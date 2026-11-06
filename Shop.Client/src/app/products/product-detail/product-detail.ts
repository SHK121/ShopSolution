import { Component, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProductService } from '../product.service';
import { Product } from '../product.model';
import { ErrorService } from '../../shared/services/error.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-product-detail',
  imports: [RouterLink],
  templateUrl: './product-detail.html',
  styleUrl: './product-detail.css',
})
export class ProductDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private productService = inject(ProductService);
  private errorService = inject(ErrorService);

  product = signal<Product | undefined>(undefined);
  loading = signal(true);

  ngOnInit() {
    this.errorService.clearError();
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.errorService.setError('Product ID is missing');
      this.loading.set(false);
      return;
    }

    this.productService.getProduct(id)
      .pipe(finalize(() => {
        this.loading.set(false);
      }))
      .subscribe({
        next: (product) => {
          this.product.set(product);
        },
        error: () => {
          this.errorService.setError('Could not load the product');
        }
      });
  }
}
