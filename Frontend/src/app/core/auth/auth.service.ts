import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { ApiService } from '../services/api.service';
import { LoginRequest, LoginResponse, RegisterRequest, RegisterResponse, User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'auth_user';
  
  private currentUserSubject = new BehaviorSubject<User | null>(this.getStoredUser());
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private apiService: ApiService) {}

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.apiService.login(credentials).pipe(
      tap(response => {
        this.setToken(response.token);
        const user: User = {
          id: response.userId,
          email: response.email,
          firstName: response.firstName,
          lastName: response.lastName,
          createdAt: new Date().toISOString()
        };
        this.setUser(user);
      })
    );
  }

  register(userData: RegisterRequest): Observable<RegisterResponse> {
    return this.apiService.register(userData);
  }

  logout(): void {
    this.removeToken();
    this.removeUser();
    this.currentUserSubject.next(null);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;
    
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const currentTime = Math.floor(Date.now() / 1000);
      const isExpired = payload.exp <= currentTime;
      return !isExpired;
    } catch (error) {
      return false;
    }
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  private setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  private removeToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  private setUser(user: User): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  private getStoredUser(): User | null {
    const userStr = localStorage.getItem(this.USER_KEY);
    if (!userStr) return null;
    
    try {
      return JSON.parse(userStr);
    } catch {
      return null;
    }
  }

  private removeUser(): void {
    localStorage.removeItem(this.USER_KEY);
  }
}
