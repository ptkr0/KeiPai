import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dev-navigation',
  standalone: true,
  imports: [],
  templateUrl: './dev-navigation.component.html',
  styleUrl: './dev-navigation.component.css'
})
export class DevNavigationComponent {

  router = inject(Router);

  redirector(path: string) {
    this.router.navigate([path]);
  }
}
