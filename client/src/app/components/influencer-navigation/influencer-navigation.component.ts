import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-influencer-navigation',
  standalone: true,
  imports: [],
  templateUrl: './influencer-navigation.component.html',
  styleUrl: './influencer-navigation.component.css'
})
export class InfluencerNavigationComponent {
  router = inject(Router);

  redirector(path: string) {
    this.router.navigate([path]);
  }
}
