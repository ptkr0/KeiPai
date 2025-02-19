import { Component, inject } from '@angular/core';
import { InfluencerNavigationComponent } from "../../components/influencer-navigation/influencer-navigation.component";
import { RequestService } from '../../services/request.service';
import { IRequestInfluencer } from '../../models/request.model';
import { RequestAcceptedComponent } from '../../components/request-accepted/request-accepted.component';
import { SubmitContentDialogComponent } from "../../components/submit-content-dialog/submit-content-dialog.component";
import { IBasicGame } from '../../models/game.model';
import { OtherMediaService } from '../../services/other-media.service';

@Component({
  selector: 'app-requests-accepted',
  standalone: true,
  imports: [InfluencerNavigationComponent, RequestAcceptedComponent, SubmitContentDialogComponent],
  templateUrl: './requests-accepted.component.html',
  styleUrl: './requests-accepted.component.css'
})
export class RequestsAcceptedComponent {

  selectedGame: IBasicGame | undefined = undefined;
  selectedRequest: number | undefined = undefined;
  isSubmitContentDialogVisible: boolean = false;

  requestService = inject(RequestService);
  otherMediaService = inject(OtherMediaService);

  requests: IRequestInfluencer[] = [];

  constructor() {
    this.fetchRequests();
   }

  fetchRequests() {
    this.requestService.getRequestsForInfluencer('accepted').subscribe({
      next: (res) => {
        this.requests = res;
        console.log('Requests:', res);
      },
      error: (error) => {
        console.error('Error fetching requests:', error);
      }
    });
  }
    
  cancelSubmitContent() {
    this.isSubmitContentDialogVisible = false;
  }

  submitContent(formData: any) {
    console.log('Submitting content:', formData);
    this.otherMediaService.addContent(formData).subscribe({
      next: (res) => {
        console.log('Content submitted:', res);
        console.log('deleting request', this.selectedRequest)
        this.requests = this.requests.filter(r => r.id !== this.selectedRequest);
        this.selectedRequest = undefined;
        this.selectedGame = undefined;
      },
      error: (error) => {
        console.error('Error submitting content:', error);
        this.selectedRequest = undefined;
        this.selectedGame = undefined;
      }
    });
    this.isSubmitContentDialogVisible = false;
  }

  onOpenSubmitContentDialog(event: { game: IBasicGame, requestId: number }) {
    this.selectedGame = event.game;
    this.selectedRequest = event.requestId;
    this.isSubmitContentDialogVisible = true;
  }
}
