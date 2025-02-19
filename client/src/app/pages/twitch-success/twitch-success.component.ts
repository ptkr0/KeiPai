import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, Inject, PLATFORM_ID } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-twitch-success',
  standalone: true,
  imports: [],
  templateUrl: './twitch-success.component.html',
  styleUrl: './twitch-success.component.css'
})
export class TwitchSuccessComponent {

  toast = inject(ToastrService);

  constructor(
    private http: HttpClient,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: object
  ) {}

  ngOnInit() {
    if (isPlatformBrowser(this.platformId)) {

      // Get the authorization code from the URL parameters
      const urlParams = new URLSearchParams(window.location.search);
      const credentials = urlParams.get('code');

      if (credentials) {

        this.http.post(`${environment.apiUrl}/twitch/connect`, JSON.stringify(credentials), {
          headers: { 'Content-Type': 'application/json' }
        }).subscribe({
          next: (response: any) => {
            console.log('Tokens received:', response);
          },
          error: (error: any) => {
            console.error('Error exchanging code for tokens:', error);
          }
        });
      } else {
        console.error('Authorization code not found.');
      }
      
      this.router.navigateByUrl('/campaigns');
      this.toast.success('Twitch Connected Successfully');
    }
  }
}
