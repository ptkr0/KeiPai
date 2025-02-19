import { Component, inject } from '@angular/core';
import { InfluencerNavigationComponent } from "../../components/influencer-navigation/influencer-navigation.component";
import { IRequestInfluencer } from '../../models/request.model';
import { RequestService } from '../../services/request.service';
import { RequestPendingComponent } from "../../components/request-pending/request-pending.component";
import { RequestCompletedComponent } from "../../components/request-completed/request-completed.component";

@Component({
  selector: 'app-requests-completed',
  standalone: true,
  imports: [InfluencerNavigationComponent, RequestCompletedComponent],
  templateUrl: './requests-completed.component.html',
  styleUrl: './requests-completed.component.css'
})
export class RequestsCompletedComponent {

  requestService = inject(RequestService);

  requests: IRequestInfluencer[] = [];

  constructor() {
    this.fetchRequests();
   }

  fetchRequests() {
    this.requestService.getRequestsForInfluencer('completed').subscribe({
      next: (res) => {
        this.requests = res;
        console.log('Requests:', res);
      },
      error: (error) => {
        console.error('Error fetching requests:', error);
      }
    });
  }
}
