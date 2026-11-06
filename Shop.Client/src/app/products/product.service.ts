import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Product } from './product.model';
import { CreateProduct } from './create-product.model';
import { UpdateProduct } from './update-product.model';
import { PaginatedResult } from '../shared/model/paginated-result';
import { PaginationRequest } from '../shared/model/pagination-request';


@Injectable({
    providedIn: 'root'
})

export class ProductService {
    private apiUrl = 'http://localhost:8081/api/products';

    //'https://localhost:7185/api/products';

    constructor(
        private http: HttpClient
    ){}

    getProduct(id: string){
        return this.http.get<Product>(`${this.apiUrl}/${id}`);
    }

    getProducts(request: PaginationRequest): Observable<PaginatedResult<Product>> {
        const params = new HttpParams()
            .set('page', request.page)
            .set('pageSize', request.pageSize);

        return this.http.get<PaginatedResult<Product>>(this.apiUrl, { params });
    }

    getProductsByIds(ids: string[]): Observable<Product[]> {
        const params = new HttpParams().set('ids', ids.join());
        const url = this.apiUrl + '/bulk';

        console.log('params', ids.join());
        console.log('new url', url);
        
        return this.http.get<Product[]>(url, { params });
    }

    createProduct(product: CreateProduct): Observable<string> {
        return this.http.post<string>(
            this.apiUrl, product
        );
    }

    updateProduct(id: string, product: UpdateProduct): Observable<void> {
        return this.http.put<void>(`${this.apiUrl}/${id}`, product);
    }

    deleteProduct(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }

    /*updateProduct(
        "abc-guid",
        {
        name:"Keyboard",
        price:100,
        currency:"EUR",
        stock:10,
        description:"..."
        }
        )*/
}