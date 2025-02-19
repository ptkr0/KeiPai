import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { IMessage, IMessageUser } from '../../models/message.model';
import { MessageService } from '../../services/message.service';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { bootstrapPersonFill } from '@ng-icons/bootstrap-icons';
import { IBasicUserInfo } from '../../models/user.model';
import { CommonModule, DatePipe } from '@angular/common';
import { MessageBoxComponent } from "../../components/message-box/message-box.component";

@Component({
  selector: 'app-messages',
  standalone: true,
  imports: [NgIconComponent, DatePipe, MessageBoxComponent, CommonModule],
  viewProviders: [provideIcons({ bootstrapPersonFill }),],
  templateUrl: './messages.component.html',
  styleUrl: './messages.component.css'
})
export class MessagesComponent {

  activatedRoute = inject(ActivatedRoute);
  id: string | null = null;
  messageUsers: IMessageUser[] = [];
  messages?: IMessage[];
  selectedUser?: IBasicUserInfo;

  messageService = inject(MessageService);
  router = inject(Router);

  constructor() {
    this.activatedRoute.params.subscribe(params => {
      this.id = params['id'] || null;
    });
    this.fetchMessageUsers();
    if (this.id) {
      this.fetchMessages(this.id);
    }
  }

  fetchMessageUsers() {
    this.messageService.getUsers().subscribe({
      next: (res) => {
        this.messageUsers = res;
        console.log('Message users:', res);
      },
      error: (error) => {
        console.error('Error fetching user info:', error);
      }
    });
  }

  fetchMessages(userId: string) {
    if(userId !== this.selectedUser?.userId) {
      this.router.navigate(['/messages', userId], {
        queryParamsHandling: 'merge',
      });
      this.messageService.getMessages(userId, 1).subscribe({
        next: (res) => {
          this.selectedUser = res.user;
          this.messages = res.messages;
          console.log('Messages:', res);
        },
        error: (error) => {
          console.error('Error fetching messages:', error);
        }
      });
    }
  }

  updateMessagers(message: IMessageUser) {
    console.log('Updating message users:', message);
    const messageUser = this.messageUsers.find(user => user.userId === message.userId);
    if (messageUser) {
      messageUser.lastMessage = message.lastMessage;
      messageUser.lastMessageDate = message.lastMessageDate;
      this.messageUsers.unshift(this.messageUsers.splice(this.messageUsers.indexOf(messageUser), 1)[0]);
    }
    else {
      this.messageUsers.unshift(message);
    }
  }
}
