import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface LoginRequest {
    username: string;
    password: string;
}

export interface LoginResponse {
    token: string;
}

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private apiUrl = 'http://localhost:8081/api/auth'; //'https://localhost:7185/api/auth';

    constructor(private http: HttpClient) {}

    login(request: LoginRequest): Observable<LoginResponse> {
        return this.http
            .post<LoginResponse>(`${this.apiUrl}/login`, request)
            .pipe(
                tap(response => {
                localStorage.setItem('token', response.token);
                })
            );
    }
}
