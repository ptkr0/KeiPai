import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { UserService } from './services/user.service';
import { HeaderComponent } from "./components/header/header.component";
import { FooterComponent } from "./components/footer/footer.component";
import { ConfirmationDialogComponent } from './components/confirmation-dialog/confirmation-dialog.component';
import { MainPageComponent } from "./pages/main-page/main-page.component";
import { OtherMediaDialogComponent } from "./components/other-media-dialog/other-media-dialog.component";
import { YoutubeVideoDialogComponent } from './components/youtube-video-dialog/youtube-video-dialog.component';
import { CommonModule } from '@angular/common';
import { TwitchStreamDialogComponent } from "./components/twitch-stream-dialog/twitch-stream-dialog.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent, ConfirmationDialogComponent, MainPageComponent, OtherMediaDialogComponent, YoutubeVideoDialogComponent, CommonModule, TwitchStreamDialogComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'KeiPai';
  userService = inject(UserService);
  router = inject(Router);
}

