import { Component, inject } from '@angular/core';
import { UserService } from '../../services/user.service';
import { ActivatedRoute, Router } from '@angular/router';
import { IUserInfo } from '../../models/user.model';
import { InfluencerProfileComponent } from "../influencer-profile/influencer-profile.component";
import { DeveloperProfileComponent } from "../developer-profile/developer-profile.component";

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [InfluencerProfileComponent, DeveloperProfileComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent {

  userService = inject(UserService);
  router = inject(Router);
  username: string;
  activatedRoute = inject(ActivatedRoute);
  profile: IUserInfo | undefined = undefined;
  loading: boolean = true;

  constructor() {
    this.username = this.activatedRoute.snapshot.params['username'];
    this.fetchUserInfo();
  }

  fetchUserInfo() {
    this.userService.findUser(this.username).subscribe({
      next: (res) => {
        this.profile = res;
        this.loading = false;
        console.log('User info:', res);
      },
      error: (error) => {
        console.error('Error fetching user info:', error);
        this.loading = false;
      }
    });
  }
}
