import { Component, inject } from '@angular/core';
import { UserService } from '../../services/user.service';
import { DeveloperSettingsComponent } from "../developer-settings/developer-settings.component";
import { InfluencerSettingsComponent } from "../influencer-settings/influencer-settings.component";

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [DeveloperSettingsComponent, InfluencerSettingsComponent],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.css'
})
export class SettingsComponent {

  userService = inject(UserService);

  constructor() {
    console.log(this.userService.user());
  }
}
