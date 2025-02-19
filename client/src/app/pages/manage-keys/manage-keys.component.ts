import { Component, ElementRef, Inject, ViewChild } from '@angular/core';
import { IPagination } from '../../models/pagination.model';
import { IAddKeys, IKey, IKeyPagination } from '../../models/key.model';
import { KeyService } from '../../services/key.service';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PlatformService } from '../../services/platform.service';
import { IPlatform } from '../../models/platform.model';
import { FormsModule } from '@angular/forms';
import { provideIcons } from '@ng-icons/core';
import { bootstrapInfoCircle } from '@ng-icons/bootstrap-icons';
import { AddKeysDialogComponent } from '../../components/add-keys-dialog/add-keys-dialog.component';
import { DeleteKeysBulkComponent } from '../../components/delete-keys-bulk/delete-keys-bulk.component';

@Component({
  selector: 'app-manage-keys',
  standalone: true,
  imports: [CommonModule, FormsModule, AddKeysDialogComponent, DeleteKeysBulkComponent],
  providers: [provideIcons({ bootstrapInfoCircle })],
  templateUrl: './manage-keys.component.html',
  styleUrl: './manage-keys.component.css'
})
export class ManageKeysComponent {
  id: number;
  platforms: Array<IPlatform & { selected: boolean }> = [];
  keys: IKey[] = [];
  pagination: IPagination | null = null;
  currentPage: number = 1;
  
  currentPlatforms: string[] = [];

  addKeysDialogComponent = AddKeysDialogComponent;
  deleteKeysBulkComponent = DeleteKeysBulkComponent;
  @ViewChild('scrollableTableContainer') scrollableTableContainer: ElementRef | undefined;

  isDeleteKeysBulkDialogVisible = false;
  isAddKeysDialogVisible = false;

  constructor(
    private activatedRoute: ActivatedRoute,
    private keyService: KeyService,
    private platformService: PlatformService,
  ) {
    this.id = this.activatedRoute.snapshot.params['id'];
    this.platforms = this.platformService.getPlatforms().map(platform => ({
      ...platform,
      selected: false
    }));
    this.fetchKeys(1, []);
  }

  fetchKeys(pageNumber: number = 1, platforms: string[]) {
    this.currentPage = pageNumber;
    this.currentPlatforms = platforms;
    this.keyService.getKeys(this.id, this.currentPage, this.currentPlatforms).subscribe({
      next: (res: IKeyPagination) => {
        this.pagination = {
          totalCount: res.totalCount,
          currentPage: res.currentPage,
          pageSize: res.pageSize,
          totalPages: res.totalPages
        };
        this.keys = res.keys;
        if (this.scrollableTableContainer) {
          this.scrollableTableContainer.nativeElement.scrollTop = 0;
        }
      },
      error: (error: any) => {
        console.error('Error fetching keys:', error);
      }
    });
  }

  onFilterClick() {
    this.currentPlatforms = this.platforms
      .filter(platform => platform.selected)
      .map(platform => platform.id.toString());
    this.fetchKeys(1, this.currentPlatforms);
  }

  onAddKeysClick() {
    this.isAddKeysDialogVisible = true;
  }

  onAddKeysDialogConfirm(keys: IAddKeys) {
    this.isAddKeysDialogVisible = false;
    this.keyService.addKeys(keys.gameId, keys.platformId, keys.keys).subscribe({
      next: () => {
        this.fetchKeys(this.currentPage, this.currentPlatforms);
      },
      error: (error: any) => {
        console.error('Error adding keys:', error);
      },
    });
  }

  onAddKeysDialogCancel() {
    this.isAddKeysDialogVisible = false;
    console.log('Add keys canceled');
  }

  onDeleteKeysClick() {
    this.isDeleteKeysBulkDialogVisible = true;
  }

  onDeleteKeysBulkConfirm(platformId: number) {
    this.isDeleteKeysBulkDialogVisible = false;
    console.log('Deleting keys for platform:', platformId);
  }

  onDeleteKeysBulkCancel() {
    this.isDeleteKeysBulkDialogVisible = false;
    console.log('Delete keys canceled');
  }

  deleteKeys(keyIds: number[]) {
    this.keyService.deleteKeys(keyIds).subscribe({
      next: () => {
        this.keys = this.keys.filter(key => !keyIds.includes(key.id));
        this.pagination!.totalCount -= keyIds.length;
      },
      error: (error: any) => {
        console.error('Error deleting keys:', error);
      }
    });
  }

  prevPage() {
    if (this.pagination?.currentPage && this.pagination.currentPage > 1) {
      this.fetchKeys(this.pagination.currentPage - 1, this.currentPlatforms);
    }
  }

  nextPage() {
    if (this.pagination?.currentPage && this.pagination.currentPage < this.pagination.totalPages) {
      this.fetchKeys(this.pagination.currentPage + 1, this.currentPlatforms);
    }
  }
}