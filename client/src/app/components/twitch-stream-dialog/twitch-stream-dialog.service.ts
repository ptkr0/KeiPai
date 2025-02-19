import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

interface DialogData {
  streamId: number;
}

@Injectable({
  providedIn: 'root'
})
export class TwitchStreamDialogService {

  private dialogDataSubject = new Subject<DialogData | null>();
  dialogData$ = this.dialogDataSubject.asObservable();

  open(streamId: number) {
    this.dialogDataSubject.next({ streamId });
  }

  close() {
    this.dialogDataSubject.next(null);
  }
}
