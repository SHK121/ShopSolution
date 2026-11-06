import { Routes } from '@angular/router';
import { ProductList } from './products/product-list/product-list';
import { ProductCreate } from './products/product-create/product-create';
import { ProductEdit } from './products/product-edit/product-edit';
import { OrderListComponent } from './orders/order-list/order-list';
import { Login } from './auth/login/login';
import { OrderDetailComponent } from './orders/order-detail/order-detail';
import { CartComponent } from './shared/components/cart/cart';
import { ProductDetailComponent } from './products/product-detail/product-detail';

export const routes: Routes = [
    {
        path: 'products/:id',
        component: ProductDetailComponent
    },
    {
        path: 'products',
        component: ProductList
    },
    {
        path: 'products/create',
        component: ProductCreate
    },
    {
        path: 'products/edit/:id',
        component: ProductEdit
    },
    {
        path: 'orders',
        component: OrderListComponent
    },
    {
        path: 'login',
        component: Login
    },
    {
        path: 'orders/:id',
        component: OrderDetailComponent
    },
    {
        path: 'cart',
        component: CartComponent
    }
];
