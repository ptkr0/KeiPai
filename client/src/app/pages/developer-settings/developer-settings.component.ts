import { Component, inject, Input, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { IDeveloper } from '../../models/developer.model';
import { Router } from '@angular/router';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-developer-settings',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, CommonModule],
  templateUrl: './developer-settings.component.html',
  styleUrl: './developer-settings.component.css'
})
export class DeveloperSettingsComponent implements OnInit {

  @Input()
  username: string | undefined;

  userService = inject(UserService);
  toast = inject(ToastrService);

  router = inject(Router);
  fb = inject(FormBuilder);

  developer: IDeveloper | undefined;
  editProfileForm!: FormGroup;
  success: boolean = false;

  ngOnInit() : void {
    this.editProfileForm = this.fb.group({
      username: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      contactEmail: new FormControl('', [Validators.email]),
      about: new FormControl('', [Validators.maxLength(200)]),
      url: new FormControl('', [Validators.pattern(/^(https?:\/\/)?(www\.)?[a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)$/)])
    })

    this.fetchDeveloper();
  }

  fetchDeveloper() {
    if(this.username) {
      this.userService.findUser(this.username).subscribe({
        next: (res) => {
          this.developer = res.developer?.developer;

          this.editProfileForm.patchValue({
            username: this.developer?.username,
            contactEmail: this.developer?.contactEmail,
            about: this.developer?.about,
            url: this.developer?.websiteUrl,
          });

        },
        error: (error) => {
          this.router.navigate(['/']);
          const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error fetching informations';
          this.toast.error(errorMessage);
        }
      });
    }
  }

  onSubmit() {
    if(this.editProfileForm.valid && this.developer) {
      const form = { ...this.editProfileForm.value };
      if (form.username === this.developer.username) {
        form.username = null;
      }

      this.userService.updateDeveloper(form).subscribe({
        next: (res) => {
          this.success = true;
          if(res.username) {
            this.userService.updateUsername(res.username);
            this.developer!.username = res.username;
          }
          this.toast.success('Profile updated successfully');
        },
        error: (error) => {
          if (error.status === 401) {
            this.editProfileForm.get('username')?.setErrors({ usernameTaken: true });
          }
          const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error editing profile';
          this.toast.error(errorMessage);
        }
      });
    }
  }
}
