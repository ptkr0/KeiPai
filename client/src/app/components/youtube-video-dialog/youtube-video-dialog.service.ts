import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

interface DialogData {
  videoId: number;
}

@Injectable({
  providedIn: 'root'
})
export class YoutubeVideoDialogService {

  private dialogDataSubject = new Subject<DialogData | null>();
  dialogData$ = this.dialogDataSubject.asObservable();

  open(videoId: number) {
    this.dialogDataSubject.next({ videoId });
  }

  close() {
    this.dialogDataSubject.next(null);
  }
}
