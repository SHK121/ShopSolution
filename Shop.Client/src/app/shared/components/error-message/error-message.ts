import { Component, inject } from '@angular/core';
import { ErrorService } from '../../services/error.service';

@Component({
  selector: 'app-error-message',
  imports: [],
  templateUrl: './error-message.html',
  styleUrl: './error-message.css',
  standalone: true
})
export class ErrorMessageComponent {

  private errorService = inject(ErrorService);
  //error = this.errorService.getError();
  error = this.errorService.error;

  clearError() {
    this.errorService.clearError();
  }
}
