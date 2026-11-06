import { Injectable } from '@angular/core';
import { ConfirmDialog } from '../components/confirm-dialog/confirm-dialog';
import { Observable, Subject } from 'rxjs';
import { DialogResult } from '../model/dialog-result';
import { DialogOptions } from '../model/dialog-options';

@Injectable({
    providedIn: 'root'
})

export class DialogService {
    
    confirm(options: DialogOptions): Observable<DialogResult> {

        const subject = new Subject<DialogResult>();
        console.log(options);
        return subject.asObservable();
    }
}
