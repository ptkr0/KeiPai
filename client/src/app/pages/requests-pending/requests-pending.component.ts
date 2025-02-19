import { Component, inject } from '@angular/core';
import { InfluencerNavigationComponent } from "../../components/influencer-navigation/influencer-navigation.component";
import { RequestService } from '../../services/request.service';
import { IRequestInfluencer } from '../../models/request.model';
import { RequestPendingComponent } from "../../components/request-pending/request-pending.component";

@Component({
  selector: 'app-requests-pending',
  standalone: true,
  imports: [InfluencerNavigationComponent, RequestPendingComponent],
  templateUrl: './requests-pending.component.html',
  styleUrl: './requests-pending.component.css'
})
export class RequestsPendingComponent {

  requestService = inject(RequestService);

  requests: IRequestInfluencer[] = [];

  constructor() {
    this.fetchRequests();
   }

  fetchRequests() {
    this.requestService.getRequestsForInfluencer('pending').subscribe({
      next: (res) => {
        this.requests = res;
        console.log('Requests:', res);
      },
      error: (error) => {
        console.error('Error fetching requests:', error);
      }
    });
  }

  cancelRequest(requestId: number) {
    this.requestService.cancelRequest(requestId).subscribe({
      next: () => {
        this.requests = this.requests.filter(request => request.id !== requestId);
      },
      error: (error) => {
        console.error('Error canceling request:', error);
      }
    });
  }
}
