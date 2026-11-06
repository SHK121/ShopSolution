import { Injectable, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})
export class ErrorService {
    private errorMessage = signal<string | null>(null);

    readonly error = this.errorMessage.asReadonly();
    /* Signal
        → "현재 상태가 무엇인가?"

        Observable
        → "시간이 지나면서 어떤 값들이 흘러오는가?" */

   /* getError() {
        console.log('errormessage:', this.errorMessage());
        return this.errorMessage();
    }*/

    setError(message: string) {
        this.errorMessage.set(message);
    }

    clearError() {
        this.errorMessage.set(null);
    }

    setHttpError(error: HttpErrorResponse) {
        const message = error.error?.message;

        if (message) {
            this.setError(message);
            return;
        }

        switch (error.status) {
            case 401:
                this.setError('Please log in first.');
                break;

            case 403:
                this.setError('You are not allowed for this action.');
                break;

            case 409:
                this.setError('This resource was changed by another request. Please try again');
                break;

            case 0:
                this.setError('Please check your network connection.');
                break;

            default:
                this.setError('Something went wrong.');
                break;
        }
    }
}
