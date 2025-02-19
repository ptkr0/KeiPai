import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { UserService } from '../../services/user.service';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  loginForm!: FormGroup;

  userService = inject(UserService);
  toast = inject(ToastrService);

  http = inject(HttpClient);
  router = inject(Router);

  constructor(private fb: FormBuilder) {
    this.loginForm = this.fb.group({
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required]),
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const { email, password } = this.loginForm.value;
      this.userService.loginUser({ email, password}).subscribe({
        next: (user) => {
          this.userService.currentUserSig.set(user);
          if(user.role === "Developer") {
          this.router.navigateByUrl('/games');
          }
          else if(user.role === "Influencer") {
          this.router.navigateByUrl('/campaigns');
          }
          this.toast.success('Logged in successfully');
        },
        error: (error) => {
          this.toast.error('Invalid email or password');
          if (error.status === 401) {
            this.loginForm.get('password')?.setErrors({ invalid: true });
          }
        },
      });
    } else {
      this.loginForm.markAllAsTouched();
    }
  }
}
