import { Component } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-twitch-button',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './twitch-button.component.html',
  styleUrls: ['./twitch-button.component.css']
})
export class TwitchButtonComponent {

  private clientId = environment.twitchClientId;
  private redirectUri = 'http://localhost:4200/twitch-success';
  private scopes = 'user:read:email moderator:read:followers';

  authorize() {
    const authUrl = 'https://id.twitch.tv/oauth2/authorize?' + 
    'client_id=' + this.clientId + 
    '&redirect_uri=' + this.redirectUri + 
    '&response_type=code' + 
    '&scope=' + encodeURIComponent(this.scopes);

    window.location.href = authUrl;
  }
}
