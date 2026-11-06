import { Injectable, signal } from '@angular/core';
import { CartItem, Cart } from '../model/cart.model';

@Injectable({
    providedIn: 'root'
})
export class CartService {
    private itemsSignal = signal<CartItem[]>([]);
    readonly items = this.itemsSignal.asReadonly();

    // providedIn: 'root'이기 때문에 Angular 애플리케이션 전체에서 같은 CartService 인스턴스를 공유하게 돼.
    // ProductList -> CartService <- CartComponent <- OrderCreate

    constructor() {
        console.log('CartService CREATED');
    }

    increaseQuantity(productId: string) {
        const items = this.itemsSignal();

        this.itemsSignal.set(
            items.map(item => 
                item.productId === productId ? 
                { ...item, quantity: item.quantity + 1 }
                : item
            )
        );
    }

    decreaseQuantity(productId: string) {
        const items = this.itemsSignal();

        this.itemsSignal.set(
            items.map(item => 
                item.productId === productId ?
                { ...item, quantity: item.quantity - 1 }
                : item
            )
            .filter(item => item.quantity > 0)
        );
    }

    removeItem(productId: string) {
        const items = this.itemsSignal();

        this.itemsSignal.set(items.filter(item => item.productId !== productId));
    }

    clear() {
        this.itemsSignal.set([]);
    }

    addItem(productId: string, quantity: number) {
        console.log('addItem', productId, quantity);
        const copiedItems = this.itemsSignal();

        const index = copiedItems.findIndex(i => i.productId === productId);

        if(index < 0) {
            this.itemsSignal.set([...copiedItems, { productId, quantity }]);
        } else {
            this.itemsSignal.set(
                copiedItems.map((item, i) => i === index ? {
                    ...item, quantity: item.quantity + quantity
                } : item)
            );
            //처럼 새 배열을 만들어 set하면 상태 변경을 명확하게 표현할 수 있어.
            // items의 요소들을 새 배열 안에 펼쳐서 복사한다는 뜻이야.
        }
        console.log('after addItem ', this.itemsSignal());
    }

   /* getItems() {
        // "상태 변경은 CartService가 책임지고, Component는 읽기만 해."
        return this.items.asReadonly();
    }*/
}
