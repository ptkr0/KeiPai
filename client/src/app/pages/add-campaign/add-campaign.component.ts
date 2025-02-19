import { CommonModule, formatDate } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AddCampaignSearchGameComponent } from '../../components/add-campaign-search-game/add-campaign-search-game.component';
import { AssignKeysComponent } from '../../components/assign-keys/assign-keys.component';
import { IAddCampaignGame } from '../../models/game.model';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { bootstrapYoutube, bootstrapTwitch, bootstrapNewspaper } from '@ng-icons/bootstrap-icons';
import { Editor, NgxEditorModule, Toolbar } from 'ngx-editor';
import { IAddCampaign } from '../../models/campaign.model';
import { CampaignService } from '../../services/campaign.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-add-campaign',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, AddCampaignSearchGameComponent, NgIconComponent, NgxEditorModule, FormsModule, AssignKeysComponent],
  viewProviders: [provideIcons({ bootstrapYoutube, bootstrapTwitch, bootstrapNewspaper })],
  templateUrl: './add-campaign.component.html',
  styleUrl: './add-campaign.component.css'
})
export class AddCampaignComponent implements OnInit, OnDestroy {

  addCampaignForm!: FormGroup;
  selectedGame: IAddCampaignGame | null = null;

  campaignService = inject(CampaignService);
  toast = inject(ToastrService);
  
  router = inject(Router);

  editor!: Editor;
  toolbar: Toolbar = [
    ['bold', 'italic'],
    ['underline', 'strike'],
    ['code', 'blockquote'],
    ['ordered_list', 'bullet_list'],
    [{ heading: ['h1', 'h2', 'h3', 'h4', 'h5', 'h6'] }],
    ['link', 'image'],
    ['text_color', 'background_color'],
    ['align_left', 'align_center', 'align_right', 'align_justify'],
  ];
  descLength: number = 0;

  constructor(private fb: FormBuilder) {
    this.addCampaignForm = this.fb.group({
      gameId: new FormControl('', [Validators.required]),
      startDate: new FormControl('', [Validators.required]),
      endDate: new FormControl(''),
      description: new FormControl('', [Validators.maxLength(1000)]),
      minimumYoutubeSubscribers: new FormControl({ value: 0, disabled: true }),
      minimumTwitchFollowers: new FormControl({ value: 0, disabled: true }),
      minimumTwitchAvgViewers: new FormControl({ value: 0, disabled: true }),
      minimumYoutubeAvgViews: new FormControl({ value: 0, disabled: true }),
      autoCodeDistribution: new FormControl(false, [Validators.required]),
      embargoDate: new FormControl(''),
      areThirdPartyWebsitesAllowed: new FormControl(0, [Validators.required]),
      isYoutubeSubsChecked: new FormControl(false),
      isYoutubeViewsChecked: new FormControl(false),
      isTwitchFollowsChecked: new FormControl(false),
      isTwitchViewsChecked: new FormControl(false),
      keys: new FormControl([], [Validators.required]),
    }, 
    { validators: this.endDateAfterStartDateValidator });

    this.addCampaignForm.get('isYoutubeSubsChecked')?.valueChanges.subscribe(checked => {
      const control = this.addCampaignForm.get('minimumYoutubeSubscribers');
      if (checked) {
        control?.enable();
      } else {
        control?.disable();
      }
    });

    this.addCampaignForm.get('isYoutubeViewsChecked')?.valueChanges.subscribe(checked => {
      const control = this.addCampaignForm.get('minimumYoutubeAvgViews');
      if (checked) {
        control?.enable();
      } else {
        control?.disable();
      }
    });

    this.addCampaignForm.get('isTwitchFollowsChecked')?.valueChanges.subscribe(checked => {
      const control = this.addCampaignForm.get('minimumTwitchFollowers');
      if (checked) {
        control?.enable();
      } else {
        control?.disable();
      }
    });

    this.addCampaignForm.get('isTwitchViewsChecked')?.valueChanges.subscribe(checked => {
      const control = this.addCampaignForm.get('minimumTwitchAvgViewers');
      if (checked) {
        control?.enable();
      } else {
        control?.disable();
      }
    });
  }

  ngOnInit(): void {
    this.editor = new Editor();
  }

  ngOnDestroy(): void {
    this.editor.destroy();
  }

