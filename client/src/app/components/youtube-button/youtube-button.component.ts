import { Component } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-youtube-button',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './youtube-button.component.html',
  styleUrl: './youtube-button.component.css'
})
export class YoutubeButtonComponent {

  private clientId = environment.googleClientId;
  private redirectUri = 'http://localhost:4200/youtube-success';
  private scopes = 'email https://www.googleapis.com/auth/youtube.readonly';


  authorize() {
    const authUrl = 'https://accounts.google.com/o/oauth2/v2/auth?' + 
    'client_id=' + this.clientId + 
    '&redirect_uri=' + this.redirectUri + 
    '&response_type=code' + 
    '&scope=' + this.scopes +
    '&access_type=offline' +
    '&prompt=consent';
  
    window.location.href = authUrl;
  }
}
