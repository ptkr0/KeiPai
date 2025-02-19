import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router, RouterLink } from '@angular/router';
import { UserService } from '../../services/user.service';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { bootstrapEnvelopeFill, bootstrapPersonFill } from '@ng-icons/bootstrap-icons';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, NgIconComponent],
  viewProviders: [provideIcons({ bootstrapPersonFill, bootstrapEnvelopeFill }),],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  userService = inject(UserService);
  http = inject(HttpClient);
  router = inject(Router);

  constructor() {
    console.log(this.userService.user());
  }

  onLogout() {
    this.userService.logout().subscribe({
      next: () => {
        // Navigate to the login page regardless of the response content
        this.userService.currentUserSig.set(null);
        this.router.navigateByUrl('/login');
      },
      error: (err) => {
        console.log('Error logging out', err);
      },
    });
  }
}
