import { Component, inject, Input } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Validators } from '@angular/forms';
import { IDeveloper } from '../../models/developer.model';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { IInfluencer, IInfluencerInfo } from '../../models/influencer.model';
import { YoutubeButtonComponent } from "../../components/youtube-button/youtube-button.component";
import { TwitchButtonComponent } from "../../components/twitch-button/twitch-button.component";
import { YoutubeService } from '../../services/youtube.service';
import { ConfirmationDialogService } from '../../components/confirmation-dialog/confirmation-dialog.service';
import { ConnectOtherMediaDialogComponent } from "../../components/connect-other-media-dialog/connect-other-media-dialog.component";
import { IConnectOtherMedia } from '../../models/otherMedia.model';
import { OtherMediaService } from '../../services/other-media.service';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { ionRefreshSharp } from '@ng-icons/ionicons';
import { ToastrService } from 'ngx-toastr';
import { TwitchService } from '../../services/twitch.service';

@Component({
  selector: 'app-influencer-settings',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, CommonModule, YoutubeButtonComponent, TwitchButtonComponent, ConnectOtherMediaDialogComponent, NgIconComponent],
  viewProviders: [provideIcons({ ionRefreshSharp })],
  templateUrl: './influencer-settings.component.html',
  styleUrl: './influencer-settings.component.css'
})
export class InfluencerSettingsComponent {
  @Input()
  username: string | undefined;

  router = inject(Router);
  fb = inject(FormBuilder);

  userService = inject(UserService);
  youtubeService = inject(YoutubeService);
  confirmationDialogService = inject(ConfirmationDialogService);
  otherMediaService = inject(OtherMediaService);
  twitchService = inject(TwitchService);
  toast = inject(ToastrService);

  influencer: IInfluencer | undefined;
  connectedMedia: IInfluencerInfo | undefined;
  editProfileForm!: FormGroup;
  success: boolean = false;

  isOtherMediaDialogVisible = false;

  ngOnInit() : void {
    this.editProfileForm = this.fb.group({
      username: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      contactEmail: new FormControl('', [Validators.email]),
      about: new FormControl('', [Validators.maxLength(200)]),
      language: new FormControl('', [Validators.required]),
    })

    this.fetchDeveloper();
  }

  fetchDeveloper() {
    if(this.username) {
      this.userService.findUser(this.username).subscribe({
        next: (res) => {
          this.influencer = res.influencer?.influencer;
          this.connectedMedia = res.influencer?.media;

          this.editProfileForm.patchValue({
            username: this.influencer?.username,
            contactEmail: this.influencer?.contactEmail,
            about: this.influencer?.about,
            language: this.influencer?.language,
          });
          
        },
        error: (error) => {
          this.router.navigate(['/']);
          const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error fetching info';
          this.toast.error(errorMessage);
        }
      });
    }
  }

  onSubmit() {
    if(this.editProfileForm.valid && this.influencer) {
      const form = { ...this.editProfileForm.value };
      if (form.username === this.influencer.username) {
        form.username = null;
      }

      this.userService.updateInfluencer(form).subscribe({
        next: (res) => {
          if(res.username) {
            this.userService.updateUsername(res.username);
            this.influencer!.username = res.username;
          }
          this.success = true;
          this.toast.success('Profile updated successfully');
        },
        error: (error) => {
          if (error.status === 401) {
            this.editProfileForm.get('username')?.setErrors({ usernameTaken: true });
          }
          const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error updating profile';
          this.toast.error(errorMessage);
        }
      });
    }
  }

  disconnectYoutube() {
    this.youtubeService.disconnectYoutube().subscribe({
      next: () => {
        this.connectedMedia!.youtube = undefined;
      },
      error: (error) => {
        const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error disconnecting YouTube';
        this.toast.error(errorMessage);
      }
    });
  }

  disconnectTwitch() {
    this.twitchService.disconnectTwitch().subscribe({
      next: () => {
        this.connectedMedia!.twitch = undefined;
      },
      error: (error) => {
        const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error disconnecting YouTube';
        this.toast.error(errorMessage);
      }
    });
  }

  disconnectOtherMedia() {
    this.otherMediaService.disconnectMedia().subscribe({
      next: () => {
        this.connectedMedia!.otherMedia = undefined;
      },
      error: (error) => {
        const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error disconnecting media';
        this.toast.error(errorMessage);
      }
    });
  }

  async onYoutubeDisconnect() {
    const confirmed = await this.confirmationDialogService.confirm(
      'Disconnect your YouTube account?',
      "You can reconnect it later. You won't be able to submit content to campaigns that require YouTube verification nor you will be able to join Youtube campaigns until you connect new Youtube account.",
      'Disconnect',
      'Cancel'
    );
    if (confirmed) {
      this.disconnectYoutube();
    } else {
    }
  }

  async onTwitchDisconnect() {
    const confirmed = await this.confirmationDialogService.confirm(
      'Disconnect your Twitch account?',
      "All collected data will be permamently lost. You won't be able to submit content to campaigns that require Twitch verification nor you will be able to join Twitch campaigns until you connect new Twitch account.",
      'Disconnect',
      'Cancel'
    );
    if (confirmed) {
      this.disconnectTwitch();
    } else {
    }
  }

  async onOtherMediaDisconnect() {
    const confirmed = await this.confirmationDialogService.confirm(
      'Disconnect your media outlet?',
      'All saved media content will be lost forever. If your media was verified, re-verification will be required.',
      'Disconnect',
      'Cancel'
    );
    if (confirmed) {
      this.disconnectOtherMedia();
    } else {
    }
  }

  onConnectOtherMedia() {
    this.isOtherMediaDialogVisible = true;
  }

  onConnectOtherMediaDialogCancel() {
    this.isOtherMediaDialogVisible = false;
  }

  onConnectOtherMediaDialogSubmit(otherMediaForm: IConnectOtherMedia) {
    this.isOtherMediaDialogVisible = false;
    this.otherMediaService.connectMedia(otherMediaForm).subscribe({
      next: (res) => {
          this.connectedMedia!.otherMedia = { ...res, isVerified: false };
      },
      error: (error) => {
        const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error connecting media';
        this.toast.error(errorMessage);
      }
    });
  }

  onRefreshYoutube() {
    this.youtubeService.refreshYoutube().subscribe({
      next: () => {
        this.connectedMedia!.youtube!.lastCrawlDate = new Date().toISOString();
      },
      error: (error) => {
        const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error refreshing YouTube';
        this.toast.error(errorMessage);
      }
    });
  }

  canRefresh() {
    const dateNow = new Date();
    const date = new Date(this.connectedMedia?.youtube?.lastCrawlDate || 0);

    return dateNow.getTime() - date.getTime() > 60 * 60 * 1000;
  }
}
