import { AfterViewChecked, Component, ElementRef, EventEmitter, inject, Input, Output, ViewChild } from '@angular/core';
import { IBasicUserInfo } from '../../models/user.model';
import { IMessage, IMessageUser, ISendMessage } from '../../models/message.model';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { bootstrapPersonCircle } from '@ng-icons/bootstrap-icons';
import { DatePipe } from '@angular/common';
import { UserService } from '../../services/user.service';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MessageService } from '../../services/message.service';

@Component({
  selector: 'app-message-box',
  standalone: true,
  imports: [NgIconComponent, DatePipe, ReactiveFormsModule, FormsModule],
  viewProviders: [provideIcons({ bootstrapPersonCircle }),],
  templateUrl: './message-box.component.html',
  styleUrl: './message-box.component.css'
})
export class MessageBoxComponent implements AfterViewChecked {

  @Input() 
  userInfo: IBasicUserInfo | undefined;
  
  @Input()
  messages: IMessage[] = [];

  messageForm: FormGroup;

  @ViewChild('scrollableTableContainer') scrollableTableContainer: ElementRef | undefined;

  @Output() sendMessageEvent = new EventEmitter<IMessageUser>();

  userService = inject(UserService);
  messageService = inject(MessageService);

  constructor(private fb: FormBuilder) {
    this.messageForm = this.fb.group({
      message: new FormControl('', [Validators.required, Validators.maxLength(100)]),
    });
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  scrollToBottom() {
    if (this.scrollableTableContainer) {
      this.scrollableTableContainer.nativeElement.scrollTop = this.scrollableTableContainer.nativeElement.scrollHeight;
    }
  }

  sendMessage() {
    if (this.messageForm.valid && this.userInfo) {
      const message: ISendMessage = { receiverId: this.userInfo.userId, content: this.messageForm.value.message };
      this.messageService.sendMessage(message).subscribe({
        next: (res) => {
          this.messages.push(res);
          this.messageForm.reset();

          const messageUser: IMessageUser = {
            userId: this.userInfo!.userId,
            username: this.userInfo!.username,
            lastMessage: res.content,
            lastMessageDate: res.createdAt,
          };
          this.sendMessageEvent.emit(messageUser);
        },
        error: (error) => {
          console.error('Error sending message:', error);
        }
      });
    }
  }
}
