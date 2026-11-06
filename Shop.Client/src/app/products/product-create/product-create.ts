import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../product.service';
import { CreateProduct } from '../create-product.model';

@Component({
  selector: 'app-product-create',
  standalone: true,
  imports: [ FormsModule ],
  templateUrl: './product-create.html',
  styleUrl: './product-create.css',
})
export class ProductCreate {
  product: CreateProduct = {
    name: '',
    price: 0,
    currency: 'USD',
    stock: 0,
    description: ''
  };

  constructor(private productService: ProductService) {

  }

  save() {
    this.productService.createProduct(this.product)
      .subscribe({
        next: id => {
          console.log('created id:', id);
        },
        error: err => {
          console.error(err);
        }
      });
  }
}
