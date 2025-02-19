import { Component, forwardRef, HostListener, inject, OnInit, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ControlValueAccessor,
  FormControl,
  NG_VALUE_ACCESSOR,
  ReactiveFormsModule,
} from '@angular/forms';
import { TagService } from '../../services/tag.service';
import { ITag } from '../../models/tag.model';

@Component({
  selector: 'app-tag-select',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TagSelectComponent),
      multi: true,
    },
  ],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './tag-select.component.html',
  styleUrls: ['./tag-select.component.css'],
})
export class TagSelectComponent implements OnInit, ControlValueAccessor {
  private tagService = inject(TagService);
  private elementRef = inject(ElementRef);
  searchControl = new FormControl('');
  tags: ITag[] = [];
  filteredTags: ITag[] = [];
  showDropdown = false;
  selectedTags: ITag[] = [];
  initialTags: number[] = [];

  onChange = (tagIds: number[]) => {};
  onTouched = () => {};

  registerOnChange(fn: (tagIds: number[]) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    isDisabled ? this.searchControl.disable() : this.searchControl.enable();
  }

  ngOnInit(): void {
    this.tagService.getTags().subscribe((tags) => {
      this.tags = tags;
      this.filteredTags = tags;

      if (this.initialTags.length > 0) {
        this.selectedTags = this.tags.filter(tag => this.initialTags.includes(tag.id));
        this.onChange(this.initialTags);
      }
    });

    this.searchControl.valueChanges.subscribe((value) => {
      this.filterTags(value ?? '');
    });
  }

  filterTags(query: string): void {
    if (!query) {
      this.filteredTags = this.tags;
    } else {
      const lowerQuery = query.toLowerCase();
      this.filteredTags = this.tags.filter((tag) =>
        tag.name.toLowerCase().includes(lowerQuery)
      );
    }
  }

  selectTag(tagId: number): void {
    const tag = this.tags.find((t) => t.id === tagId);
    if (!tag) {
      return;
    }

    const index = this.selectedTags.findIndex((t) => t.id === tagId);

    if (index >= 0) {
      this.selectedTags.splice(index, 1);
    } else {
      if (this.selectedTags.length < 5) {
        this.selectedTags.push(tag);
      }
    }

    const selectedTagIds = this.selectedTags.map((t) => t.id);
    this.onChange(selectedTagIds);
    this.onTouched();
  }

  isSelected(tagId: number): boolean {
    return this.selectedTags.some((t) => t.id === tagId);
  }

  isDisabled(tagId: number): boolean {
    return (
      this.selectedTags.length >= 5 && !this.isSelected(tagId)
    );
  }

  writeValue(value: number[]): void {
    if (Array.isArray(value)) {
      this.initialTags = value;
      if (this.tags.length > 0) {
      this.selectedTags = this.tags.filter((tag) => value.includes(tag.id));
      }
    } else {
      this.selectedTags = [];
    }
  }

  onBlur(): void {
    setTimeout(() => {
      this.showDropdown = false;
    }, 200);
    this.onTouched();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const targetElement = event.target as HTMLElement;
    if (
      targetElement &&
      !this.elementRef.nativeElement.contains(targetElement)
    ) {
      this.showDropdown = false;
    }
  }
}
