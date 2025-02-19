import { Component } from '@angular/core';
import { ConfirmationDialogService } from './confirmation-dialog.service';

@Component({
  standalone: true,
  selector: 'app-confirmation-dialog',
  templateUrl: './confirmation-dialog.component.html',
})
export class ConfirmationDialogComponent {
  visible = false;
  title = '';
  message = '';
  btnOkText = 'OK';
  btnCancelText = 'Cancel';
  private promiseResolve!: (value: boolean) => void;

  constructor(private confirmationDialogService: ConfirmationDialogService) {
    this.confirmationDialogService.dialogData$.subscribe(data => {
      if (data) {
        this.title = data.title;
        this.message = data.message;
        this.promiseResolve = data.promiseResolve;
        this.visible = true;

      } else {
        this.visible = false;
      }
    });
   }

  onConfirm() {
    this.visible = false;
    this.promiseResolve(true);
  }

  onCancel() {
    this.visible = false;
    this.promiseResolve(false);
  }
}