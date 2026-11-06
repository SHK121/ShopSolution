import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../auth.service';
import { ErrorService } from '../../shared/services/error.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
  standalone: true
})
export class Login {
  username = '';
  password = '';
  successMessage = '';

  constructor(private authService: AuthService,
    private errorService: ErrorService,
    private router: Router
  ) {}

  login() {
    this.successMessage = '';

    this.authService.login({
      username: this.username,
      password: this.password
    })
    .subscribe({
      next: response => {
        console.log('Login successful');
        console.log('Token', response.token);

        this.successMessage = 'Login successful!';
        console.log('successMessage after:', this.successMessage);

        this.router.navigate(['/orders']);
      },
      error: error => {
        console.error('Login failed', error);
      }
    });
  }

  get hasError(): boolean {
    console.log('successMessage', this.successMessage);
    console.log('hasError', this.errorService.error() !== null);
    return this.errorService.error() !== null;
    // hasError가 읽고 있는 error signal이 변경될 때 Angular가 이 template을 다시 평가한다
    // Signal이 단순한 "값 저장 변수"가 아니라 dependency를 추적하는 reactive primitive라는 게 조금 더 선명해져.
  }
}
