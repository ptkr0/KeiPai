import { HttpClient } from '@angular/common/http';
import { Component, OnInit, Inject, PLATFORM_ID, inject } from '@angular/core';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import { environment } from '../../../environments/environment.development';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-youtube-success',
  standalone: true,
  imports: [],
  templateUrl: './youtube-success.component.html',
  styleUrl: './youtube-success.component.css'
})

export class YoutubeSuccessComponent implements OnInit {

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

        this.http.post(`${environment.apiUrl}/youtube/connect`, JSON.stringify(credentials), {
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
      this.toast.success('YouTube Connected Successfully');
    }
  }
}