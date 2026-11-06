import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ErrorService } from './shared/services/error.service';
import { ErrorMessageComponent } from './shared/components/error-message/error-message';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    ErrorMessageComponent
    //CartComponent
  ],
  templateUrl: './app.html',
  standalone: true,
})
export class App {
  constructor(private errorService: ErrorService) {}

  testError() {
    this.errorService.setError('Test...');
  }
}
