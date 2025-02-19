import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

interface DialogData {
  mediaId: number;
}

@Injectable({
  providedIn: 'root'
})
export class OtherMediaDialogService {

  private dialogDataSubject = new Subject<DialogData | null>();
  dialogData$ = this.dialogDataSubject.asObservable();

  open(mediaId: number) {
    this.dialogDataSubject.next({ mediaId });
  }

  close() {
    this.dialogDataSubject.next(null);
  }
}
