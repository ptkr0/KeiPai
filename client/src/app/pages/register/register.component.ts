import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {

registerFormInfluencer!: FormGroup;
registerFormDeveloper!: FormGroup;

http = inject(HttpClient);
router = inject(Router);
activatedRoute = inject(ActivatedRoute);

toast = inject(ToastrService);
userService = inject(UserService);

type: string;

constructor(private fb: FormBuilder) {
  this.type = this.activatedRoute.snapshot.params['type'];
  
  this.registerFormInfluencer = this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(32), Validators.pattern(/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$/)]],
    confirmPassword: ['', [Validators.required]],
    language: [null, [Validators.required]],
    contactEmail: [null, [Validators.email]],
  }, {
    validators: this.passwordMatchValidator
  });

  this.registerFormDeveloper = this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(32), Validators.pattern(/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$/)]],
    confirmPassword: ['', [Validators.required]],
    websiteUrl: ['', [Validators.pattern(/^(https?:\/\/)?(www\.)?[a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)$/)]],
    contactEmail: [null, [Validators.email]],
  }, { 
    validators: this.passwordMatchValidator 
  });
}

ngOnInit(): void {}

passwordMatchValidator(form: AbstractControl) {
  const password = form.get('password')?.value;
  const confirmPassword = form.get('confirmPassword')?.value;

  if (password !== confirmPassword) {
    form.get('confirmPassword')?.setErrors({ passwordMismatch: true });
  } else {
    form.get('confirmPassword')?.setErrors(null);
  }
  return null;
}


onSubmitInfluencer(): void {
  if (this.registerFormInfluencer.valid) {
    const { name, email, password, language, contactEmail } = this.registerFormInfluencer.value;
    this.userService.registerUser({ username: name, email, password, language, contactEmail }).subscribe({
      next: (user) => {
        this.toast.success('User registered successfully');
        this.userService.currentUserSig.set(user);
        this.router.navigateByUrl('/');
      },
      error: (err) => {
        const errorMessage = err.error && err.error.length > 0 ? err.error[0].description : 'Error registering user';
        this.toast.error(errorMessage);
      },
    },

    );
  } else {
    console.log('Form is invalid');
  }
}

onSubmitDeveloper(): void {
  if (this.registerFormDeveloper.valid) {
    const { name, email, password, websiteUrl, contactEmail } = this.registerFormDeveloper.value;
    this.userService.registerDeveloper({ username: name, email, password, websiteUrl, contactEmail }).subscribe({
      next: (user) => {
        this.toast.success('User registered successfully');
        this.userService.currentUserSig.set(user);
        this.router.navigateByUrl('/');
      },
      error: (err) => {
        const errorMessage = err.error && err.error.length > 0 ? err.error[0].description : 'Error registering user';
        this.toast.error(errorMessage);
      },
    });
  } else {
    console.log('Form is invalid');
  }
}
}