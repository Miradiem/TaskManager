import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/auth/auth.service';
import { LoginRequest, RegisterRequest } from '../../core/models/user.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ]
})
export class LoginComponent {
  isLoginMode = true;
  loginForm!: FormGroup;
  registerForm!: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.initializeForms();
  }

  private initializeForms(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });

    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]]
    });
  }

  get currentForm(): FormGroup {
    return this.isLoginMode ? this.loginForm : this.registerForm;
  }

  toggleMode(): void {
    this.isLoginMode = !this.isLoginMode;
  }

  onLogin(): void {
    this.loginForm.markAllAsTouched();
    
    if (this.loginForm.valid) {
      this.isLoading = true;
      const credentials: LoginRequest = this.loginForm.value;
      
      this.authService.login(credentials).subscribe({
        next: () => {
          this.snackBar.open('Login successful!', 'Close', { duration: 3000 });
          this.router.navigate(['/tasks']);
          this.isLoading = false;
        },
        error: (error) => {
          this.snackBar.open('Login failed. Please check your credentials.', 'Close', { duration: 5000 });
          console.error('Login error:', error);
          this.isLoading = false;
        }
      });
    }
  }

  onRegister(): void {
    this.registerForm.markAllAsTouched();
    
    if (this.registerForm.valid) {
      this.isLoading = true;
      const userData: RegisterRequest = this.registerForm.value;
      
      this.authService.register(userData).subscribe({
        next: () => {
          this.snackBar.open('Registration successful! Please login.', 'Close', { duration: 3000 });
          window.location.reload();
        },
        error: (error) => {
          console.error('Registration error:', error);
          let errorMessage = 'Registration failed. Please try again.';
          
          if (error.status === 409) {
            errorMessage = 'User with this email already exists. Please use a different email.';
          } else if (error.error?.message) {
            errorMessage = error.error.message;
          }
          
          this.snackBar.open(errorMessage, 'Close', { duration: 5000 });
          this.isLoading = false;
        }
      });
    }
  }
}