  onContentChange(): void {
    this.descLength = this.addCampaignForm.get('description')?.value?.length ?? 0;
  }

  setGame($event: IAddCampaignGame) {
    this.selectedGame = ($event as IAddCampaignGame);
    if (this.selectedGame) {
      this.addCampaignForm.get('gameId')?.setValue(this.selectedGame.id);
      
      this.addCampaignForm.get('isYoutubeSubsChecked')?.patchValue(false);
      this.addCampaignForm.get('isYoutubeViewsChecked')?.patchValue(false);
      this.addCampaignForm.get('isTwitchFollowsChecked')?.patchValue(false);
      this.addCampaignForm.get('isTwitchViewsChecked')?.patchValue(false);
      this.addCampaignForm.get('minimumYoutubeSubscribers')?.patchValue(0);
      this.addCampaignForm.get('minimumYoutubeAvgViews')?.patchValue(0);
      this.addCampaignForm.get('minimumTwitchFollowers')?.patchValue(0);
      this.addCampaignForm.get('minimumTwitchAvgViewers')?.patchValue(0);
      this.addCampaignForm.get('keys')?.setValue([]);
    }
  }

  endDateAfterStartDateValidator(group: FormGroup): { [key: string]: any } | null {
    const startDate = group.get('startDate')?.value;
    const endDate = group.get('endDate')?.value;
    if (endDate && startDate && endDate < startDate) {
      group.get('endDate')?.setErrors({ endDateBeforeStartDate: true });
      return { endDateBeforeStartDate: true };
    } else {
      group.get('endDate')?.setErrors(null);
      return null;
    }
  }

  onSubmit() {
    if (this.selectedGame?.youtubeTag === null) {
      this.addCampaignForm.get('isYoutubeSubsChecked')?.patchValue(false);
      this.addCampaignForm.get('isYoutubeViewsChecked')?.patchValue(false);
      this.addCampaignForm.get('minimumYoutubeSubscribers')?.patchValue(null);
      this.addCampaignForm.get('minimumYoutubeAvgViews')?.patchValue(null);
    }
    if (this.selectedGame?.twitchTagId === null || this.selectedGame?.twitchTagId === null) {
      this.addCampaignForm.get('isTwitchFollowsChecked')?.patchValue(false);
      this.addCampaignForm.get('isTwitchViewsChecked')?.patchValue(false);
      this.addCampaignForm.get('minimumTwitchFollowers')?.patchValue(null);
      this.addCampaignForm.get('minimumTwitchAvgViewers')?.patchValue(null);
    }

    if (this.addCampaignForm.get('endDate')?.value === '') {
      this.addCampaignForm.get('endDate')?.patchValue(null);
    }

    if (this.addCampaignForm.valid) {
      const { gameId, startDate, endDate, description, minimumYoutubeSubscribers, minimumYoutubeAvgViews, minimumTwitchFollowers, minimumTwitchAvgViewers, autoCodeDistribution, embargoDate, areThirdPartyWebsitesAllowed, keys } = this.addCampaignForm.value;
      const addCampaign: IAddCampaign = {
        gameId: gameId,
        startDate: formatDate(startDate, 'yyyy-MM-dd', 'en-US'),
        endDate: endDate? formatDate(endDate, 'yyyy-MM-dd', 'en-US'): undefined,
        description: description,
        minimumYoutubeSubscribers: minimumYoutubeSubscribers,
        minimumYoutubeAvgViews: minimumYoutubeAvgViews,
        minimumTwitchFollowers: minimumTwitchFollowers,
        minimumTwitchAvgViewers: minimumTwitchAvgViewers,
        autoCodeDistribution: Boolean(autoCodeDistribution),
        embargoDate: embargoDate? formatDate(embargoDate, 'yyyy-MM-dd', 'en-US'): undefined,
        areThirdPartyWebsitesAllowed: areThirdPartyWebsitesAllowed*1,
        keys: keys,
      };
      addCampaign.areThirdPartyWebsitesAllowed = addCampaign.areThirdPartyWebsitesAllowed*1;

      this.campaignService.addCampaign(addCampaign).subscribe({
        next: (campaign) => {
          this.toast.success('Campaign added successfully');
          this.router.navigateByUrl('/campaigns');
        },
        error: (error) => {
          const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error creating campaign';
          this.toast.error(errorMessage);
      }
      });
    }
  }
}
