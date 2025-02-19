import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

interface DialogData {
  title: string;
  message: string;
  btnOkText: string;
  btnCancelText: string;
  promiseResolve: (value: boolean) => void;
}

@Injectable({
  providedIn: 'root'
})
export class ConfirmationDialogService {
  private dialogDataSubject = new Subject<DialogData | null>();
  dialogData$ = this.dialogDataSubject.asObservable();

  confirm(title: string, message: string, btnOkText = 'OK', btnCancelText = 'Cancel'): Promise<boolean> {
    return new Promise<boolean>(resolve => {
      this.dialogDataSubject.next({
        title,
        message,
        btnOkText,
        btnCancelText,
        promiseResolve: resolve
      });
    });
  }

  close() {
    this.dialogDataSubject.next(null);
  }
}