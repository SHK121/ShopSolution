import { Component, OnInit, signal } from '@angular/core';
import { UpdateProduct } from '../update-product.model';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../product.service';
import { FormsModule } from '@angular/forms';
import { Product } from '../product.model';
import { CommonModule } from '@angular/common'; //Old..*ngFor
import { ErrorService } from '../../shared/services/error.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-product-edit',
  standalone: true,
  imports: [ FormsModule, CommonModule ],
  templateUrl: './product-edit.html',
  styleUrl: './product-edit.css',
})
export class ProductEdit implements OnInit {
  id!: string;
  //product: UpdateProduct | null = null;
  product = signal<Product | null>(null);
  loading = signal(false);


  constructor(private route: ActivatedRoute,
    private productService: ProductService,
    private router: Router,
    private errorService: ErrorService
  ){}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) 
      return;
    this.id = id;
    this.loading.set(true);
    this.productService.getProduct(this.id)
      .pipe(
        finalize(() => {
          this.loading.set(false);
        })
      )
      .subscribe({      
        next: result => {
          this.product.set(result);
        }, error: err => {
          this.errorService.setHttpError(err);
        }
      });
      // this.product = product 안됨, id가 없어서
    console.log("edit id:", this.id);
  }

  goBack() {
    this.router.navigate(['/products']);
  }

  update() {
    /*if(!this.product)
      return; Signal never null*/
    const currentProduct = this.product();
    if (!currentProduct)
      return;

    const request: UpdateProduct = {
      name: currentProduct.name,//this.product.name,
      price: currentProduct.price, //this.product.price,
      currency: currentProduct.currency, //this.product.currency,
      stock: currentProduct.stock, //this.product.stock,
      description: currentProduct.description //this.product.description
    };

    this.loading.set(true);
    this.productService.updateProduct(this.id, request)
      .pipe(
        finalize(() => {
          this.loading.set(false);
        })
      )
      .subscribe({
        next:() => {
          this.router.navigate(['/products', this.id]);
        },
        error: err => {
          this.errorService.setHttpError(err);
        }
      });
  }
}
